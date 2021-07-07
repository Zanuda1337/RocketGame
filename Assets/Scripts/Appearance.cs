using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Appearance : MonoBehaviour
{
    [SerializeField] private ArrowController _arrowController;
    public List<UFO> UFOs;
    public static Appearance Instance;
    public int CurrentUfo;
    public int SelectedUfo;
    [SerializeField] private AppearanceButton _button;
    [SerializeField] private TextMeshProUGUI _text;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        for (int i = 0; i < UFOs.Count; i++)
        {
            UFOs[i].isSelected = false;
            UFOs[i].isBought = false;
            if (PlayerPrefs.GetInt("UFO" + i +"IsBought", 0) == 1)
            {
                UFOs[i].isBought = true;
            }
        }
        UFOs[0].isBought = true;
        SelectedUfo = PlayerPrefs.GetInt("SelectedUFO", 0);
        UFOs[SelectedUfo].isSelected = true;
    }
    public void SwitchUfo()
    {
        CurrentUfo = _arrowController.CurrentPage;
        Debug.Log($"UFO: {CurrentUfo}");
        Debug.Log($"UFO: {UFOs[CurrentUfo].isBought}");
        _button.UpdateState();
    }
    public void Revert()
    {
        for (int i = 0; i < UFOs.Count; i++)
        {
            UFOs[i].isBought = false;
            UFOs[i].isSelected = false;
        }
        for (int i = 0; i < UFOs.Count; i++)
        {
            PlayerPrefs.DeleteKey("UFO" + i + "IsBought");
        }
        PlayerPrefs.DeleteKey("SelectedUFO");
        UFOs[0].isBought = true;
        UFOs[0].isSelected = true;
        PlayerPrefs.DeleteKey("CrystalsSpent");
        Player.Instance.CrystalsSpent = 0;
        Player.Instance.Crystals = 10000;
        _text.text = Convert.ToString(Player.Instance.Crystals);
        _button.UpdateState();
    }
    public void ResetPages()
    {
        _arrowController.CurrentPage = SelectedUfo;
        _arrowController.ManagePages();
        SwitchUfo();
        
    }
}
