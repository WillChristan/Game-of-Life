using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTip : MonoBehaviour
{
    static ToolTip instance;

    TextMeshProUGUI tooltipText;
    RectTransform bgTrans;
    Vector3 screenSizeHalf;

    public float textPaddingSize = 5f;

    //Making a timer to disable tooltip after set time
    bool toolTipIsShown;
    float timeElapsed = 0f;
    float timerSet = 1.0f;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        if (textPaddingSize <= 0) textPaddingSize = 5f;

        //Make sure these Child Objects exists. A UI image named 'Background' and a Text Mesh Pro object named 'Text (TMP)'.
        //Alternatively, we can create public variables and set the reference in the editor.
        bgTrans = transform.Find("Background").GetComponent<RectTransform>();
        tooltipText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        screenSizeHalf.x = Camera.main.pixelWidth;
        screenSizeHalf.y = Camera.main.pixelHeight;

        screenSizeHalf *= 0.5f;

        //Hide tooltip UI at the start
        HideToolTip();
    }

    // Update is called once per frame
    void Update()
    {
        if (!toolTipIsShown) return;

        timeElapsed -= Time.deltaTime;
        if (timeElapsed > 0) return;

        HideToolTip();
    }

    void ShowToolTip(string msg)
    {
        gameObject.SetActive(true);

        tooltipText.text = msg;
        
        //Dynamically size the background according to Text Length
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize, tooltipText.preferredHeight + textPaddingSize);
        bgTrans.sizeDelta = backgroundSize;

        //Reposition the tooltip based on the Mouse Position
        Vector3 temp = Input.mousePosition - screenSizeHalf;
        temp.z = 0f;

        //Temporary fix for when tooltip is blocking the button from being pressed
        temp.y += 50f;

        transform.localPosition = temp;

        toolTipIsShown = true;
        timeElapsed = timerSet;

        Debug.Log(temp);
    }

    void HideToolTip()
    {
        gameObject.SetActive(false);
        toolTipIsShown = false;
    }

    public static void ShowToolTip_Static(string msg)
    {
        instance.ShowToolTip(msg);
    }

    public static void HideToolTip_Static()
    {
        instance.HideToolTip();
    }
}
