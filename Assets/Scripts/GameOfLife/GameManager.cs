using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public UI_ColorButton colorButtonUI;
    public bool enableDebugText = false;
    public bool showIdOrCount = false;

    TilesGenerator tilesGenerator;
    CellSimulator cellSimulator;
    UIHub uiHub;
    
    Vector3 cameraPos; //Adjusting camera zoom

    bool IsMouseOverGameWindow
    {
        get
        {
            return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
        }
    }


    //20 by 10 Cells is the current optimal distance for the camera
    [Range(5, 150)]
    public ushort cellRow;

    [Range(5, 150)]
    public ushort cellCol;

    bool isSimulating;


    // Start is called before the first frame update
    void Awake()
    {
        tilesGenerator = GetComponent<TilesGenerator>();
        tilesGenerator.Create(cellRow, cellCol, showIdOrCount);

        cellSimulator = GetComponent<CellSimulator>();
        cellSimulator.Initialize(tilesGenerator.Cells);
        cellSimulator.EnableDebug(enableDebugText);

        uiHub = GetComponent<UIHub>();

        colorButtonUI.AddCellsRef(tilesGenerator.Cells);

        //Adjusting camera z distance according to the size of the cell grid
        cameraPos = mainCam.transform.position;
        float hRatio = (cellRow - 20f) * 0.5f;
        float vRatio = (cellCol - 10f) * 0.5f;

        cameraPos.z -= hRatio > vRatio ? hRatio : vRatio;

        mainCam.transform.position = cameraPos;

        //Set UI Frequency Display
        uiHub.UpdateFrequencyDisplay(cellSimulator.GetTimeInterval());
        uiHub.UpdateGenerationDisplay(0);

        isSimulating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) MouseClick();
        if (Input.GetMouseButtonDown(1)) RightMouseClick();
        if (Input.GetMouseButton(1)) RightMouseHold();
        if (Input.mouseScrollDelta.y != 0) MouseScroll();

        if (isSimulating) uiHub.UpdateGenerationDisplay(cellSimulator.generationCount);
    }

    void MouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "CellObject")
            {
                int testId = hit.transform.GetComponentInParent<CellUnit>().id;

                cellSimulator.SelectCell(testId);
            }
        }
    }

    Vector2 initialMousePos;
    Vector2 deltaMousePos;

    void RightMouseClick()
    {
        if (!IsMouseOverGameWindow) return;

        initialMousePos = Input.mousePosition;
    }

    void RightMouseHold()
    {
        if (!IsMouseOverGameWindow) return;

        cameraPos = mainCam.transform.position;
        deltaMousePos = (Vector2)Input.mousePosition - initialMousePos;

        cameraPos.x -= deltaMousePos.x * 0.1f;
        cameraPos.y -= deltaMousePos.y * 0.1f;
        mainCam.transform.position = cameraPos;

        initialMousePos = Input.mousePosition;
    }

    void MouseScroll()
    {
        if (!IsMouseOverGameWindow) return;

        float scrollVal =  Input.mouseScrollDelta.y;

        cameraPos = mainCam.transform.position;
        cameraPos.z += scrollVal;

        mainCam.transform.position = cameraPos;
    }

    //Input Functions
    public void Randomize()
    {
        cellSimulator.RandomizeInitialLiveCells();
    }

    public void Reset()
    {
        cellSimulator.ResetToBase();
        uiHub.ShowPlayButton();
        uiHub.UpdateGenerationDisplay(0);
    }

    public void TimeSpeedUp()
    {
        cellSimulator.SpeedUpTime();
        uiHub.UpdateFrequencyDisplay(cellSimulator.GetTimeInterval());
    }
    
    public void TimeSlowDown()
    {
        cellSimulator.SlowDownTime();
        uiHub.UpdateFrequencyDisplay(cellSimulator.GetTimeInterval());
    }

    public void Play()
    {
        isSimulating = cellSimulator.BeginSimulation();

        if (isSimulating) uiHub.ShowPauseButton();
        else uiHub.ShowPlayButton();
    }

    public void StepForward()
    {
        cellSimulator.StepForwardAGeneration();
        uiHub.ShowPlayButton();
        uiHub.UpdateGenerationDisplay(cellSimulator.generationCount);
    }

    public void PercentRandomizerInput(string input)
    {
        float tryNumber = 0; 
        float.TryParse(input, out tryNumber);

        if (tryNumber == 0) 
        {
            uiHub.ChangeRandomizerInputText(cellSimulator.GetRandPercent().ToString());
            return;
        }
        if (tryNumber > 90)
        {
            uiHub.ChangeRandomizerInputText(cellSimulator.GetRandPercent().ToString());
            return;
        }

        cellSimulator.ChangeRandPercent(tryNumber);
    }

    //Need simulation //Life cycle speed
    //Need to clean up Randomizer and Mouse Select (is it only on initial or also during simulation? What for?)
    
}
