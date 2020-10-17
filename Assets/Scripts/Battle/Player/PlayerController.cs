using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public const int PLAYER_STATE_IDLE = -1;
    public const int PLAYER_STATE_SPAWN = 0;
    public const int PLAYER_STATE_ACTIVE = 1;
    public const int PLAYER_STATE_INACTIVE = 2;

    private bool isEnemy;
    private int currentState;
    private bool isInitEachState;

    private bool isMoving;
    [HideInInspector]
    public Vector3 targetPos;
     [HideInInspector] public Vector3 movePos;

     [HideInInspector] public Vector3 initPos;
     [HideInInspector] public Vector3 initRotateAngle;
     [HideInInspector] public Animator animator;

    private bool hasCarryBall;
    private bool isGoToGetBall;
    // Start is called before the first frame update
    void Start()
    {
        animator = GameController.gameControllerInstance.FindChildByName(transform,"playermodel").GetComponent<Animator>();
    }

    public void InitPlayer(Vector3 pos,bool IsEnemy)
    {
        isGoToGetBall = false;
        hasCarryBall = false;
        isMoving = false;
        transform.position = pos;
       
        isEnemy = IsEnemy;
        switchState(PLAYER_STATE_SPAWN);
        Debug.Log("Generate/Active is enemy: " + isEnemy + " pos: " + pos );
        if(isEnemy)
        {
            SetPlayerMaterial(GameController.gameControllerInstance.RedEnemyMaterial);
            transform.eulerAngles = new Vector3(0,180,0);
        }
        else
        {
            SetPlayerMaterial(GameController.gameControllerInstance.BluePlayerMaterial);
            transform.eulerAngles = Vector3.zero;
        }
        GameObject playerball = GameController.gameControllerInstance.FindChildByName(transform,"ball");
        if(playerball)
        {
            playerball.SetActive(false);
        }
        //Vector3 localscale = transform.localScale;
        //transform.localScale = new Vector3(localscale.x*transform.parent.localScale.x,localscale.y*transform.parent.localScale.y,localscale.z*transform.parent.localScale.z);
        initPos = pos;
        initRotateAngle = transform.eulerAngles;
    }

    private void SetPlayerMaterial(Material mat)
    {
        GameObject model = GameController.gameControllerInstance.FindChildByName(transform,"playermodel");
        GameObject shirt = GameController.gameControllerInstance.FindChildByName(model.transform,"Ch38_Shirt");
        SkinnedMeshRenderer smr = shirt.GetComponent<SkinnedMeshRenderer>();
        smr.materials[0].color = mat.color;
        //Debug.Log("why canot change mat");
    }

    public void SetInActiveMaterial()
    {
        SetPlayerMaterial(GameController.gameControllerInstance.GrayPlayerMaterial);
    }
    
    public void StopPlayerMove()
    {
        animator.SetBool("IsRunning",false);
        isMoving = false;
        Rigidbody rigid = transform.GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

    }
    public void SetPlayerMoveTo(Vector3 pos,bool isGoStraight = true)
    {
        isMoving = true;
        targetPos = new Vector3(pos.x,transform.position.y,pos.z);
        if(isGoStraight)
        {
            targetPos.x = transform.position.x;
        }
        movePos = Vector3.Normalize(targetPos - transform.position);
    }    
    public int CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public bool IsInitEachState
    {
        get { return isInitEachState; }
        set { isInitEachState = value; }
    }

    public bool IsMoving
    {
        get { return isMoving; }
        set { isMoving = value; }
    }

    public bool HasCarryBall
    {
        get { return hasCarryBall; }
        set { hasCarryBall = value; }
    }
    public bool IsGoToGetBall
    {
        get { return isGoToGetBall; }
        set { isGoToGetBall = value; }
    }

    public void switchState (int state)
    {
        currentState = state;
        isInitEachState = false;
        if(currentState == PLAYER_STATE_ACTIVE)
        {
            if(isEnemy)
            {
                SetPlayerMaterial(GameController.gameControllerInstance.RedEnemyMaterial);
            }
            else
            {
                SetPlayerMaterial(GameController.gameControllerInstance.BluePlayerMaterial);
            }
            //transform.gameObject.layer = LayerMask.NameToLayer("player");//GameController.gameControllerInstance.layerMaskPlayerActive;
        }
        else
        {
            transform.gameObject.layer =  LayerMask.NameToLayer("playerInActive");//GameController.gameControllerInstance.layerMaskPlayerInActive;
        }
    }
}
