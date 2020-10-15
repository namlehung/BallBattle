using System.Collections;
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

    private GameObject targetPlayer;
    public int defenderStatus;
    public const int DEFENDER_NONE = 0;
    public const int DEFENDER_MOVE_TARGET = 1;
    public const int DEFENDER_MOVE_BACK = 2;
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
                    defenderStatus = DEFENDER_NONE;
                    targetPlayer = null;
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
                    //defenderStatus = DEFENDER_NONE;
                    targetPlayer = null;
                    playerController.IsInitEachState = true;
                    playerController.SetInActiveMaterial();
                    StartCoroutine(WaitFromIdletoActive());
                }
            }
            break;
        }
    }

    public void SetTarget(GameObject target)
    {
        targetPlayer = target;
        playerController.IsMoving = true;
        defenderStatus = DEFENDER_MOVE_TARGET;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("--------OnCollisionEnter----------- : " + collision.gameObject.name);
        if(targetPlayer != null && targetPlayer == collision.gameObject)
        {
            Debug.Log("--------catch ball----------- : ");
            //targetPlayer.GetComponent<PlayerController>().animator.SetTrigger("IsImpact");
            targetPlayer.GetComponent<AttackerPlayer>().PassBall();
            targetPlayer = null;
            //playerController.animator.SetTrigger("IsImpact");
            defenderStatus = DEFENDER_MOVE_BACK;
            playerController.switchState(PlayerController.PLAYER_STATE_INACTIVE);
        }
    }
    public bool IsHasTarget()
    {
        return (targetPlayer != null);
    }
    void FixedUpdate()
    {
        if(playerController.IsMoving)
        {
            //Debug.Log("player moving");
            if(targetPlayer != null && defenderStatus == DEFENDER_MOVE_TARGET)
            {
                if(playerController.animator.GetBool("IsRunning") == false)
                {
                    playerController.animator.SetBool("IsRunning",true);
                }
                transform.LookAt(targetPlayer.transform.position);
                playerController.movePos = Vector3.Normalize(targetPlayer.transform.position - transform.position);
                transform.position = transform.position + playerController.movePos*Time.fixedDeltaTime*normalSpeed;
            }
            else if(defenderStatus == DEFENDER_MOVE_BACK)
            {
                transform.LookAt(playerController.initPos);
                playerController.movePos = Vector3.Normalize(playerController.initPos - transform.position);
                transform.position = transform.position + playerController.movePos*Time.fixedDeltaTime*returnSpeed;
                if(Vector3.Distance(transform.position, playerController.initPos) < 0.1f)
                {
                    defenderStatus = DEFENDER_NONE;
                    transform.eulerAngles = playerController.initRotateAngle;
                    playerController.animator.SetBool("IsRunning",false);
                    playerController.IsMoving = false;
                }
            }
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
