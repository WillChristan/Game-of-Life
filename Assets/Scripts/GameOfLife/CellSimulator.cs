using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellSimulator : MonoBehaviour
{
    public uint generationCount { get { return generationElapsed; } }
    
    Cell[] Cells;

    float timeInterval;
    float timeInc;
    float counter;
    float randPercent;
    int maxCellIndex;
    uint generationElapsed;
    bool runningSimulation;


    // Update is called once per frame
    public void Initialize(Cell[] generatedCells)
    {
        Cells = generatedCells;
        
        timeInterval = 0.2f;
        counter = timeInterval;
        timeInc = 0.01f;
        randPercent = 0.2f; //20% Percent of total max cell number per randomize
        maxCellIndex = Cells.Length - 1;
        generationElapsed = 0;
        runningSimulation = false;
    }

    void Update()
    {

        if (!runningSimulation) return;

        //Counter/Time related events goes below
        counter -= Time.deltaTime;
        if (counter > 0) return;

        //Time related events start
        RunSimulation();
        
        //Time related events ends
        counter = timeInterval;
    }
    
    public void EnableDebug(bool val)
    {
        foreach (var cell in Cells)
        {
            cell.cellObj.EnableText(val);
        }

    }
    
    //Randomizer value input
    public void ChangeRandPercent(float value)
    {
        if (value < 0f || value > 100f) return;
        //Convert into 0-1 range
        randPercent = value * 0.01f;
    }

    //Randomizer UI output
    public float GetRandPercent()
    {
        //Convert back into percent form
        return (randPercent * 100f);
    }

    public void RandomizeInitialLiveCells()
    {
        uint randomIndex = 0;
        int randAmount = (int)(randPercent * maxCellIndex);
        
        for (; randAmount > 0; randAmount--)
        {
            randomIndex = (uint)Random.Range(0, maxCellIndex);

            var temp = Cells[randomIndex];

            //Prevent duplicate calls
            if (temp.GetState() == true) continue;

            temp.SetState(true);
            UpdateNeighborCounts(temp.neighbors, true);
        }
    }

    //Mouse click on cell will flip its state and add to/remove from list
    public void SelectCell(int index)
    {
        var temp = Cells[index];

        temp.SwitchState();
        UpdateNeighborCounts(temp.neighbors, temp.GetState());
    }

    //Reset/Deactivate all cells
    public void ResetToBase()
    {
        runningSimulation = false;

        foreach (Cell cell in Cells)
        {
            cell.ResetState();
            cell.ShowNeighborCount();
        }

        generationElapsed = 0;
    }

    CellUnit GetCellByIndex(uint index)
    {
        if (index > maxCellIndex) return null;
        
        return Cells[index].cellObj;
    }

    Vector2 GetCellLocation(uint index)
    {
        if (index > maxCellIndex) return new Vector2(-1f, -1f);

        return Cells[index].position;
    }

    int GetCellIndex(Vector2 location)
    {
        for (int i = maxCellIndex; i > 0; i--)
        {
            if (location == Cells[i].position)
            {
                return i;
            }
        }

        return -1;
    }

    public void UpdateNeighborCounts(int[] list, bool alive)
    {
        foreach(int id in list)
        {
            if (alive)
                Cells[id].neighborCount++;
            else
                Cells[id].neighborCount--;

            Cells[id].ShowNeighborCount();
        }
    }

    public bool BeginSimulation()
    {
        
        runningSimulation = !runningSimulation;
        return runningSimulation;
    }

    public void SpeedUpTime()
    {
        if (timeInterval <= 0.01f) return;
        timeInterval -= timeInc;
    }

    public void SlowDownTime()
    {
        if (timeInterval > 1.0f) return;
        timeInterval += timeInc;
    }

    public void StepForwardAGeneration()
    {
        runningSimulation = false;
        RunSimulation();
    }

    public float GetTimeInterval() 
    { 
        return timeInterval; 
    }

    void RunSimulation()
    {
        //RULES
        //Cell with <1 neighbour dies
        //Cell with >4 neightbours dies
        //Cell with 2 or 3 neighbours lives
        //Empty with 3 neighbours lives

        //Evaluating the state of each cells in response to its neighbourCount
        foreach (Cell cell in Cells)
        {
            int count = cell.neighborCount;

            //If Alive
            if (cell.GetState())
            {
                //If less than 2 neighbours or more than 3 neighbours...
                if (count < 2 || count > 3)
                {
                    cell.SwitchState();
                }
                else
                {
                    cell.UpdatePrevState();
                }
            }
            //If Dead
            else
            {
                //If there are 3 live neighbours
                if (count == 3)
                {
                    cell.SwitchState();
                }
                else
                {
                    cell.UpdatePrevState();
                }
            }

        }

        //Update the neighbourCount AFTER all cells' state have been evaluated
        //Updating the neighbourCount during evaluation would cause constant rippling 
        foreach (Cell cell in Cells)
        {
            if (!cell.GetStateComparison())
            {
                UpdateNeighborCounts(cell.neighbors, cell.GetState());
            }
        }
        generationElapsed++;
    }
}
