﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderPlayer : MonoBehaviour
{
    public float energyCost = 3.0f;
    public float normalSpeed = 1.0f;
    public float returnSpeed = 2.0f;
    public float spawnTime = 0.5f;
    public float rangeDefender = 0;
    public float reactivateTime = 4.0f;

    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = transform.GetComponent<PlayerController>();
        rangeDefender = GameController.gameControllerInstance.GetLandSize().x * 0.35f;
    }

    // Update is called once per frame
    void Update()
    {
        switch(playerController.CurrentState)
        {
            case PlayerController.PLAYER_STATE_SPAWN:
            {
                if(playerController.IsInitEachState == false)
                {
                    playerController.IsInitEachState = true;
                    StartCoroutine(WaitFromSpawntoActive());
                }
            }
            break;
            case PlayerController.PLAYER_STATE_ACTIVE:
            {
                if(playerController.IsInitEachState == false)
                {
                    playerController.IsInitEachState = true;
                }
            }
            break;
            case PlayerController.PLAYER_STATE_INACTIVE:
            {
                if(playerController.IsInitEachState == false)
                {
                    playerController.IsInitEachState = true;
                    playerController.SetInActiveMaterial();
                    StartCoroutine(WaitFromIdletoActive());
                }
            }
            break;
        }
    }

    IEnumerator WaitFromSpawntoActive()
    {
        yield return new WaitForSeconds(spawnTime);

        playerController.switchState(PlayerController.PLAYER_STATE_ACTIVE);
    }
    IEnumerator WaitFromIdletoActive()
    {
        yield return new WaitForSeconds(reactivateTime);

        playerController.switchState(PlayerController.PLAYER_STATE_ACTIVE);
    }
}
