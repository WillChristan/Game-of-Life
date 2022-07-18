using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubBlockEvent : MonoBehaviour
{
    string info;
    BoxCollider2D col;


    // Start is called before the first frame update
    void Start()
    {
        //Make sure the object has a Collider. If it's a 3D Box Collider then change this accordingly.
        col = GetComponent<BoxCollider2D>();

        Vector2 tempSize = GetComponent<RectTransform>().sizeDelta;
        tempSize.y = transform.parent.GetComponent<RectTransform>().sizeDelta.y;

        col.size = tempSize;
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public void SetInfo(string msg)
    {
        info = msg;
    }

    //Unity event called automatically by the engine
    private void OnMouseEnter()
    {
        if (info == "" || info == null) return;
        ToolTip.ShowToolTip_Static(info);
    }

    //Unity event called automatically by the engine
    private void OnMouseExit()
    {
        ToolTip.HideToolTip_Static();
    }
}
