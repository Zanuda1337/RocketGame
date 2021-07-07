using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Garage : MonoBehaviour
{
    [SerializeField] private Button _revertButton;
    [SerializeField] private Animator _star;
    [SerializeField] private UpgradeButton _angularVelocityButton;
    [SerializeField] private UpgradeButton _powerButton;
    [SerializeField] private UpgradeButton _maxAngleButton;
    [SerializeField] private UpgradeButton _indicatorsButton;
    [SerializeField] private TextMeshProUGUI _starsUI;

    [SerializeField] private GameObject _star1;
    [SerializeField] private GameObject _star2;
    [SerializeField] private GameObject _star3;
    [SerializeField] private GameObject _star4;
    [SerializeField] private List<GameObject> UFOs;
    [SerializeField] private CanvasGroup _garageCanvas;

    private TextMeshProUGUI _text1;
    private TextMeshProUGUI _text2;
    private TextMeshProUGUI _text3;
    private TextMeshProUGUI _text4;

    public int FirstGradePrice = 9;
    public int SecondGradePrice = 15;
    public int ThirdGradePrice = 25;
    public int IndicatorsPrice = 3;

    public static Garage Instance;


    private float _dampingMod;
    private float _maxAngularMagnitudeMod;
    private float _angularForceMod;
    private float _maxAngleMod;
    private float _thrustPowerMod;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Start()
    {
        _star1.SetActive(true);
        _star2.SetActive(true);
        _star3.SetActive(true);
        _star4.SetActive(true);
        _text1 = _star1.GetComponentInChildren<TextMeshProUGUI>();
        _text2 = _star2.GetComponentInChildren<TextMeshProUGUI>();
        _text3 = _star3.GetComponentInChildren<TextMeshProUGUI>();
        _text4 = _star4.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log("Активность первой звезды: " + _star1.activeSelf);
        Debug.Log("Активность второй звезды: " + _star2.activeSelf);
        Debug.Log("Активность третьей звезды: " + _star3.activeSelf);
        Debug.Log("Активность четвертой звезды: " + _star4.activeSelf);
        UpdateButtons();
        UpdatePrice("Upgrade1", _text1, _star1);
        UpdatePrice("Upgrade2", _text2, _star2);
        UpdatePrice("Upgrade3", _text3, _star3);
        UpdateIndicatorsPrice("Upgrade4", _text4, _star4);
        UpdateIndicators();
    }
    public void ActivateRevert()
    {
        _revertButton.interactable = true;
        _star.SetTrigger("ScaleUp");
        Debug.Log("canvas: " + _garageCanvas);
        if (_garageCanvas != null)
        {
            _garageCanvas.alpha = 1;
            _garageCanvas.blocksRaycasts = true;
            _garageCanvas.interactable = true;
        }

    }
    public void DeactivateRevert()
    {
        _revertButton.interactable = false;
        _star.SetTrigger("ScaleDown");
        if (_garageCanvas != null)
        {
            _garageCanvas.alpha = 0;
            _garageCanvas.blocksRaycasts = false;
            _garageCanvas.interactable = false;
        }
    }
    public void UpgradeAngVelocity()
    {
        switch (_angularVelocityButton.Value)
        {
            case 1:
                PlayerPrefs.SetFloat("MaxAngularMagnitude", 0.63f);
                PlayerPrefs.SetFloat("AngularForce", 6f);
                Player.Instance.StarsSpent += FirstGradePrice;
                //_text1.text = Convert.ToString(_secondGradePrice);
                break;
            case 2:
                PlayerPrefs.SetFloat("MaxAngularMagnitude", 1.26f);
                PlayerPrefs.SetFloat("AngularForce", 12f);
                Player.Instance.StarsSpent += SecondGradePrice;
                //_text1.text = Convert.ToString(_thirdGradePrice);
                break;
            case 3:
                PlayerPrefs.SetFloat("MaxAngularMagnitude", 1.9f);
                PlayerPrefs.SetFloat("AngularForce", 18f);
                Player.Instance.StarsSpent += ThirdGradePrice;
                //_star1.SetActive(false);
                break;
            default:
                PlayerPrefs.DeleteKey("MaxAngularMagnitude");
                PlayerPrefs.DeleteKey("AngularForce");
                //_text1.text = Convert.ToString(_firstGradePrice);
                break;
        }
        PlayerPrefs.SetInt("Upgrade1", _angularVelocityButton.Value);
        PlayerPrefs.SetInt("StarsSpent", Player.Instance.StarsSpent);
        UpdateStars();
        UpdatePrice("Upgrade1", _text1, _star1);
    }
    public void UpgradePower()
    {
        switch (_powerButton.Value)
        {
            case 1:
                PlayerPrefs.SetFloat("ThrustPower", 30f);
                Player.Instance.StarsSpent += FirstGradePrice;
                //_text2.text = Convert.ToString(_secondGradePrice);
                break;
            case 2:
                PlayerPrefs.SetFloat("ThrustPower", 60f);
                Player.Instance.StarsSpent += SecondGradePrice;
                //_text2.text = Convert.ToString(_thirdGradePrice);
                break;
            case 3:
                PlayerPrefs.SetFloat("ThrustPower", 90f);
                Player.Instance.StarsSpent += ThirdGradePrice;
                //_star2.SetActive(false);
                break;
            default:
                PlayerPrefs.DeleteKey("ThrustPower");
                //_text2.text = Convert.ToString(_firstGradePrice);
                break;
        }
        PlayerPrefs.SetInt("Upgrade2", _powerButton.Value);
        PlayerPrefs.SetInt("StarsSpent", Player.Instance.StarsSpent);
        UpdateStars();
        UpdatePrice("Upgrade2", _text2, _star2);
    }
    public void UpgradeAngle()
    {
        switch (_maxAngleButton.Value)
        {
            case 1:
                PlayerPrefs.SetFloat("MaxAngle", 0.075f);
                Player.Instance.StarsSpent += FirstGradePrice;
                //_text3.text = Convert.ToString(_secondGradePrice);
                break;
            case 2:
                PlayerPrefs.SetFloat("MaxAngle", 0.15f);
                Player.Instance.StarsSpent += SecondGradePrice;
                //_text3.text = Convert.ToString(_thirdGradePrice);
                break;
            case 3:
                PlayerPrefs.SetInt("IsAngleClamped", 0);
                PlayerPrefs.SetFloat("Damping", 6f);
                Player.Instance.StarsSpent += ThirdGradePrice;
                //_star3.SetActive(false);
                break;
            default:
                PlayerPrefs.DeleteKey("MaxAngle");
                PlayerPrefs.DeleteKey("Damping");
                PlayerPrefs.DeleteKey("IsAngleClamped");
                //_text3.text = Convert.ToString(_firstGradePrice);
                break;
        }
        PlayerPrefs.SetInt("Upgrade3", _maxAngleButton.Value);
        PlayerPrefs.SetInt("StarsSpent", Player.Instance.StarsSpent);
        UpdateStars();
        UpdatePrice("Upgrade3", _text3, _star3);
    }
    public void UpgradeIndicators()
    {
        switch (_indicatorsButton.Value)
        {
            case 1:
                PlayerPrefs.SetInt("Indicators", 1);
                Player.Instance.StarsSpent += IndicatorsPrice;
                //_text1.text = Convert.ToString(_secondGradePrice);
                break;
            default:
                PlayerPrefs.DeleteKey("Indicators");
                //_text1.text = Convert.ToString(_firstGradePrice);
                break;
        }
        PlayerPrefs.SetInt("Upgrade4", _indicatorsButton.Value);
        PlayerPrefs.SetInt("StarsSpent", Player.Instance.StarsSpent);
        UpdateStars();
        UpdateIndicators();
        UpdateIndicatorsPrice("Upgrade4", _text4, _star4);
    }

    public void RevertStars()
    {
        _star.SetTrigger("Revert");
        Player.Instance.StarsSpent = 0;

        PlayerPrefs.DeleteKey("Upgrade1");
        PlayerPrefs.DeleteKey("Upgrade2");
        PlayerPrefs.DeleteKey("Upgrade3");
        PlayerPrefs.DeleteKey("Upgrade4");

        _angularVelocityButton.Value = 0;
        _angularVelocityButton.Value = PlayerPrefs.GetInt("Upgrade1", 0);
        _angularVelocityButton.UpdatePosition();
        UpgradeAngVelocity();

        _powerButton.Value = 0;
        _powerButton.Value = PlayerPrefs.GetInt("Upgrade2", 0);
        _powerButton.UpdatePosition();
        UpgradePower();

        _maxAngleButton.Value = 0;
        _maxAngleButton.Value = PlayerPrefs.GetInt("Upgrade3", 0);
        _maxAngleButton.UpdatePosition();
        UpgradeAngle();

        _indicatorsButton.Value = 0;
        _indicatorsButton.Value = PlayerPrefs.GetInt("Upgrade4", 0);
        _indicatorsButton.UpdatePosition();
        UpgradeIndicators();

        //UpgradeAngVelocity();
        //UpgradePower();
        PlayerPrefs.DeleteKey("StarsSpent");
        UpdatePrice("Upgrade1", _text1, _star1);
        UpdatePrice("Upgrade2", _text2, _star2);
        UpdatePrice("Upgrade3", _text3, _star3);
        UpdateIndicatorsPrice("Upgrade4", _text4, _star4);
        UpdateIndicators();

    }
    public void UpdateStars()
    {
        //Debug.Log(Player.Instance.StarsSpent);
        LevelManager.Instance.RewardCount();
        _starsUI.text = Convert.ToString(Player.Instance.Stars);
    }
    public void UpdateButtons()
    {
        _angularVelocityButton.Value = PlayerPrefs.GetInt("Upgrade1", 0);
        _powerButton.Value = PlayerPrefs.GetInt("Upgrade2", 0);
        _maxAngleButton.Value = PlayerPrefs.GetInt("Upgrade3", 0);
        _indicatorsButton.Value = PlayerPrefs.GetInt("Upgrade4", 0);
        _angularVelocityButton.UpdatePosition();
        _powerButton.UpdatePosition();
        _maxAngleButton.UpdatePosition();
        _indicatorsButton.UpdatePosition();
        Debug.Log($"PlayerPrefs.GetInt(Upgrade1) = {PlayerPrefs.GetInt("Upgrade1", 0)}");
        Debug.Log($"PlayerPrefs.GetInt(Upgrade2) = {PlayerPrefs.GetInt("Upgrade2", 0)}");
        Debug.Log($"PlayerPrefs.GetInt(Upgrade3) = {PlayerPrefs.GetInt("Upgrade3", 0)}");
    }
    public void UpdatePrice(string key, TextMeshProUGUI text, GameObject star)
    {
        star.SetActive(true);
        switch (PlayerPrefs.GetInt(key, 0))
        {
            case 1:
                text.text = Convert.ToString(SecondGradePrice);
                break;
            case 2:
                text.text = Convert.ToString(ThirdGradePrice);
                break;
            case 3:
                star.SetActive(false);
                break;
            default:
                text.text = Convert.ToString(FirstGradePrice);
                break;
        }
    }
    public void UpdateIndicatorsPrice(string key, TextMeshProUGUI text, GameObject star)
    {
        star.SetActive(true);
        switch (PlayerPrefs.GetInt(key, 0))
        {
            case 1:
                star.SetActive(false);
                break;
            default:
                text.text = Convert.ToString(IndicatorsPrice);
                break;
        }
    }
    public void UpdateIndicators()
    {
        Debug.Log("Индикаторы: " + PlayerPrefs.GetInt("Upgrade4", 0));
        if (PlayerPrefs.GetInt("Upgrade4", 0) == 1)
        {
            foreach (var ufo in UFOs)
            {
                ufo.transform.GetChild(1).transform.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var ufo in UFOs)
            {
                ufo.transform.GetChild(1).transform.gameObject.SetActive(false);
            }
        }
    }
    public void NotEnoughMoney()
    {

    }
}
