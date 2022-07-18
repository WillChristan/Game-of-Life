using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInfo
{
    public short neighbors;
    public Vector2 location;

    public CellInfo(Vector2 newLocation) => (neighbors, location) = (0, newLocation);
}

public class CellSimulator : MonoBehaviour
{
    Cell[] Cells;

    Dictionary<uint, CellInfo> CellList;
    Dictionary<uint, CellInfo> TempList;
    HashSet<int> PotentialList;
    List<uint> AddList;
    List<uint> RemoveList;

    float timeInterval;
    float counter;
    float randPercent;
    int maxCellIndex;
    bool runningSimulation;

    // Update is called once per frame
    public void Initialize(Cell[] generatedCells)
    {
        Cells = generatedCells;
        
        CellList = new Dictionary<uint, CellInfo>();
        TempList = new Dictionary<uint, CellInfo>();
        PotentialList = new HashSet<int>();
        AddList = new List<uint>();
        RemoveList = new List<uint>();

        timeInterval = 1.0f;
        counter = timeInterval;

        randPercent = 0.2f; //20% Percent of total max cell number per randomize
        maxCellIndex = Cells.Length - 1;


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

    public void RandomizeInitialLiveCells()
    {
        uint randomIndex = 0;
        CellUnit tryCell;
        int randAmount = (int)(randPercent * maxCellIndex);
        
        for (; randAmount > 0; randAmount--)
        {

            randomIndex = (uint)Random.Range(0, maxCellIndex);
            if (CellList.ContainsKey(randomIndex)) continue;

            tryCell = GetCellByIndex(randomIndex);
            if (tryCell == null) continue;

            tryCell.SetState(true);
            CellList.Add(randomIndex, new CellInfo(GetCellLocation(randomIndex)));
        }
    }

    public void ShowHideIndex(bool val)
    {
        foreach (var cell in Cells)
        {
            cell.cellObj.ShowHideIndex(val);
        }
    }

    //Mouse click on cell will flip its state and add to/remove from list
    public void SelectCell(uint index)
    {
        if (CellList.ContainsKey(index))
        {
            GetCellByIndex(index).SwitchState();
            CellList.Remove(index);
        }

        else
        {
            GetCellByIndex(index).SwitchState();
            CellList.Add(index, new CellInfo(GetCellLocation(index)));
        }
    }

    public void ResetToBase()
    {
        runningSimulation = false;
        if (CellList.Count == 0) return; //If empty, do nothing

        foreach (var Cell in CellList)
        {
            GetCellByIndex(Cell.Key).SetState(false);
        }

        CellList.Clear();
    }

    CellUnit GetCellByIndex(uint index)
    {
        if (index > maxCellIndex || index < 0) return null;
        
        return Cells[index].cellObj;
    }

    Vector2 GetCellLocation(uint index)
    {
        if (index > maxCellIndex || index < 0) return new Vector2(-1f, -1f);

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

    public bool BeginSimulation()
    {
        if (CellList.Count == 0) return false;
        
        runningSimulation = !runningSimulation;
        return runningSimulation;
    }

    public void SpeedUpTime()
    {
        if (timeInterval <= 0.05f) return;
        timeInterval -= 0.05f;
    }

    public void SlowDownTime()
    {
        if (timeInterval > 2.5f) return;
        timeInterval += 0.05f;
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

    //Variables only used in this function
    Vector2 currCellPos;
    Vector2 otherCellPos;
    Vector2 dist;
    float distSqrMag;
    int id1, id2;

    void RunSimulation()
    {
        if (CellList.Count == 0) runningSimulation = false;

        var currentEnum = CellList.GetEnumerator();

        while (currentEnum.MoveNext())
        {
            currCellPos = currentEnum.Current.Value.location;

            var nextEnum = currentEnum;

            while (nextEnum.MoveNext())
            {
                otherCellPos = nextEnum.Current.Value.location;

                dist = otherCellPos - currCellPos;
                distSqrMag = dist.sqrMagnitude;

                //distanceSqrMag == 2/1 they are 1st neighbour
                //distanceSqrMag == 4/5/8 are 2nd neighbour

                if (distSqrMag < 3f)
                {
                    currentEnum.Current.Value.neighbors++;
                    nextEnum.Current.Value.neighbors++;
                }
                else if (distSqrMag == 4f || distSqrMag == 8f)
                {
                    Vector2 nearCell = currentEnum.Current.Value.location + ((otherCellPos - currCellPos) * 0.5f);

                    id1 = GetCellIndex(nearCell);

                    if (id1 == -1) continue;
                    PotentialList.Add(id1);
                }
                else if (distSqrMag == 5f)
                {
                    Vector2 nearCell1, nearCell2;
                    nearCell1 = nearCell2 = (dist * 0.5f) + currentEnum.Current.Value.location;

                    if (dist.x % 2f == 0)
                    {
                        nearCell1.y += 0.5f;
                        nearCell2.y -= 0.5f;
                    }
                    else
                    {
                        nearCell1.x += 0.5f;
                        nearCell2.x -= 0.5f;
                    }

                    id1 = GetCellIndex(nearCell1);
                    id2 = GetCellIndex(nearCell2);

                    if (id1 == -1 || id2 == -1) continue;
                    PotentialList.Add(id1);
                    PotentialList.Add(id2);
                }

            }

            //Cell with less than 2 or more than 3 neighbours dies
            if (currentEnum.Current.Value.neighbors < 2 || currentEnum.Current.Value.neighbors > 3)
            {
                RemoveList.Add(currentEnum.Current.Key);
            }
            

            //Reset number of neighbours
            currentEnum.Current.Value.neighbors = 0;
        }
        
        if (PotentialList.Count > 0)
        {
            foreach (var emptyCell in PotentialList)
            {
                foreach (var liveCell in CellList)
                {
                    currCellPos = GetCellLocation((uint)emptyCell);
                    dist = liveCell.Value.location - currCellPos;
                    distSqrMag = dist.sqrMagnitude;

                    if (distSqrMag < 3f)
                    {
                        if (TempList.ContainsKey((uint)emptyCell))
                        {
                            TempList[(uint)emptyCell].neighbors++;
                        }
                        else
                        {
                            TempList.Add((uint)emptyCell, new CellInfo(currCellPos));
                            TempList[(uint)emptyCell].neighbors++;
                        }
                    }
                }

                if (TempList[(uint)emptyCell].neighbors == 3)
                {
                    AddList.Add((uint)emptyCell);
                }
            }

            TempList.Clear();
            PotentialList.Clear();
        }
        

        foreach(uint index in AddList)
        {
            if (CellList.ContainsKey(index)) continue;
            CellList.Add(index, new CellInfo(GetCellLocation(index)));
            GetCellByIndex((uint)index).SetState(true);
        }
        AddList.Clear();

        //Remove dead cells and clear out the RemoveList
        foreach(uint index in RemoveList)
        {
            GetCellByIndex(index).SetState(false);
            CellList.Remove(index);
        }
        RemoveList.Clear();
    }

    //RULES
    //Cell with <1 neighbour dies
    //Cell with >4 neightbours dies
    //Cell with 2 or 3 neighbours lives
    //Empty with 3 neighbours lives
}
