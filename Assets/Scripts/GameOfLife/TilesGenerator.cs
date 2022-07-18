using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public CellUnit cellObj; //GameObject + Script
    public int[] neighbors;
    public Vector2 position { get; }
    public int neighborCount { get; set; }

    bool isAlive = false;
    bool prevState = false;

    public Cell(CellUnit newCellObj, Vector2 newPosition) => (cellObj, position) = (newCellObj, newPosition);

    public void AssignNeighbors(int[] neighborList)
    {
        neighbors = neighborList;
    }

    public void SetState(bool newState)
    {
        prevState = isAlive;
        isAlive = newState;

        if (isAlive == prevState) return;

        cellObj.ChangeColorOnState(isAlive);
    }

    public void SwitchState()
    {
        prevState = isAlive;
        isAlive = !isAlive;

        cellObj.ChangeColorOnState(isAlive);
    }

    public void ResetState()
    {
        isAlive = false;
        prevState = false;
        neighborCount = 0;

        cellObj.ChangeColorOnState(isAlive);
    }

    public void UpdatePrevState()
    {
        prevState = isAlive;
    }

    public bool GetState()
    {
        return isAlive;
    }

    public bool GetStateComparison()
    {
        return isAlive == prevState;
    }

    public void ShowNeighborCount()
    {
        cellObj.AddText(neighborCount);
    }

    public void ShowIndex()
    {
        cellObj.ShowIndex();
    }
}


public class TilesGenerator : MonoBehaviour
{
    public CellUnit tileObj; //Prefab reference
    public Cell[] Cells;

    uint cellCol;
    uint cellRow;
    Vector2 screenMinLimit;

    bool debugOption;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Create(uint newRow, uint newCol, bool _debugOption)
    {
        cellRow = newRow;
        cellCol = newCol;

        Cells = new Cell[cellRow * cellCol];

        screenMinLimit = new Vector2((cellRow - 1) * -0.5f, (cellCol - 1) * -0.5f);

        debugOption = _debugOption;

        GenerateTiles();
    }

    void GenerateTiles()
    {
        if (tileObj == null)
        {
            Debug.Log("No tile prefab set");
            return;
        }

        Vector3 pos = Vector3.zero;
        pos.x = screenMinLimit.x;
        pos.y = screenMinLimit.y;
        pos.z = 0f; //Always zero

        int indexCount = 0;

        for (int i = 0; i < cellCol; i++)
        {
            for (int j = 0; j < cellRow; j++)
            {
                CellUnit CellObj = Instantiate<CellUnit>(tileObj, pos, Quaternion.identity);

                CellObj.AssignID(indexCount);

                //Debug: Show index number if true, else show neighbours count
                if (debugOption) CellObj.AddText(indexCount);
                else CellObj.AddText(0);

                Cells[indexCount] = new Cell(CellObj, new Vector2(j, i));

                indexCount++;
                pos.x += 1; //Increment x position
            }

            //Reset x position and increment y position
            pos.x = screenMinLimit.x;
            pos.y += 1;
        }



        //Find all the id values for all the cells on the right side of the grid to compare to
        int[] rightSideCheckVals = new int[cellCol - 2];
        int colTotal = (int)cellCol - 1;

        for (int x = 0; colTotal > 1; x++)
        {
            rightSideCheckVals[x] = ((int)cellRow * colTotal) - 1;

            colTotal--;
        }

        //Find the 4 corner blocks
        int[] cornerBlocks = new int[4];
        cornerBlocks[0] = 0;                            //Bottom Left is always Zero
        cornerBlocks[1] = (int)cellRow - 1;             //Bottom Right block   
        cornerBlocks[2] = (int)(cellRow * (cellCol - 1)); //Top Left block   
        cornerBlocks[3] = (int)((cellRow * cellCol) - 1); //Top Right block   

        bool earlyBreak = false;
        ///Find and Assign neighbouring cells
        for (int i = 0; i < Cells.Length; i++)
        {

            //Check if 4 corner blocks
            for (int j = 0; j < cornerBlocks.Length; j++)
            {
                if (i == cornerBlocks[j])
                {
                    Cells[i].AssignNeighbors(GetNeighborCorner(i, j, (int)cellRow, (int)cellCol));
                    earlyBreak = true;

                    break;
                }
            }

            if (earlyBreak)
            {
                earlyBreak = false;
                continue;
            }
                

            //If LEFT side
            if (i % cellRow == 0)
            {
                Cells[i].AssignNeighbors(GetNeighborL(i, (int)cellRow));
                continue;
            }

            //If BOTTOM 
            else if (i < cellRow)
            {
                Cells[i].AssignNeighbors(GetNeighborB(i, (int)cellRow, (int)cellCol));
                continue;
            }

            //If TOP
            else if (i > cellRow * (cellCol - 1))
            {
                Cells[i].AssignNeighbors(GetNeighborT(i, (int)cellRow, (int)cellCol));
                continue;
            }

            //If RIGHT side
            else
            { 
                foreach (int y in rightSideCheckVals)
                {
                    if (i == y)
                    {
                        Cells[i].AssignNeighbors(GetNeighborR(i, (int)cellRow));
                        earlyBreak |= true;

                        break;
                    }
                }
            }
            if (earlyBreak)
            {
                earlyBreak = false;
                continue;
            }


            //If just cells in the middle of the grid then...
            Cells[i].AssignNeighbors(GetNeighbor(i, (int)cellRow));
        }

        //Testing
        //VisualizeNeighbors();
    }

