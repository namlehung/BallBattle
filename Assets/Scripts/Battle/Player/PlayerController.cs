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
    public Vector3 targetPos;
    public Vector3 movePos;
    public Animator animator;

    private bool hasCarryBall;
    // Start is called before the first frame update
    void Start()
    {
        animator = GameController.FindChildByName(transform,"playermodel").GetComponent<Animator>();
    }

    public void InitPlayer(Vector3 pos,bool IsEnemy)
    {
        hasCarryBall = false;
        isMoving = false;
        transform.position = pos;
        isEnemy = IsEnemy;
        isInitEachState = false;
        currentState = PLAYER_STATE_SPAWN;
        Debug.Log("Generate/Active is enemy: " + isEnemy + " pos: " + pos );
        GameController gameController = GameObject.Find("Battle").GetComponent<GameController>();
        transform.gameObject.layer = gameController.layerMaskPlayerActive;
        if(isEnemy)
        {
            SetPlayerMaterial(gameController.RedEnemyMaterial);
            transform.eulerAngles = new Vector3(0,180,0);
        }
        else
        {
            SetPlayerMaterial(gameController.BluePlayerMaterial);
            transform.eulerAngles = Vector3.zero;
        }
    }

    private void SetPlayerMaterial(Material mat)
    {
        GameObject model = GameController.FindChildByName(transform,"playermodel");
        GameObject shirt = GameController.FindChildByName(model.transform,"Ch38_Shirt");
        SkinnedMeshRenderer smr = shirt.GetComponent<SkinnedMeshRenderer>();
        smr.materials[0] = mat;
        Debug.Log("why canot change mat");
    }

    public void SetInActiveMaterial()
    {
        GameController gameController = transform.GetComponentInParent<GameController>();
        SetPlayerMaterial(gameController.GrayPlayerMaterial);
    }
    
    public void StopPlayerMove()
    {
        animator.SetBool("IsRunning",false);
        isMoving = false;
        GameController gameController = transform.GetComponentInParent<GameController>();
        transform.gameObject.layer = gameController.layerMaskPlayerInActive;
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
}
