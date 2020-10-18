using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//source youtube: https://www.youtube.com/watch?v=wLuAFDdzbek&ab_channel=Lekabros
//souce git: https://www.youtube.com/redirect?v=wLuAFDdzbek&event=video_description&q=https%3A%2F%2Fgithub.com%2FBigThinker%2FHexagonia%2Fblob%2Fmaster%2FServer%2FCameraMovement.cs&redir_token=QUFFLUhqbWQ3WGZRQjhld0pHWk9qcE1MMUh1bXNCc1VfUXxBQ3Jtc0ttZHZZSE9WWWgxNUc2UHhiSExKM1pvX04wN215TUxiN3oydUNiNkp1aV9pQjFlVExvdVBOZUNCenliVHREelBnemlkNUVBZHRVNzZDRlE4ZEY2bHkxemZiOUQ0ZGV4Q3hmTl9NQmctNWcwaVFkWTNTNA%3D%3D
public class MazeGenerator : MonoBehaviour
{
    [HideInInspector] public int Rows = 10;
    [HideInInspector] public int Columns = 5;
    public GameObject Wall;
    private MazeCell[,] grid;
    private int currentRow;
    private int currentColumn;
    private bool scanComplete;

    private GameObject goTheMaze;
	void Start ()
    {
        goTheMaze = new GameObject();
        goTheMaze.transform.parent = transform;
        goTheMaze.name = "TheMaze";
        //GenerateGrid();
	}

    public Vector3 GetPlayerPenaltyPos()
    {
        Vector2 yardsize = GameController.gameControllerInstance.GetLandSize();
        float size = yardsize.x / Columns;
        float startX = -(Columns/2)*size;
        float startZ = (Rows/2)*size ;
        if(Columns%2 == 0)
        {
            startX += size/2;
        }
        if(Rows%2 == 0)
        {
            startZ -= size/2;
        }
        int i = Columns/2;
        int j = Rows-1;
        return new Vector3(startX + i*size,0,startZ - j*size);
    }
    public Vector3 GeneratePosBall()
    {
        Vector2 yardsize = GameController.gameControllerInstance.GetLandSize();
        float size = yardsize.x / Columns;
        float startX = -(Columns/2)*size;
        float startZ = (Rows/2)*size ;
        if(Columns%2 == 0)
        {
            startX += size/2;
        }
        if(Rows%2 == 0)
        {
            startZ -= size/2;
        }
        int i = Random.Range(0,100)%Columns;
        int j = Random.Range(0,100)%Rows;
        return new Vector3(startX + i*size,0,startZ - j*size);
    }
    public void GenerateGrid()
    {
        // destroy all the children of this goTheMaze object.
        foreach (Transform transform in goTheMaze.transform)
        {
            Destroy(transform.gameObject);
        }

        // first, we create the grid with all the walls and floors.
        CreateGrid();

        // then, we fix the camera position so it's centered.
        //ChangeCameraPosition();

        // reset the algorithm variables.
        currentRow = 0;
        currentColumn = 0;
        scanComplete = false;

        // then we run the algorithm to carve the paths.
        HuntAndKill();

        RemoveWalls();
    }

    void RemoveWalls()//the wall has same postion with the wall of the yard
    {
       foreach(Transform childtf in goTheMaze.transform)
       {
           string name = childtf.gameObject.name;
            if(name.IndexOf("r0")>0)
            {
                RemoveWall(childtf.gameObject,0);
            }
            if(name.IndexOf("c0")>0)
            {
                RemoveWall(childtf.gameObject,2);
            }
            if(name.IndexOf("c" +(Columns-1))>0)
            {
                RemoveWall(childtf.gameObject,3);
            }
            if(name.IndexOf("r"+(Rows-1))>0)
            {
                RemoveWall(childtf.gameObject,1);
            }
       }
    }

    void RemoveWall(GameObject cell,int direction)// 0: up, 1: down, 2: left , 3: right
    {
        string nameremove = "UpWall";
        if(direction == 1)
        {
            nameremove = "DownWall";
        }
        else if(direction == 2)
        {
            nameremove = "LeftWall";
        }
        else if(direction == 3)
        {
            nameremove = "RightWall";
        }
        GameObject goremove = GameController.gameControllerInstance.FindChildByName(cell.transform,nameremove);
        if(goremove!= null)
        {
            Destroy(goremove);
        }
    }

