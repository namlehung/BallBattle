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
    private bool[] arrIsPlayerAttack = {true,false,true,false,true};

    private bool isEndMatch;

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

    private GameObject goEndMatch;
    private GameObject goEndGame;
    private int finalresultGame;

    private const int NUMBER_MATCH = 5;

    private float scaleGameInARMode = 0.1f;
    public bool isCheatdraw = true;
    public bool isPauseGameAtStart = false;
    [HideInInspector] public bool isPlayInARMode = false;
    [HideInInspector] public bool hasARpointerDown = false;
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
    }
    // Start is called before the first frame update
    void Start()
    {
        if(isFirstLaunch) 
        {
            isFirstLaunch = false;
        }
        Debug.Log("start Game Controller");
        timeRemain = GameObject.Find("Battle/Canvas/Game_UI/Time/Timeremain").GetComponentInChildren<TextMeshProUGUI>();
        goGamePause = GameObject.Find("Battle/Canvas/Game_UI/GamePause");
        txtEnemyTitle = GameObject.Find("Battle/Canvas/Game_UI/EnemyInfo/Name/txt").GetComponent<TextMeshProUGUI>();
        txtPlayerTitle = GameObject.Find("Battle/Canvas/Game_UI/PlayerInfo/Name/txt").GetComponent<TextMeshProUGUI>();
        goGamePause.SetActive(false);

        goEndMatch = GameObject.Find("Battle/Canvas/Game_UI/GameEndMatch");
        goEndMatch.SetActive(false);

        goEndGame = GameObject.Find("Battle/Canvas/Game_UI/GameEndGame");
        goEndGame.SetActive(false);

        totalTime = 0;
        isNeedGenerateBall = false;
        finalresultGame = -1;
        StartNewGame();

        CaculatePlayZone(transform.localScale.x,transform.position);

        //gameBall = Instantiate(GameBallPrefab);
        //gameBall.transform.parent = transform;
        //gameBall.SetActive(false);

        // if(isPauseGameAtStart)
        // {
        //     PauseGame(true);
        // }
    }

    public void HideGameWaitForAR()
    {
        // for(int i =0;i<transform.childCount;i++)
        // {
        //     GameObject gochild = transform.GetChild(i).gameObject;
        //     if(gochild.name.Equals("Canvas"))
        //     {
        //         for(int j =0;j<gochild.transform.childCount;j++)
        //         {
        //             if(!gochild.transform.GetChild(j).name.Equals("ARToggleButton") && gochild.transform.GetChild(j).gameObject.activeSelf)
        //             {
        //                 gochild.transform.GetChild(j).gameObject.SetActive(false);
        //             }
        //         }
        //     }
        //     else if(gochild.name.Equals("yard"))
        //     {
        //         gochild.SetActive(false);
        //     }
        // }
        transform.position = new Vector3(0,-100,0);
    }

    public void ShowGameWhenReady()
    {
        // for(int i =0;i<transform.childCount;i++)
        // {
        //     GameObject gochild = transform.GetChild(i).gameObject;
        //     if(gochild.name.Equals("Canvas"))
        //     {
        //         for(int j =0;j<gochild.transform.childCount;j++)
        //         {
        //             if(!gochild.transform.GetChild(j).name.Equals("ARToggleButton") && gochild.transform.GetChild(j).gameObject.activeSelf == false)
        //             {
        //                 gochild.transform.GetChild(j).gameObject.SetActive(true);
        //             }
        //         }
        //     }
        //     else if(gochild.name.Equals("yard"))
        //     {
        //         gochild.SetActive(true);
        //     }
        // }
    }
    public void SetUpARForBattle(Vector3 pos, Quaternion quaternion, Vector3 scale)
    {
        //transform.rotation = quaternion;
       
        transform.localScale = scale*scaleGameInARMode;
        transform.position = new Vector3(pos.x,pos.y +0.01f,pos.z);
        Debug.Log("namlh battle pos: " + pos);
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
        Debug.Log("size field: " + sizeLandField);
        Debug.Log("player Zone: " + playerZone);
        Debug.Log("Enemy Zoen: " + enemyZone);
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
        Debug.Log("namlh point on Game: " + pos);
        transform.GetComponent<GamePlay>().SetPointerDown(pos);
    }
    public Vector2 GetLandSize()
    {
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
            transform.GetComponent<GamePlay>().EndMatch();
            if(isCurrentPlayerAttack())
            {
                arrResultMatch[currnetMatch] = MATCH_LOSE;
            }
            else
            {
                arrResultMatch[currnetMatch] = MATCH_WIN;
            }
            isEndMatch = true;
        }
        else
        {
            //gameBall.SetActive(true);
            gameBall = Instantiate(GameBallPrefab);
            gameBall.transform.parent = transform;
            position.y = GetLandPosY() + (gameBall.GetComponent<SphereCollider>().radius*gameBall.transform.localScale.y);
            gameBall.transform.position = position;
            gameBall.transform.localScale = Vector3.one;
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
        return arrIsPlayerAttack[currnetMatch];
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
        if(!arrIsPlayerAttack[currnetMatch])
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
        isEndMatch = false;
        timeLimit = GameTimePerMatch;
        currnetMatch++;
        SetPlayerTitle();
        isNeedGenerateBall = true;
        goEndMatch.SetActive(false);
        PauseGame(false);
    }
    private void EndCurrentMatch()
    {
        StartNextMatch();
    }
    public void StartNewGame()
    {
        if((finalresultGame == MATCH_DRAW || isCheatdraw) && currnetMatch>=NUMBER_MATCH-1)
        {
            StartPenaltyGame();
            return;
        }
        finalresultGame = -1;
        currnetMatch = -1;
        arrResultMatch = new int[NUMBER_MATCH];
        StartNextMatch();
        goEndGame.SetActive(false);
        PauseGame(false);
    }

    private void StartPenaltyGame()
    {

    }
    private void SetPlayerTitle()
    {
        if(currnetMatch >= 0 && currnetMatch <= arrIsPlayerAttack.Length)
        {
            if(arrIsPlayerAttack[currnetMatch])
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
        if(isEndMatch)
        {
            if(currnetMatch >= arrResultMatch.Length-1)
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
            else
            {
                if(goEndMatch.activeSelf == false)
                {
                    PauseGame(true);
                    goGamePause.SetActive(false);
                    goEndMatch.SetActive(true);
                    string currentScore = GetCurrentScore();
                    FindChildByName(goEndMatch.transform,"currentScore").GetComponent<TextMeshProUGUI>().text = currentScore;
                }
            }
        }
        if(isGamePause)
            return;

        totalTime += Time.deltaTime;
        timeLimit -= Time.deltaTime;
        timeRemain.text = Mathf.RoundToInt(timeLimit) + "s";
        GenerateBall();
        if(timeLimit<0)
        {
            isEndMatch = true;
            transform.GetComponent<GamePlay>().EndMatch();
            //gameBall.SetActive(false);
            gameBall.transform.parent = null;
            Destroy(gameBall);
            arrResultMatch[currnetMatch] = MATCH_DRAW;
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
            posSpawn.x = Random.Range(zone.x,zone.x + zone.width);// + transform.position.x;
            posSpawn.z = Random.Range(zone.y,zone.y + zone.height);//  + transform.position.z;
            posSpawn.y = GetLandPosY() + (gameBall.GetComponent<SphereCollider>().radius*transform.localScale.y);
            Debug.Log("pospawn: " + posSpawn + " battle pos: " + transform.position + " landposy : " + GetLandPosY() + " scaley: " + transform.localScale.y + " radius: " + gameBall.GetComponent<SphereCollider>().radius);
            //posSpawn.x += transform.position.x;
            //posSpawn.z += transform.position.z;
            Debug.Log("zone : " + zone + " posSwan: " + posSpawn);

            gameBall.transform.position = posSpawn;
            gameBall.transform.localScale = Vector3.one;
            //gameBall.transform.parent = transform;
            Debug.Log("Generate ball pos:" + gameBall.transform.position);
            //Vector3 localscale = gameBall.transform.localScale;
            //gameBall.transform.localScale = new Vector3(localscale.x*transform.localScale.x,localscale.y*transform.localScale.y,localscale.z*transform.localScale.z);
            //gameBall.SetActive(true);
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
                if(isPlayInARMode)
                {
                    Time.timeScale = 1;//*scaleGameInARMode;
                }
                else
                {
                    Time.timeScale = 1;
                }
                AudioListener.pause = false;
            }
       }
   }

    public void AttackerScoreGoal()
    {
        transform.GetComponent<GamePlay>().EndMatch();
        if(isCurrentPlayerAttack())
        {
            arrResultMatch[currnetMatch] = MATCH_WIN;
        }
        else
        {
            arrResultMatch[currnetMatch] = MATCH_LOSE;
        }
        isEndMatch = true;
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
