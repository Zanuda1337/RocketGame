using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private int _levelUnlock;
    [SerializeField] private GameObject _exitButtons;
    //[SerializeField] private GameObject[] _tabs;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private AudioSource _exitSound;
    [SerializeField] private GameObject _resetMenu;
    [SerializeField] private ResetButtonsController _resetButtons;
    [SerializeField] private LevelManager _levelManager;
    private Image _image;
    private bool _isExit = false;
    void Awake()
    {
        //UpdateButtons();
    }
    private void Start()
    {
        UpdateButtons();
        if (AudioManager.instance!=null)
        {
            Destroy(AudioManager.instance.gameObject);
        }
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
        PlayerPrefs.DeleteKey("Quality");
        PlayerPrefs.DeleteKey("Resolution");
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            PlayerPrefs.DeleteKey("Level" + i + "Record");
        }
        _levelManager.RewardCount();
        UpdateButtons();
        _resetButtons.Back();
    }
    public void UpdateButtons()
    {
        _levelUnlock = PlayerPrefs.GetInt("levels", 1);
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = false;
            for (int j = 0; j < 3; j++)
            {
                _buttons[i].transform.GetChild(j).transform.GetChild(0).gameObject.SetActive(false);
                SetColor(_buttons[i].transform.GetChild(j).gameObject, new Vector4(1,1,1,0.3f));     
            }
        }

        for (int i = 0; i < _levelUnlock; i++)
        {
            if (SceneManager.sceneCountInBuildSettings > i + 1)
            {
                _buttons[i].interactable = true;
                for (int j = 0; j < _levelManager.Levels[i].StarsAchieved; j++)
                {
                    Debug.Log($"Даю звезду номер {j}  уровню номер {i}");
                    _buttons[i].transform.GetChild(j).transform.GetChild(0).gameObject.SetActive(true);
                    SetColor(_buttons[i].transform.GetChild(j).gameObject, new Vector4(1, 1, 1, 1f));
                }
                for (int y = 0; y < 3; y++)
                {
                    SetColor(_buttons[i].transform.GetChild(y).gameObject, new Vector4(1, 1, 1, 1f));
                }
            }
        }
    }
    public void SetColor(GameObject gameObject, Vector4 color)
    {
        _image = gameObject.GetComponent<Image>();
        _image.color = color;
    }
}
