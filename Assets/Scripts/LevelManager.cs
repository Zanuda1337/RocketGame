using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<Level> Levels;
    public List<GameObject> Images;
    public GameObject Stars;
    void Awake()
    {
        /*for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            Levels.Add()
        }*/
        RewardCount();
    }

    void Update()
    {
        
    }
    public void RewardCount()
    {
        foreach (var level in Levels)
        {
            level.RecordTime = PlayerPrefs.GetFloat($"Level{level.Id}Record");
            //level.TwoStarsTime = level.ThreeStarsTime * 1.2f;
            CalculateStars(level.RecordTime, level.ThreeStarsTime, level.TwoStarsTime, out level.StarsAchieved);

            /*if (level.RecordTime <= level.ThreeStarsTime && level.RecordTime != 0f)
            {
                level.StarsAchieved = 1;
                Debug.Log(level.StarsAchieved);
            }
            else if (level.RecordTime <= level.TwoStarsTime && level.RecordTime != 0f)
            {
                level.StarsAchieved = 2;
                Debug.Log(level.StarsAchieved);
            }
            else if (level.RecordTime >= level.TwoStarsTime && level.RecordTime != 0f)
            {
                level.StarsAchieved = 1;
                Debug.Log(level.StarsAchieved);
            }
            else
            {
                level.StarsAchieved = 0;
            }*/
        }
    }
    public void CalculateStars(float time, float threeStarsTime, float twoStarsTime, out int starsAchieved)
    {
        //float twoStarsTime = threeStarsTime * 1.4f;
        if (time <= threeStarsTime && time != 0f)
        {
            starsAchieved = 3;
        }
        else if (time <= twoStarsTime && time != 0f)
        {
            starsAchieved = 2;
        }
        else if (time >= twoStarsTime && time != 0f)
        {
            starsAchieved = 1;
        }
        else
        {
            starsAchieved = 0;
        }
    }
}
