using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<Level> Levels;
    public List<GameObject> Images;
    public GameObject Stars;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        RewardCount();
    }
    void Start()
    {
        //RewardCount();
    }

    public void RewardCount()
    {
        int totalStars = 0;
        foreach (var level in Levels)
        {
            level.RecordTime = PlayerPrefs.GetFloat($"Level{level.Id}Record", 0);
            CalculateStars(level.RecordTime, level.ThreeStarsTime, level.TwoStarsTime, out level.StarsAchieved);
            Debug.Log(level.StarsAchieved);
            totalStars += level.StarsAchieved;
        }
        if (Player.Instance != null) Player.Instance.Stars = totalStars - Player.Instance.StarsSpent + 100;
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
