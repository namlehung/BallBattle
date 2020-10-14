using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnergyController enemyEnergy;
    private List<GameObject> arrAttackerPlayer;
    private List<GameObject> arrDefenderPlayer;
    
    private GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        enemyEnergy = GameObject.Find("Canvas/Game_UI/EnemyInfo/FillBar").GetComponent<EnergyController>();
        arrAttackerPlayer = new List<GameObject>();
        arrDefenderPlayer = new List<GameObject>();
        gameController = transform.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.IsGamePause())
            return;
        AIGenerate();
    }

    private void AIGenerate()
    {
        if(enemyEnergy.SpentEnergy(gameController.GetEnemyEnergyToSpawn()))
        {
            Rect zone = GameController.GetEnemyZone();
            Vector3 posSpawn = Vector3.zero;
            posSpawn.x = Random.Range(zone.x,zone.x + zone.width);
            posSpawn.z = Random.Range(zone.y,zone.y + zone.height);
            //bool isAttack = !gameController.isCurrentPlayerAttack();
            GeneratePlayer(posSpawn);
        }
    }
    private void GeneratePlayer(Vector3 pos)
    {
        pos.y = gameController.GetLandPosY();
        int i = 0;
        bool isAttack = !gameController.isCurrentPlayerAttack();
        GameObject go;
        if(isAttack)
        {
            for(;i<arrAttackerPlayer.Count;i++)
            {
                go = arrAttackerPlayer[i];
                if(go.activeSelf == false)
                {
                    go.SetActive(true);
                    go.GetComponent<PlayerController>().InitPlayer(pos,true);
                    return;
                }
            }
            go = Instantiate(gameController.AttackerPlayerPrefab);
            go.GetComponent<PlayerController>().InitPlayer(pos,true);
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
                    go.GetComponent<PlayerController>().InitPlayer(pos,true);
                    return;
                }
            }
            go = Instantiate(gameController.DefenderPlayerPrefab);
            go.GetComponent<PlayerController>().InitPlayer(pos,true);
            go.transform.parent = transform;
            arrDefenderPlayer.Add(go);
        }
    }
}
