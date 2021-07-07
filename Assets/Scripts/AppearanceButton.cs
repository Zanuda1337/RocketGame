using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AppearanceButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color SelectColor;
    [SerializeField] private Color BuyColor;
    [SerializeField] private GameObject _crystalIcon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _crystalsUI;
    private Image _image;
    private State _state;
    public UnityEvent OnBuy;
    public UnityEvent OnSelect;
    public UnityEvent NotEnoughMoney;
    private enum State {Selected, Select, Buy}
    public void Start()
    {
        _image = GetComponent<Image>();
        UpdateState();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");
        SwitchState();
        UpdateState();
    }
    public void UpdateState()
    {
        if (Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].isBought)
        {
            if (Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].isSelected)
            {
                _state = State.Selected;
            }
            else
            {
                _state = State.Select;
            }
        }
        else
        {
            _state = State.Buy;
        }
        Color();
    }
    private void Color()
    {
        switch (_state)
        {
            case State.Selected:
                _image.color = SelectedColor;
                _crystalIcon.SetActive(false);
                _text.text = "Selected";
                break;
            case State.Select:
                _image.color = SelectColor;
                _crystalIcon.SetActive(false);
                _text.text = "Select";
                break;
            case State.Buy:
                _image.color = BuyColor;
                _crystalIcon.SetActive(true);
                _text.text = $"{Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].Cost}";
                break;
            default:
                break;
        }
    }
    public void SwitchState()
    {
        if (Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].isBought)
        {
            if (!Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].isSelected)
            {
                foreach (var ufo in Appearance.Instance.UFOs)
                {
                    if (ufo.isSelected)
                    {
                        ufo.isSelected = false;
                    }
                }
                Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].isSelected = true;
                Appearance.Instance.SelectedUfo = Appearance.Instance.CurrentUfo;
                PlayerPrefs.SetInt("SelectedUFO", Appearance.Instance.CurrentUfo);
                OnSelect.Invoke();
            }
        }
        else
        {
            if (Player.Instance.Crystals >= Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].Cost)
            {
                Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].isBought = true;
                PlayerPrefs.SetInt("UFO" + Appearance.Instance.CurrentUfo + "IsBought", 1);
                OnBuy.Invoke();
                Player.Instance.CrystalsSpent += Appearance.Instance.UFOs[Appearance.Instance.CurrentUfo].Cost;
                PlayerPrefs.SetInt("CrystalsSpent", Player.Instance.CrystalsSpent);
                Player.Instance.Crystals = 10000 - Player.Instance.CrystalsSpent;
                _crystalsUI.text = Convert.ToString(Player.Instance.Crystals);
            }
            else
            {
                NotEnoughMoney.Invoke();
            }
        }
    }
}
