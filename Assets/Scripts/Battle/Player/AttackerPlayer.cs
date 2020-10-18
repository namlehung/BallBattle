using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerPlayer : MonoBehaviour
{
    public float energyCost = 2.0f;
    public float normalSpeed = 1.5f;
    public float carrySpeed = 0.75f;
    public float spawnTime = 0.5f;
    public float reactivateTime = 2.5f;

    private PlayerController playerController;

    private GameObject goIndicator;
    // Start is called before the first frame update
    void Start()
    {
        playerController = transform.GetComponent<PlayerController>();
        goIndicator = GameController.gameControllerInstance.FindChildByName(transform,"PlayerIndicator");
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
                    playerController.effectSpawn.Play();
                    StartCoroutine(WaitFromSpawntoActive());
                }
            }
            break;
            case PlayerController.PLAYER_STATE_ACTIVE:
            {
                if(playerController.IsInitEachState == false)
                {
                    playerController.IsInitEachState = true;
                    goIndicator.SetActive(true);
                }
                if(playerController.HasCarryBall == false && playerController.IsGoToGetBall == false)
                {
                    if(goIndicator.activeSelf == false)
                    {
                        goIndicator.SetActive(true);
                    }
                }
                else
                {
                    if(goIndicator.activeSelf)
                    {
                        goIndicator.SetActive(false);
                    }
                }
                if(playerController.HasCarryBall)
                {
                    if(playerController.effectHasBall.gameObject.activeSelf == false)
                    {
                        playerController.effectHasBall.gameObject.SetActive(true);
                        playerController.effectHasBall.Play();
                    }
                }
                else
                {
                    if(playerController.effectHasBall.gameObject.activeSelf)
                    {
                        playerController.effectHasBall.Pause();
                        playerController.effectHasBall.gameObject.SetActive(false);
                    }
                }
            }
            break;
            case PlayerController.PLAYER_STATE_INACTIVE:
            {
                if(playerController.IsInitEachState == false)
                {
                    playerController.IsInitEachState = true;
                    playerController.SetInActiveMaterial();
                    if(playerController.effectHasBall.gameObject.activeSelf)
                    {
                        playerController.effectHasBall.Pause();
                        playerController.effectHasBall.gameObject.SetActive(false);
                    }
                    StartCoroutine(WaitFromIdletoActive());
                }
            }
            break;
        }
    }

    void FixedUpdate()
    {
        if(playerController.CurrentState == PlayerController.PLAYER_STATE_ACTIVE)
        {
            if(playerController.HasCarryBall || playerController.IsGoToGetBall)
            {
                if(transform.gameObject.layer !=  LayerMask.NameToLayer("player"))
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("player");
                }
            }
        }
        if(playerController.IsMoving)
        {
            //Debug.Log("player moving");
            if(playerController.animator.GetBool("IsRunning") == false)
            {
                playerController.animator.SetBool("IsRunning",true);
            }
            transform.LookAt(playerController.targetPos);
            float gameScale = GameController.gameControllerInstance.transform.localScale.x;
            if(playerController.HasCarryBall)
            {
                transform.position = transform.position + playerController.movePos*Time.fixedDeltaTime*carrySpeed*gameScale;
            }
            else
            {
                transform.position = transform.position + playerController.movePos*Time.fixedDeltaTime*normalSpeed*gameScale;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        string targetname = other.transform.parent.name;
        if(targetname.Equals("EnemyGoal") || targetname.Equals("PlayerGoal"))
        {
            if(other.gameObject.name.Equals("mid") && playerController.HasCarryBall)
            {
                Debug.Log("Player hit the Goal");
                if(playerController.HasCarryBall)
                {
                    GameController.gameControllerInstance.AttackerScoreGoal();
                }
            }
            else
            {
                Debug.Log("Player hit the Fence");
                HitTheFence();
            }
        }
        else if(other.gameObject.CompareTag("battle_ball"))
        {
            Debug.Log("Player hit the ball");
            TakeBall();
        }
    }

    private void TakeBall()
    {
        GameController.gameControllerInstance.PlayerTakeBall();
        GameObject playerball = GameController.gameControllerInstance.FindChildByName(transform,"ball");
        playerball.SetActive(true);
        playerball.GetComponent<BallController>().SetRotate(true);
        playerController.IsGoToGetBall = false;
        Debug.Log("-----------------player take ball ------------");
        playerController.HasCarryBall = true;
        playerController.IsMoving = false;
    }

    public void PassBall()
    {
        GameObject playerball = GameController.gameControllerInstance.FindChildByName(transform,"ball");
        // if(playerController.animator.GetBool("IsRunning"))
        // {
        //     playerController.animator.SetBool("IsRunning",false);
        // }
        playerball.SetActive(false);
        playerController.switchState(PlayerController.PLAYER_STATE_INACTIVE);
        playerController.IsGoToGetBall = false;
        playerController.HasCarryBall = false;
        playerController.IsMoving = false;
        GameController.gameControllerInstance.PlayerPassBall(transform.position);
    }
    void HitTheFence()
    {
        playerController.switchState(PlayerController.PLAYER_STATE_IDLE);
        playerController.StopPlayerMove();
        playerController.animator.SetTrigger("IsDeath");
        StartCoroutine(WaitForAnimEnd());
    }

    IEnumerator WaitForAnimEnd()
    {
        yield return new WaitForSeconds(1.0f);
        transform.gameObject.SetActive(false);
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
