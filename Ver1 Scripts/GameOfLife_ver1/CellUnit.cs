using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CellUnit : MonoBehaviour
{
    [HideInInspector]
    public uint id;

    GameObject tileObj;
    Material material;
    TextMeshPro tMesh;
    Color colorBase;
    Color colorLive;
    bool isAlive;

    // Start is called before the first frame update
    void Awake()
    {
        tileObj = this.gameObject;
        material = GetComponentInChildren<MeshRenderer>().material;

        colorBase = Color.black;
        material.color = colorBase;
        colorLive = Color.green;
        isAlive = false;

        tMesh = GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignID(uint newID) 
    {
        id = newID;
    }

    //True for alive, false for dead
    public void SetState(bool newState)
    {
        if (isAlive = newState)
        {
            ChangeColor(colorLive);
        }
        else
        {
            ChangeColor(colorBase);
        }
    }

    public void SwitchState()
    {
        isAlive = !isAlive;
        ChangeColor(isAlive ? colorLive : colorBase); //'Inline if' statement
    }

    public void ChangeColor(Color newColor)
    {
        material.color = newColor;
    }

    public void AddText(int val)
    {
        if (tMesh != null)
        {
            tMesh.SetText((val).ToString());
        }
    }

    public void ShowHideIndex(bool val)
    {
        tMesh.enabled = val;
    }

    public bool GetState()
    {
        return isAlive;
    }
}
