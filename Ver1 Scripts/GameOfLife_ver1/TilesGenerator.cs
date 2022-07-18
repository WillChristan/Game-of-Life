using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public CellUnit cellObj; //GameObject + Script
    public Vector2 position;
    public Cell(CellUnit newCellObj, Vector2 newPosition) => (cellObj, position) = (newCellObj, newPosition);
}


public class TilesGenerator : MonoBehaviour
{
    public CellUnit tileObj;
    public Cell[] Cells;

    uint cellCol;
    uint cellRow;
    Vector2 screenMinLimit;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Create(uint newRow, uint newCol)
    {
        cellRow = newRow;
        cellCol = newCol;

        Cells = new Cell[cellRow * cellCol];

        screenMinLimit = new Vector2((cellRow - 1) * -0.5f, (cellCol - 1) * -0.5f);

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

        uint indexCount = 0;

        for (int i = 0; i < cellCol; i++)
        {
            for (int j = 0; j < cellRow; j++)
            {
                CellUnit CellObj = Instantiate<CellUnit>(tileObj, pos, Quaternion.identity);

                CellObj.AssignID(indexCount);
                CellObj.AddText((int)indexCount);
                Cells[indexCount] = new Cell(CellObj, new Vector2 (j, i));

                indexCount++;
                pos.x += 1; //Increment x position
            }

            //Reset x position and increment y position
            pos.x = screenMinLimit.x;
            pos.y += 1;
        }
    }

}
