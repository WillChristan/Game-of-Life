using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ColorButton : MonoBehaviour
{
    public Image colorImage;
    public GameObject sliderPanel;
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;

    Cell[] cells;
    bool showSliderPanel;
    Color colorVar;


    // Start is called before the first frame update
    void Start()
    {
        showSliderPanel = false;
        sliderPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCellsRef(Cell[] cellList)
    {
        cells = cellList;
        colorVar = cells[0].cellObj.GetActiveColor();
        ChangeButtonColor(colorVar);
        
        rSlider.value = colorVar.r;
        gSlider.value = colorVar.g;
        bSlider.value = colorVar.b;
    }

    public void OnButtonPressed()
    {
        showSliderPanel = !showSliderPanel;
        sliderPanel.SetActive(showSliderPanel);
    }

    public void On_R_SliderValueChange(float value)
    {
        colorVar.r = value;
        ChangeButtonColor(colorVar);
        ChangeCellsColor(colorVar);
    }

    public void On_G_SliderValueChange(float value)
    {
        colorVar.g = value;
        ChangeButtonColor(colorVar);
        ChangeCellsColor(colorVar);
    }

    public void On_B_SliderValueChange(float value)
    {
        colorVar.b = value;
        ChangeButtonColor(colorVar);
        ChangeCellsColor(colorVar);
    }

    void ChangeButtonColor(Color newColor)
    {
        colorImage.color = newColor;
    }

    void ChangeCellsColor(Color newColor)
    {
        foreach (Cell cell in cells) 
        {
            cell.cellObj.ChangeLiveColor(newColor);
        }
    }
}
