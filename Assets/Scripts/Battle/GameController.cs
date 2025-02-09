﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameController : MonoBehaviour
{
    public int GameTimePerMatch;
    public Material GrayPlayerMaterial;
    public Material BluePlayerMaterial;
    public Material RedEnemyMaterial;
    public GameObject AttackerPlayerPrefab;
    public GameObject DefenderPlayerPrefab;
    public LayerMask LayerMaskGround;

    public GameObject GameBallPrefab;
    private bool isFirstLaunch = true;
    private bool isGamePause = false;

    private float totalTime;
    private float timeLimit;
    private TextMeshProUGUI timeRemain;
    private GameObject goGamePause;
    private TextMeshProUGUI txtEnemyTitle;
    private TextMeshProUGUI txtPlayerTitle;

    private int currnetMatch;
    private int[] arrResultMatch;

    private Rect playerZone = Rect.zero;
    private Rect enemyZone = Rect.zero;

    private GameObject gameBall;
    private bool isNeedGenerateBall;

    public GameObject EnemyGoalPos;
    public GameObject PlayerGoalPos;

    //public int layerMaskPlayerActive = 9;//replace with LayerMask.tNameToPlayer()
    //public int layerMaskPlayerInActive = 11;

    public static GameController gameControllerInstance = null;

    private Vector2 sizeLandField;

    private const int MATCH_WIN = 1;
    private const int MATCH_LOSE = 2;
    private const int MATCH_DRAW = 3;

    private const int STATE_GAME_NONE = -1;
    private const int STATE_GAME_PLAY = 0;
    private const int STATE_RESULT_MATCH = 1;
    private const int STATE_RESULT_5_MATCH = 2;
    private const int STATE_GAME_PENALTY = 3;
    private const int STATE_FINAL_RESULT = 4;
    private const int STATE_WAITING_AR = 5;

    private GameObject goEndMatch;
    private GameObject goEndGame;
    private GameObject goTextWaitingAR;

    private GameObject goYard;
    private int finalresultGame;

    private const int NUMBER_MATCH = 5;

    private float scaleGameInARMode = 0.1f;
    public bool isCheatdraw = true;

    [HideInInspector] public bool isPlayInARMode = false;
    [HideInInspector] public bool hasARpointerDown = false;

    private int gameState;
    private int preGameState;
    void Awake()
    {
        if(gameControllerInstance == null)
        {
            gameControllerInstance = this;
        }
        else if(gameControllerInstance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        CaculatePlayZone(transform.localScale.x,transform.position);
    }
    // Start is called before the first frame update
    void Start()
    {
        if(isFirstLaunch) 
        {
            isFirstLaunch = false;
        }
        //Debug.Log("start Game Controller");
        timeRemain = GameObject.Find("Battle/Canvas/Game_UI/Time/Timeremain").GetComponentInChildren<TextMeshProUGUI>();
        goGamePause = GameObject.Find("Battle/Canvas/Game_UI/GamePause");
        txtEnemyTitle = GameObject.Find("Battle/Canvas/Game_UI/EnemyInfo/Name/txt").GetComponent<TextMeshProUGUI>();
        txtPlayerTitle = GameObject.Find("Battle/Canvas/Game_UI/PlayerInfo/Name/txt").GetComponent<TextMeshProUGUI>();
        goGamePause.SetActive(false);

        goEndMatch = GameObject.Find("Battle/Canvas/Game_UI/GameEndMatch");
        goEndMatch.SetActive(false);

        goEndGame = GameObject.Find("Battle/Canvas/Game_UI/GameEndGame");
        goEndGame.SetActive(false);

        goTextWaitingAR = GameObject.Find("Battle/Canvas/Game_UI/txtWaiting");;
        goTextWaitingAR.SetActive(false);

        goYard = GameObject.Find("Battle/yard");

        totalTime = 0;
        isNeedGenerateBall = false;
        finalresultGame = -1;
        StartNewGame();

        gameState = STATE_GAME_PLAY;
        preGameState = STATE_GAME_NONE;
    }

    private void switchGameState (int nextState)
    {
        
        if(gameState != nextState)
        {
            preGameState = gameState;
            gameState = nextState;
        }
    }
    public void PrepareForARGame()
    {
        isPlayInARMode = true;
        PauseGame(true);
        switchGameState(STATE_WAITING_AR);
        goYard.SetActive(false);
        GameObject goUI = GameObject.Find("Battle/Canvas/Game_UI");
        for(int i = 0;i<goUI.transform.childCount;i++)
        {
            goUI.transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.position = new Vector3(0,-100,0);
    }

    public void BackToNoneARGame()
    {
        isPlayInARMode = false;
        if(gameState == STATE_WAITING_AR)
        {
            goYard.SetActive(true);
            GameObject goUI = GameObject.Find("Battle/Canvas/Game_UI");
            for(int i = 0;i<goUI.transform.childCount;i++)
            {
                goUI.transform.GetChild(i).gameObject.SetActive(true);
            }
            //if(preGameState == STATE_GAME_PLAY)
            {
                goEndGame.SetActive(false);
                goEndMatch.SetActive(false);
            }
            goTextWaitingAR.SetActive(false);
            switchGameState(preGameState);
        }
        ResetBattleLocation();
    }
    public void SetUpARForBattle(Vector3 pos, Quaternion quaternion, Vector3 scale)
    {
        //transform.rotation = quaternion;
        if(gameState == STATE_WAITING_AR)
        {
            goYard.SetActive(true);
            GameObject goUI = GameObject.Find("Battle/Canvas/Game_UI");
            for(int i = 0;i<goUI.transform.childCount;i++)
            {
                goUI.transform.GetChild(i).gameObject.SetActive(true);
            }
            //if(preGameState == STATE_GAME_PLAY)
            {
                goEndGame.SetActive(false);
                goEndMatch.SetActive(false);
            }
            goTextWaitingAR.SetActive(false);
            switchGameState(preGameState);
        }
        transform.localScale = scale*scaleGameInARMode;
        transform.position = new Vector3(pos.x,pos.y +0.01f,pos.z);
        //Debug.Log("namlh battle pos: " + pos);
        CaculatePlayZone(transform.localScale.x,transform.position);
    }

    void CaculatePlayZone(float scale, Vector3 deltaPos)
    {
        GameObject yard = FindChildByName(transform,"yard");
        GameObject land = FindChildByName(yard.transform,"Plane");

        sizeLandField = new Vector2(yard.transform.localScale.x * land.transform.localScale.x,yard.transform.localScale.z * land.transform.localScale.z);
        sizeLandField*=scale;
        Vector3 playersizebox = GetPlayerSizeBox()*scale;
        float recx = -sizeLandField.x/2 + playersizebox.x + deltaPos.x;
        float recz = -sizeLandField.y/2+playersizebox.z + deltaPos.z;
        float width = sizeLandField.x- playersizebox.x;
        float height = sizeLandField.y/2 - playersizebox.z;
        playerZone = new Rect(recx,recz,width,height);
        recz = 0 +playersizebox.z + deltaPos.z;
        enemyZone = new Rect(recx,recz,width,height);
        //Debug.Log("size field: " + sizeLandField);
        //Debug.Log("player Zone: " + playerZone);
        //Debug.Log("Enemy Zoen: " + enemyZone);
    }

    public void ResetBattleLocation()
    {
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        transform.position = Vector3.zero;

        CaculatePlayZone(transform.localScale.x,transform.position);
    }
    public void SetARPointerDown(Vector3 pos)
    {
        if(isGamePause)
        {
            return;
        }
        hasARpointerDown = true;
        //pos.x = (pos.x - transform.position.x)/scaleGameInARMode;
        //pos.y = (pos.y -transform.position.y)/scaleGameInARMode;
        //pos.z = (pos.z - transform.position.z)/scaleGameInARMode;
        //Debug.Log("namlh point on Game: " + pos);
        transform.GetComponent<GamePlay>().SetPointerDown(pos);
    }
    public Vector2 GetLandSize()
    {
        //CaculatePlayZone(transform.localScale.x,transform.position);
        return sizeLandField;
    }
    public bool GetBallPosition(out Vector3 pos)
    {
        if(gameBall != null)
        {
            pos = gameBall.transform.position;
            return true;
        }
        pos = Vector3.zero;
        return false;
    }

    public void PlayerTakeBall()
    {
        BallController ballController = gameBall.GetComponent<BallController>();
        ballController.SetMoving(false);
        gameBall.transform.parent = null;
        Destroy(gameBall);
        //gameBall.SetActive(false);
        //gameBall.transform.parent = null;
    }

    public void PlayerPassBall(Vector3 position)
    {
        Vector3 playerReceiveBallPos;
        if(transform.GetComponent<GamePlay>().PlayerPassBall(position,out playerReceiveBallPos) == false)
        {
            if(isCurrentPlayerAttack())
            {
                arrResultMatch[currnetMatch] = MATCH_LOSE;
            }
            else
            {
                arrResultMatch[currnetMatch] = MATCH_WIN;
            }
            switchGameState(STATE_RESULT_MATCH);
        }
        else
        {
            //gameBall.SetActive(true);
            gameBall = Instantiate(GameBallPrefab);
            gameBall.transform.parent = transform;
            position.y = GetLandPosY() + (gameBall.GetComponent<SphereCollider>().radius*transform.localScale.y);
            gameBall.transform.position = position;
            gameBall.transform.localScale = Vector3.one;
            Debug.Log("namlh debug pass ball pos:" + gameBall.transform.position + " player pos: " + playerReceiveBallPos);
            BallController ballController = gameBall.GetComponent<BallController>();
            ballController.setTargetPos(playerReceiveBallPos);
        }
    }
    public Rect GetAttackerZone()
    {
        if(isCurrentPlayerAttack())
            return playerZone;
        return enemyZone;
    }
    
    public Rect GetDefenderZone()
    {
        if(isCurrentPlayerAttack())
            return enemyZone;
        return playerZone;
    }

    public Vector3 GetPlayerSizeBox()
    {
        return AttackerPlayerPrefab.transform.GetComponent<BoxCollider>().size;
    }

    public bool isCurrentPlayerAttack()
    {
        if(IsPenaltyGame())
        {
            return true;
        }
        return (currnetMatch%2==0);
    }
    public float GetPlayerEnergyToSpawn(bool isAttack)
    {
        if(isAttack)
        {
            return AttackerPlayerPrefab.GetComponent<AttackerPlayer>().energyCost;
        }
        else
        {
            return DefenderPlayerPrefab.GetComponent<DefenderPlayer>().energyCost;
        }
    }
    public float GetEnemyEnergyToSpawn()
    {
        if(isCurrentPlayerAttack() == false)
        {
            return AttackerPlayerPrefab.GetComponent<AttackerPlayer>().energyCost;
        }
        else
        {
            return DefenderPlayerPrefab.GetComponent<DefenderPlayer>().energyCost;
        }
    }

    public Vector3 GetGoalPosOf(bool isEnemy)
    {
        if(isEnemy)
        {
            return EnemyGoalPos.transform.position;
        }
        return PlayerGoalPos.transform.position;
    }

    public float GetLandPosY()
    {
        return transform.position.y;
    }
    public void StartNextMatch()
    {
        timeLimit = GameTimePerMatch;
        currnetMatch++;
        SetPlayerTitle();
        isNeedGenerateBall = true;
        switchGameState(STATE_GAME_PLAY);
        goEndMatch.SetActive(false);
        PauseGame(false);
    }
    private void EndCurrentMatch()
    {
        StartNextMatch();
    }
    public void StartNewGame()
    {
        if(finalresultGame == MATCH_DRAW )
        {
            StartPenaltyGame();
            return;
        }
        isNeedGenerateBall = true;
        //transform.GetComponent<MazeGenerator>().ClearGrib();
        finalresultGame = -1;
        currnetMatch = -1;
        arrResultMatch = new int[NUMBER_MATCH];
        StartNextMatch();
        goEndGame.SetActive(false);
        PauseGame(false);
    }

    private void StartPenaltyGame()
    {
        switchGameState(STATE_GAME_PENALTY);
        isNeedGenerateBall = true;
        transform.GetComponent<MazeGenerator>().GenerateGrid();
        goEndGame.SetActive(false);
        PauseGame(false);
        Vector3 pos = transform.GetComponent<MazeGenerator>().GetPlayerPenaltyPos();
        transform.GetComponent<GamePlay>().GeneratePenaltyPlayer(pos);
        if(transform.GetComponent<GamePenalty>().isManualMove)
        {
            timeLimit = GameTimePerMatch/3;
        }
        else
        {
            timeLimit = GameTimePerMatch;
        }
    }

    public bool IsPenaltyGame()
    {
        return (gameState == STATE_GAME_PENALTY);
    }
    private void SetPlayerTitle()
    {
        if(currnetMatch >= 0 && currnetMatch <= arrResultMatch.Length)
        {
            if(isCurrentPlayerAttack())
            {
                txtEnemyTitle.text = "ENEMY (DEFENDER)";
                txtPlayerTitle.text = "PLAYER (ATTAKER)";
            }
            else
            {
                txtEnemyTitle.text = "ENEMY (ATTAKER)";
                txtPlayerTitle.text = "PLAYER (DEFENDER)";
            }
            
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
    #if !UNITY_EDITOR
        if(hasFocus== false)
        {
            PauseGame(!hasFocus);
        }
    #endif
    }

    private string GetCurrentScore()
    {
        int playerscore  = 0;
        int enemyscore = 0;
        for(int i= 0;i<=currnetMatch;i++)
        {
            if(arrResultMatch[i] == MATCH_WIN)
            {
                playerscore++;
            }
            else if(arrResultMatch[i] == MATCH_LOSE)
            {
                enemyscore++;
            }
        }
        if(currnetMatch >= arrResultMatch.Length -1)
        {
            if(isCheatdraw)
            {
                playerscore = enemyscore;
            }
            if(playerscore > enemyscore)
            {
                finalresultGame = MATCH_WIN;
                return "YOU WIN"; 
            }
            else if(playerscore < enemyscore)
            {
                finalresultGame = MATCH_LOSE;
                return "YOU LOSE";
            }
            else
            {
                finalresultGame = MATCH_DRAW;
                return "DRAW";
            }
        }
        return "PLAYER " + playerscore + " - " + enemyscore + " ENEMY";
    }
    // Update is called once per frame
    void Update()
    {
        
        switch(gameState)
        {
            case STATE_RESULT_MATCH:
            {
                if(currnetMatch >= arrResultMatch.Length-1)
                {
                    transform.GetComponent<GamePlay>().EndMatch();
                    switchGameState(STATE_RESULT_5_MATCH);
                }
                else
                {
                    if(goEndMatch.activeSelf == false)
                    {
                        transform.GetComponent<GamePlay>().EndMatch();
                        PauseGame(true);
                        goGamePause.SetActive(false);
                        goEndMatch.SetActive(true);
                        string currentScore = GetCurrentScore();
                        FindChildByName(goEndMatch.transform,"currentScore").GetComponent<TextMeshProUGUI>().text = currentScore;
                    }
                }
            }
            break;
            case STATE_RESULT_5_MATCH:
            {
                if(goEndGame.activeSelf == false)
                {
                    PauseGame(true);
                    goEndGame.SetActive(true);
                    goGamePause.SetActive(false);
                    string currentScore = GetCurrentScore();
                    FindChildByName(goEndGame.transform,"endScore").GetComponent<TextMeshProUGUI>().text = currentScore;
                    if(finalresultGame == MATCH_DRAW)
                    {
                        FindChildByName(goEndGame.transform,"Button").GetComponentInChildren<TextMeshProUGUI>().text = "PLAY PENALTY";
                    }
                    else
                    {
                        FindChildByName(goEndGame.transform,"Button").GetComponentInChildren<TextMeshProUGUI>().text = "PLAY AGAIN";
                    }
                }
            }
            break;
            case STATE_WAITING_AR:
            {
                if(goTextWaitingAR.activeSelf == false)
                {
                    goTextWaitingAR.SetActive(true);
                }
            }
            break;
            case STATE_GAME_PENALTY:
            {
            }
            break;
            default:
            if(goTextWaitingAR.activeSelf)
            {
                goTextWaitingAR.SetActive(false);
            }
            break;
        }
        if(isGamePause)
            return;

        totalTime += Time.deltaTime;
        timeLimit -= Time.deltaTime;
        timeRemain.text = Mathf.RoundToInt(timeLimit) + "s";
        GenerateBall();
        if(timeLimit<0)
        {
            PauseGame(true);
            if(IsPenaltyGame())
            {
                arrResultMatch[currnetMatch] = MATCH_LOSE;
            }
            else
            {
                arrResultMatch[currnetMatch] = MATCH_DRAW;
            }
            switchGameState(STATE_RESULT_MATCH);
            //gameBall.SetActive(false);
            if(gameBall)
            {
                gameBall.transform.parent = null;
                Destroy(gameBall);
            }
        }
    }

    private void GenerateBall()
    {
        if(gameBall == null && isNeedGenerateBall)
        {
            isNeedGenerateBall = false;
            
            gameBall = Instantiate(GameBallPrefab);
            Rect zone = GetAttackerZone();
           
            if(gameBall.transform.parent != transform)
            {
                gameBall.transform.parent = transform;
            }
            Vector3 posSpawn = Vector3.zero;
            float delta = 0.2f*transform.localScale.y;//player size for incase the ball it to near the wall
            posSpawn.x = Random.Range(zone.x + delta,zone.x + zone.width - delta );// + transform.position.x;
            posSpawn.z = Random.Range(zone.y + delta,zone.y + zone.height - delta);//  + transform.position.z;
            posSpawn.y = GetLandPosY() + (gameBall.GetComponent<SphereCollider>().radius*transform.localScale.y);
            //Debug.Log("pospawn: " + posSpawn + " battle pos: " + transform.position + " landposy : " + GetLandPosY() + " scaley: " + transform.localScale.y + " radius: " + gameBall.GetComponent<SphereCollider>().radius);
            //Debug.Log("zone : " + zone);

            if(IsPenaltyGame())
            {
                Vector3 temppos = transform.GetComponent<MazeGenerator>().GeneratePosBall();
                posSpawn.x = temppos.x;
                posSpawn.z = temppos.z; 
            }
            gameBall.transform.position = posSpawn;
            gameBall.transform.localScale = Vector3.one;
            
            Debug.Log("namlh debug Generate ball pos:" + gameBall.transform.position);
            
            gameBall.tag = "battle_ball";
        }
    }
    public bool IsGamePause()
    {
        return isGamePause;
    }
   public void PauseGame(bool pause)
   {
       if(isGamePause != pause)
       {
           isGamePause = pause;
           if(isGamePause)
            {
                goGamePause.SetActive(true);
                Time.timeScale = 0;
                AudioListener.pause = true;
            }
            else
            {
                goGamePause.SetActive(false);
                Time.timeScale = 1;
                AudioListener.pause = false;
            }
       }
   }

    public void PenaltyFailed()
    {
        for(int i = 0;i<arrResultMatch.Length;i++)
        {
            arrResultMatch[i] = MATCH_LOSE;
        }
        if(gameBall)
        {
            Destroy(gameBall);
        }
        switchGameState(STATE_RESULT_MATCH);
    }
    public void AttackerScoreGoal()
    {
        if(IsPenaltyGame())
        {
            for(int i = 0;i<arrResultMatch.Length;i++)
            {
                arrResultMatch[i] = MATCH_WIN;
            }
        }
        else
        {
            if(isCurrentPlayerAttack())
            {
                arrResultMatch[currnetMatch] = MATCH_WIN;
            }
            else
            {
                arrResultMatch[currnetMatch] = MATCH_LOSE;
            }
        }
        switchGameState(STATE_RESULT_MATCH);
    }
   public GameObject FindChildByName(Transform parent,string name)
   {
       for(int i = 0;i<parent.childCount;i++)
       {
           if(parent.GetChild(i).gameObject.name.Equals(name))
           {
               return parent.GetChild(i).gameObject;
           }
       }
       return null;
   }
}
