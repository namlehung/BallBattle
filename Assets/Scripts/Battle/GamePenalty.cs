using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePenalty : MonoBehaviour
{
    public bool isManualMove = true;
    [HideInInspector]  public Vector2 MOVE_LEFT = new Vector2(0,-1);
    [HideInInspector] public Vector2 MOVE_RIGHT = new Vector2(0,1);
    [HideInInspector]  public Vector2 MOVE_UP = new Vector2(-1,0);
    [HideInInspector] public Vector2 MOVE_DOWN = new Vector2(1,0);

    private PlayerController penaltyPlayer;
    private MazeGenerator mazeGenerator;

    private CellInfo[] cellsInfo;
    private List<CellInfo> listPath = new List<CellInfo>();
    private int indexPath = 0;
    private CellInfo[] arrPathToCheck;
    private int indexToCheck = 0;
    private bool hasMoveOverTarget = false;

    private const int PENALTY_STATE_NONE = 0;
    private const int PENALTY_STATE_HAS_BALL = 1;
    private const int PENALTY_STATE_DONE = 2;


    private int currentState = PENALTY_STATE_NONE;
    private CellInfo currentCell;

   
    // Start is called before the first frame update
    void Start()
    {
        mazeGenerator = transform.GetComponent<MazeGenerator>();

        arrPathToCheck = new CellInfo[mazeGenerator.Rows*mazeGenerator.Columns];
        penaltyPlayer = null;
        cellsInfo =  new CellInfo[mazeGenerator.Rows*mazeGenerator.Columns];
    }

    public void StartPenaltyGame(GameObject player)
    {
        penaltyPlayer = player.GetComponent<PlayerController>();
        currentCell = mazeGenerator.GetCellInfo(mazeGenerator.Rows-1,mazeGenerator.Columns/2);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isManualMove)
        {
            UpdateAutoMove();
        }
    }

    public void UpdateManualMove(Vector3 pos)
    {
        if(penaltyPlayer == null || !isManualMove)
        {
            return;
        }
        if(penaltyPlayer.HasCarryBall == false && penaltyPlayer.IsGoToGetBall == false)
        {
            penaltyPlayer.IsGoToGetBall = true;
        }
        penaltyPlayer.SetPlayerMoveTo(pos,false);
        /*float deltax = pos.x - penaltyPlayer.gameObject.transform.position.x;
        float deltaz = pos.z - penaltyPlayer.gameObject.transform.position.z;
        int index = -1;
        if(Mathf.Abs(deltax) > Mathf.Abs(deltaz))
        {
            if(deltax > 0)
            {
                if(IsPlayerCanMove(MOVE_RIGHT,out index))
                {
                    if(penaltyPlayer.HasCarryBall == false && penaltyPlayer.IsGoToGetBall == false)
                    {
                        penaltyPlayer.IsGoToGetBall = true;
                    }
                    MovePlayer(index);
                }
            }
            else
            {
                if(IsPlayerCanMove(MOVE_LEFT,out index))
                {
                    if(penaltyPlayer.HasCarryBall == false && penaltyPlayer.IsGoToGetBall == false)
                    {
                        penaltyPlayer.IsGoToGetBall = true;
                    }
                    MovePlayer(index);
                }
            }
        }
        else
        {
            if(deltaz > 0)
            {
                if(IsPlayerCanMove(MOVE_UP,out index))
                {
                    if(penaltyPlayer.HasCarryBall == false && penaltyPlayer.IsGoToGetBall == false)
                    {
                        penaltyPlayer.IsGoToGetBall = true;
                    }
                    MovePlayer(index);
                }
            }
            else
            {
                if(IsPlayerCanMove(MOVE_DOWN,out index))
                {
                    if(penaltyPlayer.HasCarryBall == false && penaltyPlayer.IsGoToGetBall == false)
                    {
                        penaltyPlayer.IsGoToGetBall = true;
                    }
                    MovePlayer(index);
                }
            }
        }*/
    }
    private void UpdateAutoMove()
    {
        if(penaltyPlayer == null)
        {
            return;
        }
        switch(currentState)
        {
            case PENALTY_STATE_NONE:
            {
                if(penaltyPlayer.HasCarryBall == false)
                {
                    currentState = PENALTY_STATE_HAS_BALL;
                }
                else if(penaltyPlayer.IsMoving == false)
                {
                    bool HasPath = false;
                    HasPath |= CheckAndMovePlayerToBall(MOVE_LEFT);
                    HasPath |= CheckAndMovePlayerToBall(MOVE_RIGHT);
                    HasPath |= CheckAndMovePlayerToBall(MOVE_UP);
                    HasPath |= CheckAndMovePlayerToBall(MOVE_DOWN);
                    if(HasPath)
                    {

                    }
                }
            }
            break;
            case PENALTY_STATE_HAS_BALL:
            {

            }
            break;
            case PENALTY_STATE_DONE:
            {

            }
            break;
        }
        
    }

    private bool CheckAndMovePlayerToBall(Vector2 direction)
    {
        int index;
        if(IsPlayerCanMove(direction,out index))
        {
           if(penaltyPlayer.IsMoving)
            {
                arrPathToCheck[indexToCheck++] = cellsInfo[index];
            }
            else
            {
                listPath.Add(cellsInfo[index]);
                if(penaltyPlayer.IsGoToGetBall == false)
                {
                    penaltyPlayer.IsGoToGetBall = true;
                }
                MovePlayer(index);
            }
            return true;
        }
        return false;
    }

    private void MovePlayer(int index)
    {
        Vector3 pos = mazeGenerator.GetPosAt(cellsInfo[index].irow,cellsInfo[index].icol);
        penaltyPlayer.SetPlayerMoveTo(pos,false);
    }
    private bool IsPlayerCanMove(Vector2 direct,out int index)
    {
        int nextrow = (int)(currentCell.irow + direct.x);
        int newcol = (int)(currentCell.icol + direct.y);
        index = mazeGenerator.GetIndex(nextrow,newcol);
        if(index == -1)
        {
            return false;
        }
        CellInfo nextCell = cellsInfo[index];
        if(nextCell == null)
        {
            nextCell = mazeGenerator.GetCellInfo(nextrow,newcol);
        }

        if(nextCell.NumCanMove() > 1)
        {
            return true;
        }
        return false;
    }
}

public class CellInfo
{
    public int irow;
    public int icol;
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public CellInfo()
    {
        irow = 0;
        icol = 0;
        up = false;
        down = false;
        left = false;
        right = false;
    }
    public int NumCanMove()
    {
        int moveable = 0;
        if(up )
        {
            moveable ++;
        }
        if(down)
        {
            moveable ++;
        }
         if(left)
        {
            moveable ++;
        }
        if(right)
        {
            moveable ++;
        }
        return moveable;
    }
}