    void CreateGrid()
    {
        Vector2 yardsize = GameController.gameControllerInstance.GetLandSize();
        if(Columns*2 != Rows)
        {
            Rows = Columns*2;
        }
        float size = yardsize.x / Columns;//Wall.transform.localScale.x;
        float startX = -(Columns/2)*size;
        float startZ = (Rows/2)*size ;
        if(Columns%2 == 0)
        {
            startX += size/2;
        }
        if(Rows%2 == 0)
        {
            startZ -= size/2;
        }
        float cellPosX = 0, cellPosY = 0,cellPosZ = startZ;

        grid = new MazeCell[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            cellPosX = startX;
            for (int j = 0; j < Columns; j++)
            {
                GameObject goCell = new GameObject();
                goCell.name = "Cell_r"+i+"_c"+j;
                goCell.transform.position = new Vector3(cellPosX,cellPosY,cellPosZ);
                goCell.transform.parent = goTheMaze.transform;
                

                GameObject upWall = Instantiate(Wall);
                upWall.name = "UpWall";
                upWall.transform.parent = goCell.transform;
                
                Vector3 localsale = upWall.transform.localScale;
                localsale.x *= size;
                localsale.y *= size;
                upWall.transform.localScale = localsale;
                upWall.transform.localPosition = new Vector3(0,0,size/2);
                upWall.transform.rotation = Quaternion.identity;
               
                GameObject downWall = Instantiate(Wall);
                downWall.name = "DownWall";
                downWall.transform.parent = goCell.transform;
                downWall.transform.localScale = localsale;
                downWall.transform.localPosition = new Vector3(0,0,-size/2);
                downWall.transform.rotation = Quaternion.identity;

                GameObject leftWall = Instantiate(Wall);
                leftWall.name = "LeftWall";
                leftWall.transform.parent = goCell.transform;
                leftWall.transform.localScale = localsale;
                leftWall.transform.localPosition = new Vector3(-size/2,0,0);
                leftWall.transform.rotation = Quaternion.Euler(0, 90, 0);

                GameObject rightWall = Instantiate(Wall);
                rightWall.name = "RightWall";
                rightWall.transform.parent = goCell.transform;
                rightWall.transform.localScale = localsale;
                rightWall.transform.localPosition = new Vector3(size/2,0,0);
                rightWall.transform.rotation = Quaternion.Euler(0, 90, 0);

                // create the maze cell and add references to its walls.
                grid[i, j] = new MazeCell();
                grid[i, j].UpWall = upWall;
                grid[i, j].DownWall = downWall;
                grid[i, j].LeftWall = leftWall;
                grid[i, j].RightWall = rightWall;

                // create an exit door.
                if (i == 0 && j == Columns/2)// 0)
                {
                   Destroy(upWall);
                    //Destroy(leftWall);
                }

                // create an entrance door.
                if (i == Rows - 1 && j == Columns/2)// - 1)
                {
                    Destroy(downWall);
                    //Destroy(rightWall);
                }
                cellPosX += size;
            }
            cellPosZ -= size;
        }
    }

	void HuntAndKill()
    {
        // mark the first cell of the random walk as visited.
        grid[currentRow, currentColumn].Visited = true;

        while (!scanComplete)
        {
            Walk();
            Hunt();
        }
    }

