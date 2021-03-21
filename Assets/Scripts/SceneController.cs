using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Text _debugCurrentLevelUI;
    [SerializeField] private Text _debugEnergyUI;
    [SerializeField] private Text _debugTimerUI;
    [SerializeField] private Text _debugRecordUI;
    [SerializeField] private Text _debugSpeed;

    [SerializeField] private TextMeshProUGUI _currentLevelUI;
    [SerializeField] private GameObject _stars;
    [SerializeField] private TextMeshProUGUI _requireTimeUI;
    [SerializeField] private TextMeshProUGUI _timerUI;

    [SerializeField] private GameObject _debug;
    [SerializeField] private GameObject _menu;

    [SerializeField] private LevelManager _levelManager;
    [HideInInspector] public int StarsAchieved;
    private bool _isDebugShown;
    public bool IsMenuShown = false;
    private float _timer;
    private float _recordTime;
    private Vector3 _velocity, _angularVelocity;
    private Scene _currentLevel;
    [SerializeField] private Button _nextButton;

    public static SceneController instance;

    void Start()
    {
        Time.timeScale = 1f;
        if (PlayerPrefs.GetInt("Debug") == 1) { _isDebugShown = true; _debug.SetActive(true); }
        else { _isDebugShown = false; _debug.SetActive(false); }
        _nextButton.interactable = false;
        _currentLevel = SceneManager.GetActiveScene();
        Debug.Log($"На данный момент открыто {PlayerPrefs.GetInt("levels", 1)} уровней");
        if (_currentLevel.buildIndex < PlayerPrefs.GetInt("levels", 1) && SceneManager.sceneCountInBuildSettings - 1> _currentLevel.buildIndex) _nextButton.interactable = true;
        Debug.Log($"{SceneManager.sceneCountInBuildSettings - 1} > {_currentLevel.buildIndex}");
        //if (SceneManager.sceneCountInBuildSettings -1 > _currentLevel.buildIndex) _nextButton.interactable = false;
        Debug.Log($"{SceneManager.sceneCountInBuildSettings - 1 } и {_currentLevel.buildIndex}");
        Debug.Log($"{_currentLevel.buildIndex} и {PlayerPrefs.GetInt("levels", 1)}");
        if (_debugEnergyUI != null) _debugEnergyUI.text = "Fuel: " + Rocket.instance.EnergyTotal.ToString();
        if (_debugCurrentLevelUI != null) _debugCurrentLevelUI.text = _currentLevel.name;
        if (_currentLevelUI != null) _currentLevelUI.text = _currentLevel.name;
        if (_requireTimeUI != null) _requireTimeUI.text = string.Format("{0:0.000}",_levelManager.Levels[_currentLevel.buildIndex-1].ThreeStarsTime);
        if (_debugRecordUI != null) _debugRecordUI.text = string.Format("Record time: {0:0.000}", PlayerPrefs.GetFloat("Level" + _currentLevel.buildIndex + "Record"));
        AudioManager.instance.Play("OST");
        AudioManager.instance.Play("Ambient");
        AudioManager.instance.SetVolume("OST", 0.33f);
        //Menu();
        //Time.timeScale = 100;
        //Screen.SetResolution(1920, 1080, true);
        //Screen.SetResolution(960, 540, true);
        foreach (var item in QualitySettings.names)
        {
            Debug.Log($"{item}");
        }
    }

    void Update()
    {
        if (Rocket.instance.Status == Rocket.State.NextLevel)
        {
            Finish();
        }
        if (Rocket.instance.Status == Rocket.State.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _menu != null)
            {
                IsMenuShown = !IsMenuShown;
                _menu.SetActive(IsMenuShown);
                Menu();
            }
        }
        if (Rocket.instance.Status == Rocket.State.Dead)
        {
            /*if (IsMenuShown)
            {
                IsMenuShown = true;
                _menu.SetActive(IsMenuShown);
            }*/
            StartCoroutine(Restart());
        }
        if (Debug.isDebugBuild)
        {
            DebugKeys();
        }
        Timer();
        StarsManagement();
    }

    private void Finish()
    {
        Debug.Log($"Заработано {StarsAchieved} звезд, заработано {_levelManager.Levels[_currentLevel.buildIndex].StarsAchieved}");
        IsMenuShown = true;
        _menu.SetActive(IsMenuShown);
        if (_currentLevel.buildIndex >= PlayerPrefs.GetInt("levels"))
        {
            if (SceneManager.sceneCountInBuildSettings - 1> _currentLevel.buildIndex) _nextButton.interactable = true;
            PlayerPrefs.SetInt("levels", _currentLevel.buildIndex + 1);
        }
        //Invoke("LoadNextLevel", 2f);
    }
    private IEnumerator Restart()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsMenuShown && _menu != null)
        {
            IsMenuShown = true;
            _menu.SetActive(IsMenuShown);
            Menu();
            //yield return null;
        }
        yield return new WaitForSeconds(1.8f);
        if (!IsMenuShown)
        {
            LoadCurrentLevel();
        }
    }
    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene(_currentLevel.buildIndex);
    }

    public void LoadNextLevel()
    {
        if (SceneManager.sceneCountInBuildSettings - 1 != _currentLevel.buildIndex)
            SceneManager.LoadScene(_currentLevel.buildIndex + 1);
        else SceneManager.LoadScene(0);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadPreviousLevel()
    {
        if (_currentLevel.buildIndex > 1)
        {
            SceneManager.LoadScene(_currentLevel.buildIndex - 1);
        }
    }

    public void Menu()
    {
        //IsMenuShown = !IsMenuShown;
        //_menu.SetActive(IsMenuShown);
        if (IsMenuShown)
        {
            Rocket.instance.StopAllCoroutines();
            AudioManager.instance.StopAllCoroutines();
            AudioManager.instance.Play("Click1");
            if (Rocket.instance.Status == Rocket.State.Playing) { /*AudioManager.instance.Stop("Hover", 5f);*/ StartCoroutine(AudioManager.instance.SmoothPitchDown("Hover", 0f, 0.4f)); }
            _velocity = Rocket.instance.Body.velocity;
            _angularVelocity = Rocket.instance.Body.angularVelocity;
            Rocket.instance.Body.constraints = RigidbodyConstraints.FreezeAll;
            Time.timeScale = 0;
            StartCoroutine(Stop());
        }
        else
        {
            Time.timeScale = 1;
            AudioManager.instance.StopAllCoroutines();
            this.StopAllCoroutines();
            AudioManager.instance.Play("Click1");
            if (Rocket.instance.Status == Rocket.State.Playing && Rocket.instance.EnergyTotal > 0)
            {
                AudioManager.instance.Play("Hover"); 
                StartCoroutine(AudioManager.instance.SmoothPitchUp("Hover", 1f, 0.2f));
            }
            Rocket.instance.Body.constraints = RigidbodyConstraints.None;
            Rocket.instance.Body.constraints = RigidbodyConstraints.FreezePositionZ;
            Rocket.instance.Body.constraints = RigidbodyConstraints.FreezeRotationX;
            Rocket.instance.Body.constraints = RigidbodyConstraints.FreezeRotationY;
            Rocket.instance.Body.velocity = _velocity;
            Rocket.instance.Body.angularVelocity = _angularVelocity;
            //Time.timeScale = 1f;
        }
    }
    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            LoadPreviousLevel();
        }
        if (Input.GetKeyDown(KeyCode.Tab) && _debug != null)
        {
            _isDebugShown = !_isDebugShown;
            _debug.SetActive(_isDebugShown);
            if (_isDebugShown) PlayerPrefs.SetInt("Debug", 1);
            else PlayerPrefs.SetInt("Debug", 0);
        }
    }
    private void Timer()
    {
        if (_timerUI != null)
        {
            if (_timer < 60f)
            {
                _timerUI.text = string.Format("{0:0.000}", _timer);
            }
            else if (_timer < 3600f)
            {
                string minutes = Convert.ToString(Convert.ToInt32(_timer / 60));
                string seconds = string.Format("{0:0.000}", (_timer % 60));
                if (Convert.ToDouble(seconds) < 10f) seconds = "0" + seconds;
                _timerUI.text = $"{minutes}:{seconds}";
            }
            else
            {
                _timerUI.text = "Seriously?";
            }
            switch (StarsAchieved)
            {
                case 3:
                    _timerUI.color = new Vector4(0.313f, 0.9f, 0.27f, 1);
                    break;
                case 2:
                    _timerUI.color = new Vector4(0.91f, 0.60f, 0.27f, 1);
                    break;
                default:
                    _timerUI.color = new Vector4(0.86f, 0, 0, 1);
                    break;
            }

        }
        if (Rocket.instance.Status == Rocket.State.Playing && !IsMenuShown)
        {
            _timer += Time.deltaTime;
            if (_debugTimerUI!=null) _debugTimerUI.text = string.Format("Timer: {0:0.000}", _timer);
        }
        if (Rocket.instance.Status == Rocket.State.NextLevel)
        {
            if (_timer < PlayerPrefs.GetFloat("Level" + _currentLevel.buildIndex + "Record") || PlayerPrefs.GetFloat("Level" + _currentLevel.buildIndex + "Record") == 0f)
            {
                _recordTime = _timer;
                PlayerPrefs.SetFloat("Level" + _currentLevel.buildIndex + "Record", _recordTime);
                if (_debugRecordUI != null) _debugRecordUI.text = string.Format("Record time: {0:0.000}", PlayerPrefs.GetFloat("Level" + _currentLevel.buildIndex + "Record"));
                Debug.Log($"New record: {PlayerPrefs.GetFloat("Level" + _currentLevel.buildIndex + "Record")}");
            }
        }
        if (_debugSpeed != null)_debugSpeed.text = Convert.ToString(string.Format("Speed: {0:0}", Rocket.instance.Body.velocity.magnitude));
        if (_debugEnergyUI != null) _debugEnergyUI.text = "Fuel: " + (Convert.ToInt32(Rocket.instance.EnergyTotal)).ToString();
    }
    private void StarsManagement()
    {
        int starsAchieved;
        _levelManager.CalculateStars(_timer, _levelManager.Levels[_currentLevel.buildIndex-1].ThreeStarsTime, _levelManager.Levels[_currentLevel.buildIndex - 1].TwoStarsTime,  out starsAchieved);
        for (int i = 0; i < _stars.transform.childCount; i++)
        {
            _stars.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
        }
        if (Rocket.instance.Status != Rocket.State.Dead)
        {
            for (int i = 0; i < starsAchieved; i++)
            {
                _stars.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else starsAchieved = 0;
        StarsAchieved = starsAchieved;
        _levelManager.Levels[_currentLevel.buildIndex].StarsAchieved = starsAchieved;
    }
    private IEnumerator Stop()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0.01f;
    }
}