    //FOR TESTING
    void VisualizeNeighbors()
    {
        int[] test = new int[8];

        test = GetNeighborCorner(0, 0,(int)cellRow, (int) cellCol);

        foreach (int i in test)
        {
            Cells[i].cellObj.ChangeColor(Color.cyan);
        }
    }

    int[] GetNeighbor(int id, int row)
    {
        int[] neighbors = new int[8];
        //Bottom 3 cells
        neighbors[0] = id - row - 1; 
        neighbors[1] = id - row; 
        neighbors[2] = id - row + 1;  

        neighbors[3] = id - 1;  //Left cell neighbour
        neighbors[4] = id + 1;  //Right cell neighbour

        //Top 3 cells
        neighbors[5] = id + row - 1;
        neighbors[6] = id + row;
        neighbors[7] = id + row + 1;

        return neighbors;
    }

    //Get neighbours for cells on the LEFT side
    int[] GetNeighborL(int id, int row)
    {
        int[] neighbors = new int[8];
        //Bottom 3 cells
        neighbors[0] = id - 1;
        neighbors[1] = id - row;
        neighbors[2] = id - row + 1;

        neighbors[3] = id + row - 1;  //Left
        neighbors[4] = id + 1;  //Right

        //Top 3 cells
        neighbors[5] = id + (2 * row) - 1;
        neighbors[6] = id + row;
        neighbors[7] = id + row + 1;

        return neighbors;
    }

    //Get neighbours for cells on the RIGHT side
    int[] GetNeighborR(int id, int row)
    {
        int[] neighbors = new int[8];
        //Bottom 3 cells
        neighbors[0] = id - row - 1;
        neighbors[1] = id - row;
        neighbors[2] = id - (2 * row) + 1;

        neighbors[3] = id - 1;      //Left
        neighbors[4] = id - row + 1;//Right

        //Top 3 cells
        neighbors[5] = id + row - 1;
        neighbors[6] = id + row;
        neighbors[7] = id + 1;

        return neighbors;
    }

    //Get neighbours for cells on the TOP side
    int[] GetNeighborT(int id, int row, int col)
    {
        int[] neighbors = new int[8];
        //Bottom 3 cells
        neighbors[0] = id - row - 1;
        neighbors[1] = id - row;
        neighbors[2] = id - row + 1;

        neighbors[3] = id - 1;  //Left
        neighbors[4] = id + 1;  //Right

        int temp = (row * col) - row;

        //Top 3 cells
        neighbors[5] = id - temp - 1;
        neighbors[6] = id - temp;
        neighbors[7] = id - temp + 1;

        return neighbors;
    }

    //Get neighbours for cells on the BOTTOM side
    int[] GetNeighborB(int id, int row, int col)
    {
        int[] neighbors = new int[8];


        //Bottom 3 cells
        int temp = (row * col) - row;

        neighbors[0] = id + temp - 1;
        neighbors[1] = id + temp;
        neighbors[2] = id + temp + 1;

        neighbors[3] = id - 1;  //Left
        neighbors[4] = id + 1;  //Right

        //Top 3 cells
        neighbors[5] = id + row - 1;
        neighbors[6] = id + row;
        neighbors[7] = id + row + 1;

        return neighbors;
    }

    int[] GetNeighborCorner(int id, int j, int row, int col)
    {
        int[] neighbors = new int[8];
        int temp = (row * col) - row;

        switch (j)
        {
            case 0://Bottom Left Corner, aka ZERO
                //Bottom 3 cells
                neighbors[0] = (row * col) - 1;
                neighbors[1] = id + temp;
                neighbors[2] = id + temp + 1;

                neighbors[3] = row - 1;  //Left
                neighbors[4] = id + 1;  //Right

                //Top 3 cells
                neighbors[5] = (row * 2)- 1;
                neighbors[6] = id + row;
                neighbors[7] = id + row + 1;
                break;

            case 1://Bottom Right Corner
                //Bottom 3 cells
                neighbors[0] = id + temp - 1;
                neighbors[1] = id + temp;
                neighbors[2] = row * (col - 1);

                neighbors[3] = id - 1;  //Left
                neighbors[4] = 0;  //Right

                //Top 3 cells
                neighbors[5] = id + row - 1;
                neighbors[6] = id + row;
                neighbors[7] = id + 1;
                break;

            case 2://Top Left Corner
                //Bottom 3 cells
                neighbors[0] = id - 1;
                neighbors[1] = id - row;
                neighbors[2] = id - row + 1;

                neighbors[3] = (row * col) - 1;  //Left
                neighbors[4] = id + 1;  //Right

                //Top 3 cells
                neighbors[5] = row - 1;
                neighbors[6] = id - temp;
                neighbors[7] = id - temp + 1;
                break;

            case 3://Top Right Corner
                //Bottom 3 cells
                neighbors[0] = id - row - 1;
                neighbors[1] = id - row;
                neighbors[2] = id - (2 * row) + 1;

                neighbors[3] = id - 1;  //Left
                neighbors[4] = id - row + 1;  //Right

                //Top 3 cells
                neighbors[5] = id - temp - 1;
                neighbors[6] = id - temp;
                neighbors[7] = 0;
                break;
        }

        return neighbors;
    }
}
