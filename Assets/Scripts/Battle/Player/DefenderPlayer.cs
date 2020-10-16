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
     [HideInInspector] public int defenderStatus;
    public const int DEFENDER_NONE = 0;
    public const int DEFENDER_MOVE_TARGET = 1;
    public const int DEFENDER_MOVE_BACK = 2;
    public const int DEFENDER_IMPACT = 3;
    public const int DEFENDER_WAITING_ANIM_IMPACT_END = 4;

    private GameObject goRangeDefender;
    // Start is called before the first frame update
    void Start()
    {
        playerController = transform.GetComponent<PlayerController>();
        rangeDefender = GameController.gameControllerInstance.GetLandSize().x * 0.35f;
        goRangeDefender = GameController.gameControllerInstance.FindChildByName(transform,"RangeDefender");
        goRangeDefender.transform.localScale = new Vector3(rangeDefender,0.01f,rangeDefender); 
        goRangeDefender.SetActive(false);
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
                if(targetPlayer == null)
                {
                    if(goRangeDefender.activeSelf == false)
                    {
                        goRangeDefender.SetActive(true);
                    }
                }
                else
                {
                    if(goRangeDefender.activeSelf)
                    {
                        goRangeDefender.SetActive(false);
                    }
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
            targetPlayer.GetComponent<PlayerController>().animator.SetBool("IsRunning",false);
            targetPlayer.GetComponent<PlayerController>().animator.SetTrigger("IsImpact");
            targetPlayer.GetComponent<AttackerPlayer>().PassBall();
            targetPlayer = null;
            playerController.animator.SetBool("IsRunning",false);
            playerController.animator.SetTrigger("IsImpact");
            //defenderStatus = DEFENDER_MOVE_BACK;
            defenderStatus = DEFENDER_IMPACT;
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
                if(playerController.animator.GetBool("IsRunning") == false)
                {
                    playerController.animator.SetBool("IsRunning",true);
                }
                transform.LookAt(playerController.initPos);
                playerController.movePos = Vector3.Normalize(playerController.initPos - transform.position);
                transform.position = transform.position + playerController.movePos*Time.fixedDeltaTime*returnSpeed;
                if(Vector3.Distance(transform.position, playerController.initPos) < 0.05f)
                {
                    defenderStatus = DEFENDER_NONE;
                    transform.eulerAngles = playerController.initRotateAngle;
                    playerController.animator.SetBool("IsRunning",false);
                    playerController.IsMoving = false;
                }
            }
            else if(defenderStatus == DEFENDER_IMPACT)
            {
                defenderStatus = DEFENDER_WAITING_ANIM_IMPACT_END;
                StartCoroutine(WaitforAnimImpactEnd());
            }
        }
    }
    IEnumerator WaitforAnimImpactEnd()
    {
        yield return new WaitForSeconds(0.75f);
        defenderStatus = DEFENDER_MOVE_BACK;
    }
    IEnumerator WaitFromSpawntoActive()
    {
        yield return new WaitForSeconds(spawnTime);

        transform.gameObject.layer = LayerMask.NameToLayer("player");
        playerController.switchState(PlayerController.PLAYER_STATE_ACTIVE);
    }
    IEnumerator WaitFromIdletoActive()
    {
        yield return new WaitForSeconds(reactivateTime);

        transform.gameObject.layer = LayerMask.NameToLayer("player");
        playerController.switchState(PlayerController.PLAYER_STATE_ACTIVE);
    }
}
