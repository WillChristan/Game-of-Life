using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//A single Cell/Tile object
public class CellUnit : MonoBehaviour
{
    [HideInInspector]
    public int id;

    GameObject tileObj;
    Material material;
    TextMeshPro tMesh;
    Color colorBase;
    Color colorLive;


    void Awake()
    {
        tileObj = this.gameObject;
        material = GetComponentInChildren<MeshRenderer>().material;
        tMesh = GetComponentInChildren<TextMeshPro>();

        colorBase = Color.white;
        material.color = colorBase;
        colorLive = Color.cyan;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Used for mouse-click cell identification
    public void AssignID(int newID) 
    {
        id = newID;
    }

    //True for alive, false for dead
    public void ChangeColorOnState(bool isItAlive)
    {
        ChangeColor(isItAlive ? colorLive : colorBase);
    }

    public void ChangeColor(Color newColor)
    {
        material.color = newColor;
    }

    public void ChangeLiveColor(Color newColor)
    {
        colorLive = newColor;
    }

    public Color GetActiveColor()
    {
        return colorLive;
    }

    public void EnableText(bool val)
    {
        tMesh.enabled = val;
    }

    public void AddText(int val)
    {
        if (tMesh != null)
        {
            tMesh.SetText((val).ToString());
        }
    }

    public void ShowIndex()
    {
        AddText(id);
    }
}


