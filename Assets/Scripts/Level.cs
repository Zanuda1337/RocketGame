using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public string Name;

    public int Id;
    [HideInInspector] public int StarsAchieved = 0;
    public float ThreeStarsTime;
    [HideInInspector] public float RecordTime;

    public float TwoStarsTime;
    [HideInInspector] public float OneStarTime;

    /*public void Awake()
    {
        _recordTime = PlayerPrefs.GetFloat($"Level{Id}Record");
        if (_recordTime <= ThreeStarsTime)
        {
            StarsAchieved = 3;
        }
    }*/
}
