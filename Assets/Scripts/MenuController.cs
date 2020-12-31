using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private int _levelUnlock;
    [SerializeField] private GameObject _exitButtons;
    [SerializeField] private GameObject[] _tabs;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private AudioSource _exitSound;
    [SerializeField] private GameObject _resetMenu;
    [SerializeField] private ResetButtonsController _resetButtons;
    private bool _isExit = false;
    void Awake()
    {
        UpdateButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _resetMenu.activeSelf==false)
        {
            ShowExitButtons();
            if (_exitSound != null) _exitSound.Play();
        }
    }
    public void ShowExitButtons()
    {
        _isExit = !_isExit;
        _exitButtons.SetActive(_isExit);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Back()
    {
        ShowExitButtons();
    }
    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("levels");
        UpdateButtons();
        _resetButtons.Back();
    }
    public void UpdateButtons()
    {
        _levelUnlock = PlayerPrefs.GetInt("levels", 1);
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = false;
        }

        for (int i = 0; i < _levelUnlock; i++)
        {
            _buttons[i].interactable = true;
        }
    }
}
