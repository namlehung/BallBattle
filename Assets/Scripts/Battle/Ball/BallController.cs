using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    
    public float ballSpeed = 1.5f;
    private bool isMoving =false;
    private bool isNeedRotate = false;
    private Vector3 targetPos;
    
    // Start is called before the first frame update
    void Start()
    {
        //isMoving = false;
        //isNeedRotate = false;
    }   

    public void setTargetPos(Vector3 pos)
    {
        targetPos = pos;
        isMoving = true;
    }

    public void SetRotate(bool value)
    {
        isNeedRotate = value;
    }
    public void SetMoving(bool value)
    {
        isMoving = value;
    }
    // Update is called once per frame
    void Update()
    {
        if(isMoving || isNeedRotate)
        {
            transform.Rotate(Vector3.right,Time.fixedTime*ballSpeed);
        }
    }
    void FixedUpdate()
    {
        if(isMoving)
        {
            Vector3 movepos = Vector3.Normalize(targetPos - transform.position);
            float gameScale = GameController.gameControllerInstance.transform.localScale.x;
            transform.position = transform.position + movepos*ballSpeed*Time.fixedDeltaTime*gameScale;
        }
    }
}
