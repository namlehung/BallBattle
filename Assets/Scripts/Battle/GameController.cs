using System.Collections;
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
    private static bool isFirstLaunch = true;
    private static bool isGamePause = false;

    private float totalTime;
    private float timeLimit;
    private TextMeshProUGUI timeRemain;
    private TextMeshProUGUI txtGamePause;
    private TextMeshProUGUI txtEnemyTitle;
    private TextMeshProUGUI txtPlayerTitle;

    private int currnetMatch;
    private int[] arrResultMatch;
    private bool[] arrIsPlayerAttack = {true,false,true,false,true};

    private bool isEndMatch;

    private static Rect playerZone = Rect.zero;
    private static Rect enemyZone = Rect.zero;

    private GameObject gameBall;

    public GameObject EnemyGoalPos;
    public GameObject PlayerGoalPos;

    public int layerMaskPlayerActive = 9;
    public int layerMaskPlayerInActive = 11;
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.isFirstLaunch) 
        {
            GameController.isFirstLaunch = false;
        }
       
        timeRemain = GameObject.Find("Canvas/Game_UI/Time/Timeremain").GetComponentInChildren<TextMeshProUGUI>();
        txtGamePause = GameObject.Find("Canvas/Game_UI/txtGamePause").GetComponent<TextMeshProUGUI>();
        txtEnemyTitle = GameObject.Find("Canvas/Game_UI/EnemyInfo/Name/txt").GetComponent<TextMeshProUGUI>();
        txtPlayerTitle = GameObject.Find("Canvas/Game_UI/PlayerInfo/Name/txt").GetComponent<TextMeshProUGUI>();
        txtGamePause.gameObject.SetActive(false);
        totalTime = 0;
        StartNewGame();

        GameObject yard = FindChildByName(transform,"yard");
        GameObject land = FindChildByName(yard.transform,"Plane");
        Vector2 sizeLandField = new Vector2(yard.transform.localScale.x * land.transform.localScale.x,yard.transform.localScale.z * land.transform.localScale.z);
        playerZone = new Rect(-sizeLandField.x/2,-sizeLandField.y/2,sizeLandField.x,sizeLandField.y/2);
        
        Vector3 playersizebox = GetPlayerSizeBox();
        enemyZone = new Rect(-sizeLandField.x/2 + playersizebox.x,0+playersizebox.z,sizeLandField.x - playersizebox.x,sizeLandField.y/2 - playersizebox.z);

        gameBall = Instantiate(GameBallPrefab);
        gameBall.SetActive(false);
    }

    public bool GetBallPosition(out Vector3 pos)
    {
        if(gameBall.activeSelf)
        {
            pos = gameBall.transform.position;
        }
        pos = Vector3.zero;
        return false;
    }
    public static Rect GetPlayerZone()
    {
        return playerZone;
    }
    
    public static Rect GetEnemyZone()
    {
        return enemyZone;
    }

    public Vector3 GetPlayerSizeBox()
    {
        return AttackerPlayerPrefab.transform.GetComponent<BoxCollider>().size;
    }

    public bool isCurrentPlayerAttack()
    {
        return arrIsPlayerAttack[currnetMatch];
    }
    public float GetPlayerEnergyToSpawn()
    {
        if(arrIsPlayerAttack[currnetMatch])
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

    public Vector3 GetGoalPosOfEnemy(bool isEnemy)
    {
        if(isEnemy)
        {
            return EnemyGoalPos.transform.position;
        }
        return PlayerGoalPos.transform.position;
    }

    public float GetLandPosY()
    {
        return 0;
    }
    private void StartNextMatch()
    {
        isEndMatch = false;
        timeLimit = GameTimePerMatch;
        currnetMatch++;
        SetPlayerTitle();
    }
    private void EndCurrentMatch()
    {
        StartNextMatch();
    }
    private void StartNewGame()
    {
        currnetMatch = -1;
        arrResultMatch = new int[5];
        StartNextMatch();
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
    public void CheckAndGotoAR()
    {
    #if PLATFORM_ANDROID
        if(GamePermission.HasPermissionToUseAR())
        {
            SceneController.GotoARScene();
        }
    #endif
    }
    void OnApplicationFocus(bool hasFocus)
    {
    #if !UNITY_EDITOR
        PauseGame(!hasFocus);
    #endif
    }

    // Update is called once per frame
    void Update()
    {
        if(isGamePause)
            return;

        totalTime += Time.deltaTime;
        timeLimit -= Time.deltaTime;
        timeRemain.text = Mathf.RoundToInt(timeLimit) + "s";
    }
    public static bool IsGamePause()
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
                txtGamePause.gameObject.SetActive(true);
                Time.timeScale = 0;
                AudioListener.pause = true;
            }
            else
            {
                txtGamePause.gameObject.SetActive(false);
                Time.timeScale = 1;
                AudioListener.pause = false;
            }
       }
   }

   public static GameObject FindChildByName(Transform parent,string name)
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
