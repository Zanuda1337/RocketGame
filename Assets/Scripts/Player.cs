using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public int Stars;
    [HideInInspector] public int StarsSpent;
    [HideInInspector] public int Crystals;
    [HideInInspector] public int CrystalsSpent;
    public static Player Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        StarsSpent = PlayerPrefs.GetInt("StarsSpent", 0);
        CrystalsSpent = PlayerPrefs.GetInt("CrystalsSpent", 0);
    }
}
