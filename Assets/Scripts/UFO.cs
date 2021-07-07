using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UFO
{
    public string Name;
    public int Cost;
    public Color GlowColor;
    /*[HideInInspector]*/ public bool isBought;
    /*[HideInInspector]*/ public bool isSelected;
}
