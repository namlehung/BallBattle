using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    private EnergyController playerEnergy;
    private EnergyController enemyEnergy;
    private List<GameObject> arrAttackerPlayer;
    private List<GameObject> arrDefenderPlayer;
    
    private const int ATTACKER_STATUS_NONE = 0;
    private const int ATTACKER_STATUS_MOVE_TO_GET_BALL = 1;
    private const int ATTACKER_STATUS_HAS_BALL = 2;
    // Start is called before the first frame update
    void Start()
    {
        playerEnergy = GameObject.Find("Canvas/Game_UI/PlayerInfo/FillBar").GetComponent<EnergyController>();
        enemyEnergy = GameObject.Find("Canvas/Game_UI/EnemyInfo/FillBar").GetComponent<EnergyController>();
        arrAttackerPlayer = new List<GameObject>();
        arrDefenderPlayer = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.gameControllerInstance.IsGamePause())
            return;
        Vector3 pos;
        if(HasPointerDown(out pos))
        {
            Rect Azone = GameController.gameControllerInstance.GetAttackerZone();
            if(pos.x > Azone.x && pos.x < Azone.x + Azone.width)
            {
                if(pos.z > Azone.y && pos.z < Azone.y + Azone.height)
                {
                    GenerateAttacker(pos);
                }            
            }
            Rect Dzone = GameController.gameControllerInstance.GetDefenderZone();
            if(pos.x > Dzone.x && pos.x < Dzone.x + Dzone.width)
            {
                if(pos.z > Dzone.y && pos.z < Dzone.y + Dzone.height)
                {
                    GenerateDefender(pos);
                }            
            }
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
        /*/foreach(GameObject go in arrDefenderPlayer)
        {
            PlayerController playerController = go.GetComponent<PlayerController>();
            if(playerController.CurrentState == PlayerController.PLAYER_STATE_ACTIVE && go.activeSelf)
            {
                DefenderPlayer defender = go.GetComponent<DefenderPlayer>();
                float range = defender.rangeDefender;
                if(defender.defenderStatus == DefenderPlayer.DEFENDER_NONE && Vector3.Distance(go.transform.position,playerhasball.transform.position) < range)
                {
                    defender.SetTarget(playerhasball);
                }
            }
        }*/
        int index = GetNearestTo(arrDefenderPlayer,playerhasball.transform.position);
        if(index != -1)
        {
           // Debug.Log("defender nearest index: " + index);
            DefenderPlayer defender = arrDefenderPlayer[index].GetComponent<DefenderPlayer>();
            float range = defender.rangeDefender;
            if(defender.defenderStatus == DefenderPlayer.DEFENDER_NONE && Vector3.Distance(arrDefenderPlayer[index].transform.position,playerhasball.transform.position) < range)
            {
                defender.SetTarget(playerhasball);
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
                    go.SetActive(true);
                    go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
                    return;
                }
            }
            go = Instantiate(GameController.gameControllerInstance.AttackerPlayerPrefab);
            go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
            go.transform.parent = transform;
            arrAttackerPlayer.Add(go);
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
                    go.SetActive(true);
                    go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
                    return;
                }
            }
            go = Instantiate(GameController.gameControllerInstance.DefenderPlayerPrefab);
            go.GetComponent<PlayerController>().InitPlayer(pos,isEnemy);
            go.transform.parent = transform;
            arrDefenderPlayer.Add(go);
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

    public bool PlayerPassBall(Vector3 position)
    {
        int index = GetNearestTo(arrAttackerPlayer,position);
        if(index != -1)
        {
            PlayerController playerController = arrAttackerPlayer[index].GetComponent<PlayerController>();
            playerController.SetPlayerMoveTo(position,false);
           GameController.gameControllerInstance.MoveBallToPlayer(arrAttackerPlayer[index].transform.position);
            return true;
        }
        return false;
    }
}
