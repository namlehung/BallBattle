using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private EnergyController playerEnergy;
    private List<GameObject> arrAttackerPlayer;
    private List<GameObject> arrDefenderPlayer;
    
    private GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = transform.GetComponent<GameController>();
        playerEnergy = GameObject.Find("Canvas/Game_UI/PlayerInfo/FillBar").GetComponent<EnergyController>();
        arrAttackerPlayer = new List<GameObject>();
        arrDefenderPlayer = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.IsGamePause())
            return;
        Vector3 pos;
        if(HasPointerDown(out pos))
        {
            Rect zone = GameController.GetPlayerZone();
            Vector3 size = gameController.GetPlayerSizeBox();
            if(pos.x > zone.x && pos.x < zone.x + zone.width)
            {
                if(pos.z > zone.y && pos.z < zone.y + zone.height)
                {
                    if(pos.x - size.x < zone.x)
                    {
                        pos.x += size.x;
                    }
                    if(pos.z - size.z < zone.y)
                    {
                        pos.z += size.z;
                    }  
                    if(pos.x + size.x > zone.x + zone.width)
                    {
                        pos.x -= size.x;
                    }
                    if(pos.z + size.z > zone.y + zone.height)
                    {
                        pos.z -= size.z;
                    }
                    if(playerEnergy.SpentEnergy(gameController.GetPlayerEnergyToSpawn()))
                    {
                        GeneratePlayer(pos);
                    }
                }            
            }
        }
        
        foreach(GameObject go in arrAttackerPlayer)
        {
            if(go.activeSelf)
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if(pc.CurrentState == PlayerController.PLAYER_STATE_ACTIVE && gameController.isCurrentPlayerAttack())
                {
                    if(pc.IsMoving ==false)
                    {
                        pc.SetPlayerMoveTo(gameController.GetGoalPosOfEnemy(true));
                    }
                }
            }
        }
    }
    private void GeneratePlayer(Vector3 pos)
    {
        pos.y = gameController.GetLandPosY();
        int i = 0;
        bool isAttack = gameController.isCurrentPlayerAttack();
        GameObject go;
        if(isAttack)
        {
            for(;i<arrAttackerPlayer.Count;i++)
            {
                go = arrAttackerPlayer[i];
                if(go.activeSelf == false)
                {
                    go.SetActive(true);
                    go.GetComponent<PlayerController>().InitPlayer(pos,false);
                    return;
                }
            }
            go = Instantiate(gameController.AttackerPlayerPrefab);
            go.GetComponent<PlayerController>().InitPlayer(pos,false);
            go.transform.parent = transform;
            arrAttackerPlayer.Add(go);
        }
        else
        {
            for(;i<arrDefenderPlayer.Count;i++)
            {
                go = arrDefenderPlayer[i];
                if(go.activeSelf == false)
                {
                    go.SetActive(true);
                    go.GetComponent<PlayerController>().InitPlayer(pos,false);
                    return;
                }
            }
            go = Instantiate(gameController.DefenderPlayerPrefab);
            go.GetComponent<PlayerController>().InitPlayer(pos,false);
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
}
