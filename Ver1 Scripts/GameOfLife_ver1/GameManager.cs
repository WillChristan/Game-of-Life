using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public TextMeshProUGUI dispFrequency;
    public Image playButtonImage;
    public Sprite spritePlayButton;
    public Sprite spritePauseButton;
    public bool showIndex = false;

    TilesGenerator tilesGenerator;
    CellSimulator cellSimulator;

    //20 by 10 Cells is the current optimal distance for the camera
    [Range(1, 150)]
    public ushort cellRow;

    [Range(1, 150)]
    public ushort cellCol;
    
    bool isSimulating;

    // Start is called before the first frame update
    void Awake()
    {
        tilesGenerator = GetComponent<TilesGenerator>();
        tilesGenerator.Create(cellRow, cellCol);

        cellSimulator = GetComponent<CellSimulator>();
        cellSimulator.Initialize(tilesGenerator.Cells);
        cellSimulator.ShowHideIndex(showIndex);

        //Adjusting camera z distance according to the size of the cell grid
        Vector3 cameraPos = mainCam.transform.position;
        float hRatio = (cellRow - 20f) * 0.5f;
        float vRatio = (cellCol - 10f) * 0.5f;

        cameraPos.z -= hRatio > vRatio? hRatio : vRatio;

        mainCam.transform.position = cameraPos;

        //Set UI Frequency Display
        UpdateFrequencyDisplay();

        isSimulating = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSimulating) MouseClick();

    }

    void MouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "CellObject")
            {
                uint testId = hit.transform.GetComponentInParent<CellUnit>().id;
                
                cellSimulator.SelectCell(testId);
            }
        }
    }

    //Input Functions
    public void Randomize()
    {
        cellSimulator.RandomizeInitialLiveCells();
    }

    public void Reset()
    {
        cellSimulator.ResetToBase();
        playButtonImage.sprite = spritePlayButton;
    }

    public void TimeSpeedUp()
    {
        cellSimulator.SpeedUpTime();
        UpdateFrequencyDisplay();
    }

    public void TimeSlowDown()
    {
        cellSimulator.SlowDownTime();
        UpdateFrequencyDisplay();
    }

    public void Play()
    {
        bool playBool = cellSimulator.BeginSimulation();
        playButtonImage.sprite = playBool? spritePauseButton : spritePlayButton;
    }

    public void StepForward()
    {
        cellSimulator.StepForwardAGeneration();
        playButtonImage.sprite = spritePlayButton;
    }

    //Need simulation //Life cycle speed
    //Need to clean up Randomizer and Mouse Select (is it only on initial or also during simulation? What for?)
    //Pause Button missing still

    //Helper Functions
    void UpdateFrequencyDisplay()
    {
        if (dispFrequency != null)
        {
            double temp = System.Math.Round(cellSimulator.GetTimeInterval(), 2);
            dispFrequency.SetText(temp.ToString() + " sec");
        }
    }
}
