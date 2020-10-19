using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public GameObject NotEnoughEnergyPrefab;
    private EnergyController playerEnergy;
    private EnergyController enemyEnergy;
    private List<GameObject> arrAttackerPlayer;
    private List<GameObject> arrDefenderPlayer;
    
    private const int ATTACKER_STATUS_NONE = 0;
    private const int ATTACKER_STATUS_MOVE_TO_GET_BALL = 1;
    private const int ATTACKER_STATUS_HAS_BALL = 2;

    private const int NUMBER_PLAYER_GENERATE_FIRST_TIME = 10;
    private Vector3 pointerDown;
    // Start is called before the first frame update
    void Start()
    {
        pointerDown = Vector3.zero;
        playerEnergy = GameObject.Find("Battle/Canvas/Game_UI/PlayerInfo/FillBar").GetComponent<EnergyController>();
        enemyEnergy = GameObject.Find("Battle/Canvas/Game_UI/EnemyInfo/FillBar").GetComponent<EnergyController>();
        arrAttackerPlayer = new List<GameObject>();
        arrDefenderPlayer = new List<GameObject>();
        for(int i = 0;i<NUMBER_PLAYER_GENERATE_FIRST_TIME;i++)
        {
            GameObject go = Instantiate(GameController.gameControllerInstance.AttackerPlayerPrefab);
            go.transform.parent = transform;
            go.GetComponent<PlayerController>().InitPlayer(Vector3.zero,false);
            arrAttackerPlayer.Add(go);
            go.SetActive(false);

            go = Instantiate(GameController.gameControllerInstance.DefenderPlayerPrefab);
            go.transform.parent = transform;
            go.GetComponent<PlayerController>().InitPlayer(Vector3.zero,true);
            arrDefenderPlayer.Add(go);
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.gameControllerInstance.IsGamePause())
            return;

        if((GameController.gameControllerInstance.isPlayInARMode == false && HasPointerDown(out pointerDown)) || (GameController.gameControllerInstance.isPlayInARMode && GameController.gameControllerInstance.hasARpointerDown))
        {
            if(GameController.gameControllerInstance.IsPenaltyGame() == false)
            {
                GameController.gameControllerInstance.hasARpointerDown = false;
                Rect Azone = GameController.gameControllerInstance.GetAttackerZone();
                if(pointerDown.x > Azone.x && pointerDown.x < Azone.x + Azone.width)
                {
                    if(pointerDown.z > Azone.y && pointerDown.z < Azone.y + Azone.height)
                    {
                        GenerateAttacker(pointerDown);
                    }            
                }
                Rect Dzone = GameController.gameControllerInstance.GetDefenderZone();
                if(pointerDown.x > Dzone.x && pointerDown.x < Dzone.x + Dzone.width)
                {
                    if(pointerDown.z > Dzone.y && pointerDown.z < Dzone.y + Dzone.height)
                    {
                        GenerateDefender(pointerDown);
                    }            
                }
            }
            else
            {
                transform.GetComponent<GamePenalty>().UpdateManualMove(pointerDown);
            }
        }
        if(GameController.gameControllerInstance.IsPenaltyGame())
        {
            return;
        }

        GameObject playerhasball = null;
        int status = GetAttackerStatus(out playerhasball);
        if(status == ATTACKER_STATUS_NONE)
        {
            Vector3 ballpos;
            if(GameController.gameControllerInstance.GetBallPosition(out ballpos))
            {
                int index = GetNearestTo(arrAttackerPlayer,ballpos);
                if(index >= 0 && index < arrAttackerPlayer.Count)
                {
                    PlayerController playerController = arrAttackerPlayer[index].GetComponent<PlayerController>();
                    playerController.IsGoToGetBall = true;
                    playerController.SetPlayerMoveTo(ballpos,false);
                }
            }
        }
        else if(status == ATTACKER_STATUS_HAS_BALL)
        {
            DefenderGoToTarget(playerhasball);
        }

        AttackerMoveToTarget();
    }

    public void SetPointerDown(Vector3 pos)
    {
        pointerDown = pos;
    }
    public void EndMatch()
    {
        foreach(GameObject go in arrAttackerPlayer)
        {
            go.SetActive(false);
        }
        foreach(GameObject go in arrDefenderPlayer)
        {
            go.SetActive(false);
        }
        playerEnergy.ResetEnergy();
        enemyEnergy.ResetEnergy();
    }

    private bool DefenderGoToTarget(GameObject playerhasball)
    {
        if(playerhasball == null)
        {
            return false;
        }
        foreach(GameObject go in arrDefenderPlayer)
        {
            DefenderPlayer dp = go.GetComponent<DefenderPlayer>();
            if(dp.IsHasTarget() && dp.gameObject.activeSelf)
            {
               return false;
            }
        }
        int index = GetNearestTo(arrDefenderPlayer,playerhasball.transform.position);
        if(index != -1)
        {
           // Debug.Log("defender nearest index: " + index);
            DefenderPlayer defender = arrDefenderPlayer[index].GetComponent<DefenderPlayer>();
            float range = defender.rangeDefender/2 + (0.2f * GameController.gameControllerInstance.transform.localScale.x);//player size box
            //Debug.Log("namlh debug range : " + range);
            //Debug.Log("namlh deubg range 2: " + (0.2f * GameController.gameControllerInstance.transform.localScale.x));
			float distoplayer = Vector3.Distance(arrDefenderPlayer[index].transform.position,playerhasball.transform.position);
            if(distoplayer < range)
            {
                if(defender.defenderStatus == DefenderPlayer.DEFENDER_NONE)
                {
                    defender.SetTarget(playerhasball);
                }
            }
            
        }
        return false;
    }
    private int GetNearestTo(List<GameObject> list,Vector3 pos,float minRange = -1)
    {
        int i = 0, index = -1;
        Vector2 size = GameController.gameControllerInstance.GetLandSize();
        float mindis = Mathf.Max(size.x,size.y)*2;
        if(minRange < mindis && minRange > 0)
        {
            mindis = minRange;
        }
        for(;i<list.Count;i++)
        {
            //if(list[i].gameObject.layer == GameController.gameControllerInstance.layerMaskPlayerActive)
            if(list[i].GetComponent<PlayerController>().CurrentState == PlayerController.PLAYER_STATE_ACTIVE && list[i].activeSelf)
            {
                float dis = Vector3.Distance(pos,list[i].transform.position);
                if(dis < mindis)
                {
                    mindis = dis;
                    index = i;
                }
            }
        }
        return index;
    }
    private void AttackerMoveToTarget()
    {
        foreach(GameObject go in arrAttackerPlayer)
        {
            PlayerController playerController = go.GetComponent<PlayerController>();
            if(playerController.CurrentState == PlayerController.PLAYER_STATE_ACTIVE && go.activeSelf)
            {
                if(playerController.IsMoving == false)
                {
                    bool isEnemy = GameController.gameControllerInstance.isCurrentPlayerAttack();
                    bool isStraight = !playerController.HasCarryBall;
                    playerController.SetPlayerMoveTo(GameController.gameControllerInstance.GetGoalPosOf(isEnemy),isStraight);
                }
            }
        }
    }
    private int GetAttackerStatus(out GameObject playerhasball)
    {
        foreach(GameObject go in arrAttackerPlayer)
        {
            PlayerController playerController = go.GetComponent<PlayerController>();
            if(playerController.CurrentState == PlayerController.PLAYER_STATE_ACTIVE && go.activeSelf)
            {
                if (playerController.HasCarryBall) 
                {
                    {
                        playerhasball = go;
                        return ATTACKER_STATUS_HAS_BALL;
                    }
                }
                if(playerController.IsGoToGetBall)
                {
                    playerhasball = null;
                    return ATTACKER_STATUS_MOVE_TO_GET_BALL;
                }
            }
        }
        playerhasball = null;
        return ATTACKER_STATUS_NONE;
    }

    private void ShowNotEnoughEnergy(Vector3 pos)
    {
    #if !PLATFORM_ANDROID
        GameObject nee = Instantiate(NotEnoughEnergyPrefab);
        nee.transform.position = new Vector3(pos.x,pos.y+0.1f,pos.z);
        nee.transform.localScale = Vector3.one*GameController.gameControllerInstance.transform.localScale.x;
    #endif//
    }
    public void GeneratePenaltyPlayer(Vector3 pos)
    {
        pos.y = GameController.gameControllerInstance.GetLandPosY();
        int i = 0;
        bool isPlayerAttack = true;
        bool isEnemy = !isPlayerAttack;
        GameObject go;
        for(;i<arrAttackerPlayer.Count;i++)
        {
            go = arrAttackerPlayer[i];
            if(go.activeSelf == false)
            {
                //go.transform.parent = null;
                //go.transform.parent = transform;
                go.SetActive(true);
                go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);

                transform.GetComponent<GamePenalty>().StartPenaltyGame(go);

                return;
            }
        }
        go = Instantiate(GameController.gameControllerInstance.AttackerPlayerPrefab);
        go.transform.parent = transform;
        go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);

        transform.GetComponent<GamePenalty>().StartPenaltyGame(go);

        arrAttackerPlayer.Add(go);
    }
    private void GenerateAttacker(Vector3 pos)
    {
        pos.y = GameController.gameControllerInstance.GetLandPosY();
        int i = 0;
        bool isPlayerAttack = GameController.gameControllerInstance.isCurrentPlayerAttack();
        EnergyController energyController = playerEnergy;
        if(isPlayerAttack == false)
        {
            energyController = enemyEnergy;
        }
        bool isEnemy = !isPlayerAttack;
        if(energyController.SpentEnergy(GameController.gameControllerInstance.GetPlayerEnergyToSpawn(true)))
        {
            GameObject go;
            for(;i<arrAttackerPlayer.Count;i++)
            {
                go = arrAttackerPlayer[i];
                if(go.activeSelf == false)
                {
                    //go.transform.parent = null;
                    //go.transform.parent = transform;
                    go.SetActive(true);
                    go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
                    return;
                }
            }
            go = Instantiate(GameController.gameControllerInstance.AttackerPlayerPrefab);
            go.transform.parent = transform;
            go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
            
            arrAttackerPlayer.Add(go);
        }
        else
        {
            ShowNotEnoughEnergy(pos);
        }
    }

    void GenerateDefender(Vector3 pos)
    {
        pos.y = GameController.gameControllerInstance.GetLandPosY();
        int i = 0;
        bool isPlayerAttack = GameController.gameControllerInstance.isCurrentPlayerAttack();
        EnergyController energyController = enemyEnergy;
        if(isPlayerAttack == false)
        {
            energyController = playerEnergy;
        }
        bool isEnemy = isPlayerAttack;
        if(energyController.SpentEnergy(GameController.gameControllerInstance.GetPlayerEnergyToSpawn(false)))
        {
            GameObject go;
            for(;i<arrDefenderPlayer.Count;i++)
            {
                go = arrDefenderPlayer[i];
                if(go.activeSelf == false)
                {
                    //go.transform.parent = null;
                    //go.transform.parent = transform;
                    go.SetActive(true);
                    go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
                    return;
                }
            }
            go = Instantiate(GameController.gameControllerInstance.DefenderPlayerPrefab);
            go.transform.parent = transform;
            go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
            arrDefenderPlayer.Add(go);
        }
        else
        {
            ShowNotEnoughEnergy(pos);
        }
    }
    private bool HasPointerDown(out Vector3 pointdown)
    {
        #if PLATFORM_ANDROID && !UNITY_EDITOR
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            pointdown = Input.GetTouch(0).position;
        #else
        if(Input.GetMouseButtonDown(0))
        {
            pointdown = Input.mousePosition;
        #endif
            Ray ray = Camera.main.ScreenPointToRay(pointdown);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit,100,transform.GetComponent<GameController>().LayerMaskGround))
            {
                //Debug.Log("point down: " + hit.point);
                pointdown = hit.point;
                return true;
            }
        }
        pointdown = Vector3.zero;
        return false;
    }

    public bool PlayerPassBall(Vector3 position, out Vector3 playerReceiveBallPos)
    {
        int index = GetNearestTo(arrAttackerPlayer,position);
        if(index != -1)
        {
            PlayerController playerController = arrAttackerPlayer[index].GetComponent<PlayerController>();
            playerController.SetPlayerMoveTo(position,false);
            playerReceiveBallPos = arrAttackerPlayer[index].transform.position;
           //GameController.gameControllerInstance.MoveBallToPlayer(position,arrAttackerPlayer[index].transform.position);
            return true;
        }
        playerReceiveBallPos = Vector3.zero;
        return false;
    }
}
