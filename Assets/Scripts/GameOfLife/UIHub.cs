using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHub : MonoBehaviour
{
    public TextMeshProUGUI dispFrequency;
    public TextMeshProUGUI dispGeneration;
    public Image playButtonImage;
    public Sprite spritePlayButton;
    public Sprite spritePauseButton;
    public Button SpeedUpButtonRef;
    public Button SlowDownButtonRef;
    public Button PlayButtonRef;
    public Button StepForwardButtonRef;
    public TMP_InputField RandomizeInputField;
    public GameObject ColorChangeButton;


    // Start is called before the first frame update
    void Awake()
    {
        SpeedUpButtonRef.GetComponent<SubBlockEvent>().SetInfo("Speed Up Time");
        SlowDownButtonRef.GetComponent<SubBlockEvent>().SetInfo("Slow Down Time");
        PlayButtonRef.GetComponent<SubBlockEvent>().SetInfo("Begin Simulation");
        StepForwardButtonRef.GetComponent<SubBlockEvent>().SetInfo("Advance One Generation");
        RandomizeInputField.GetComponent<SubBlockEvent>().SetInfo("Enter % amount to randomize (1-90)");
        ColorChangeButton.GetComponent<SubBlockEvent>().SetInfo("Click to change colour");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPlayButton()
    {
        if (playButtonImage != null && spritePlayButton != null)
        {
            playButtonImage.sprite = spritePlayButton;
        }
    }

    public void ShowPauseButton()
    {
        if (playButtonImage != null && spritePlayButton != null)
        {
            playButtonImage.sprite = spritePauseButton;
        }
    }

    public void UpdateFrequencyDisplay(float newValue)
    {
        if (dispFrequency != null)
        {
            double temp = System.Math.Round(newValue, 2);
            dispFrequency.SetText(temp.ToString() + " sec");
        }
    }

    public void UpdateGenerationDisplay(uint newValue)
    {
        if (dispGeneration != null)
        {
            dispGeneration.SetText(newValue.ToString());
        }
    }

    public void ChangeRandomizerInputText(string text)
    {
        RandomizeInputField.text = text;
    }
}