    void Walk()
    {
        while (AreThereUnvisitedNeighbors())
        {
            // then go to a random direction.
            int direction = Random.Range(0, 4);

            // check up.
            if (direction == 0)
            {
                // make sure the above cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow - 1, currentColumn))
                {
                    // Debug.Log("Went up.");

                    // destroy the up wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                        //grid[currentRow, currentColumn].UpWall.SetActive(false);
                    }

                    currentRow--;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the down wall of the cell above if there's any.
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                        //grid[currentRow, currentColumn].DownWall.SetActive(false);
                    }
                }
            }
            // check down.
            else if (direction == 1)
            {
                // make sure the below cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn))
                {
                    // Debug.Log("Went down.");

                    // destroy the down wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                        //grid[currentRow, currentColumn].DownWall.SetActive(false);
                    }

                    currentRow++;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the up wall of the cell below if there's any.
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                        //grid[currentRow, currentColumn].UpWall.SetActive(false);
                    }
                }
            }
            // check left.
            else if (direction == 2)
            {
                // make sure the left cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1))
                {
                    // Debug.Log("Went left.");

                    // destroy the left wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                        //grid[currentRow, currentColumn].LeftWall.SetActive(false);
                    }

                    currentColumn--;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the right wall of the cell at the left if there's any.
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                        //grid[currentRow, currentColumn].RightWall.SetActive(false);
                    }
                }
            }
            // check right.
            else if (direction == 3)
            {
                // make sure the right cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1))
                {
                    // Debug.Log("Went right.");

                    // destroy the right wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                        //grid[currentRow, currentColumn].RightWall.SetActive(false);
                    }

                    currentColumn++;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the left wall of the cell at the right if there's any.
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                        //grid[currentRow, currentColumn].LeftWall.SetActive(false);
                    }
                }
            }
        }
    }

    // after random walk is complete, we run Hunt.
    void Hunt()
    {
        // assume the scan is complete.
        scanComplete = true;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                // if the condition is satisfied that a cell is unvisited and it has a visited neighbour, do another random walk from new cell.
                if (!grid[i, j].Visited && AreThereVisitedNeighbors(i, j))
                {
                    // scan is not actually complete.
                    scanComplete = false;
                    // set the new current row and column.
                    currentRow = i;
                    currentColumn = j;
                    // mark it as visited.
                    grid[currentRow, currentColumn].Visited = true;
                    // and create a passage (by destroying wall/s) between the new current cell and any adjacent cell.
                    DestroyAdjacentWall();

                    return;
                }
            }
        }
    }

    void DestroyAdjacentWall()
    {
        bool destroyed = false;

        while (!destroyed)
        {
            // pick a random adjacent cell that is visited and within boundaries,
            // and destroy the wall/s between the current cell and adjacent cell.
            int direction = Random.Range(0, 4);

            // check up.
            if (direction == 0)
            {
                if (currentRow > 0 && grid[currentRow - 1, currentColumn].Visited)
                {
                    // Debug.Log("Destroyed down wall of " + (currentRow - 1) + " " + currentColumn
                    //             + " and up wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    if (grid[currentRow - 1, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow - 1, currentColumn].DownWall);
                    }
                    
                    destroyed = true;
                }
            }
            // check down.
            else if (direction == 1)
            {
                if (currentRow < Rows - 1 && grid[currentRow + 1, currentColumn].Visited)
                {
                    // Debug.Log("Destroyed up wall of " + (currentRow + 1) + " " + currentColumn
                    //             + " and down wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    if (grid[currentRow + 1, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow + 1, currentColumn].UpWall);
                    }

                    destroyed = true;
                }
            }
            // check left.
            else if (direction == 2)
            {
                if (currentColumn > 0 && grid[currentRow, currentColumn - 1].Visited)
                {
                    // Debug.Log("Destroyed right wall of " + currentRow + " " + (currentColumn - 1)
                    //         + " and left wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    if (grid[currentRow, currentColumn - 1].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn - 1].RightWall);
                    }

                    destroyed = true;
                }
            }
            // check right.
            else if (direction == 3)
            {
                if (currentColumn < Columns - 1 && grid[currentRow, currentColumn + 1].Visited)
                {
                    // Debug.Log("Destroyed left wall of " + currentRow + " " + (currentColumn + 1)
                    //         + " and right wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    if (grid[currentRow, currentColumn + 1].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn + 1].LeftWall);
                    }

                    destroyed = true;
                }
            }
        }
    }

    bool AreThereUnvisitedNeighbors()
    {
        // check up.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow - 1, currentColumn))
        {
            return true;
        }

        // check down.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn))
        {
            return true;
        }

        // check left.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1))
        {
            return true;
        }

        // check right.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1))
        {
            return true;
        }

        return false;
    }

    public bool AreThereVisitedNeighbors(int row, int column)
    {
        // check up.
        if (row > 0 && grid[row - 1, column].Visited)
        {
            return true;
        }

        // check down.
        if (row < Rows - 1 && grid[row + 1, column].Visited)
        {
            return true;
        }

        // check left.
        if (column > 0 && grid[row, column - 1].Visited)
        {
            return true;
        }

        // check right.
        if (column < Columns - 1 && grid[row, column + 1].Visited)
        {
            return true;
        }

        return false;
    }

    // do a boundary check and unvisited check.
    bool IsCellUnvisitedAndWithinBoundaries(int row, int column)
    {
        if (row >= 0 && row < Rows && column >= 0 && column < Columns
            && !grid[row, column].Visited)
        {
            return true;
        }

        return false;
    }

}
public class MazeCell {
    public bool Visited = false;
    public GameObject UpWall;
    public GameObject DownWall;
    public GameObject LeftWall;
    public GameObject RightWall;
}