using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButtonsController : MonoBehaviour
{
    private bool _isMenuOpen;
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _menu2;
    [SerializeField] private GameObject _menu3;

    public void ShowButtons()
    {
        _isMenuOpen = !_isMenuOpen;
        _menu.SetActive(_isMenuOpen);
    }
    public void Back()
    {
        ShowButtons();
        _menu2.SetActive(_isMenuOpen);
        _menu3.SetActive(_isMenuOpen);
    }
    public void ShowButtons2()
    {
        _menu2.SetActive(_isMenuOpen);
    }
    public void ShowButtons3()
    {
        _menu3.SetActive(_isMenuOpen);
    }

}
