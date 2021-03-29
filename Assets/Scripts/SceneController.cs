using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Audio;

public class SceneController : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _mixer;
    private float _volume;
    [SerializeField] private Text _debugCurrentLevelUI;
    [SerializeField] private Text _debugEnergyUI;
    [SerializeField] private Text _debugTimerUI;
    [SerializeField] private Text _debugRecordUI;
    [SerializeField] private Text _debugSpeed;

    [SerializeField] private TextMeshProUGUI _currentLevelUI;
    [SerializeField] private TextMeshProUGUI _currentLevelUI2;
    [SerializeField] private GameObject _stars;
    [SerializeField] private TextMeshProUGUI _requireTimeUI;
    [SerializeField] private TextMeshProUGUI _timerUI;
    [SerializeField] private TextMeshProUGUI _requireTimeUI2;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _finishButton;
    [SerializeField] private Animator _finishStars;

    [SerializeField] private GameObject _debug;
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _intro;

    [SerializeField] private LevelManager _levelManager;
    [HideInInspector] public int StarsAchieved;
    private bool _isDebugShown;
    private bool _deathTrigger = false;
    public bool IsMenuShown = false;
    private float _timer = -1f;
    private float _recordTime;
    private float _timeLeft;
    private Vector3 _velocity, _angularVelocity;
    private Scene _currentLevel;
    [SerializeField] private Button _nextButton;
    [SerializeField] private PostProcessVolume _postProcess;
    private ChromaticAberration _chromaticAberration;


    public static SceneController instance;
    private float _transitionTime = 1.25f;

    private void Awake()
    {
        _mixer.audioMixer.SetFloat("MasterVolume", -40);
        if (instance == null)
        {
            instance = this;
        }
        Time.timeScale = 0.001f;
    }
    void Start()
    {
        //Time.timeScale = 1f;
        _animator.SetTrigger("End");
        //_volume = -40f;
        StartCoroutine(SmoothVolumeUp(0.6f));
        if (PlayerPrefs.GetInt("Debug") == 1) { _isDebugShown = true; _debug.SetActive(true); }
        else { _isDebugShown = false; _debug.SetActive(false); }
        _postProcess.profile.TryGetSettings(out _chromaticAberration);
        _nextButton.interactable = false;
        _currentLevel = SceneManager.GetActiveScene();
        if (_currentLevel.buildIndex < PlayerPrefs.GetInt("levels", 1) && SceneManager.sceneCountInBuildSettings - 1> _currentLevel.buildIndex) _nextButton.interactable = true;
        //if (SceneManager.sceneCountInBuildSettings -1 > _currentLevel.buildIndex) _nextButton.interactable = false;
        float _timeLeft = _levelManager.Levels[_currentLevel.buildIndex - 1].ThreeStarsTime;
        if (_debugEnergyUI != null) _debugEnergyUI.text = "Fuel: " + Rocket.instance.EnergyTotal.ToString();
        if (_debugCurrentLevelUI != null) _debugCurrentLevelUI.text = _currentLevel.name;
        if (_currentLevelUI != null) _currentLevelUI.text = _currentLevel.name;
        if (_currentLevelUI2 != null) _currentLevelUI2.text = _currentLevel.name;
        if (_requireTimeUI != null) _requireTimeUI.text = string.Format("{0:0.000}",_levelManager.Levels[_currentLevel.buildIndex-1].ThreeStarsTime);
        if (_debugRecordUI != null) _debugRecordUI.text = string.Format("Record time: {0:0.000}", PlayerPrefs.GetFloat("Level" + _currentLevel.buildIndex + "Record"));
        AudioManager.instance.Play("OST");
        AudioManager.instance.Play("Ambient");
        AudioManager.instance.SetVolume("OST", 0.33f);
        //AudioManager.instance.SetVolume("Hover", 0f);
        //StartCoroutine(AudioManager.instance.SmoothVolumeUp("Hover", 1f, 5f));
        //Menu();
        //Time.timeScale = 100;
        //Screen.SetResolution(1920, 1080, true);
        //Screen.SetResolution(960, 540, true);
        foreach (var item in QualitySettings.names)
        {
            Debug.Log($"{item}");
        }
        Time.timeScale = 1f;
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
            if (!_deathTrigger)
            {
                StartCoroutine(PostEffect(0.07f, 0.18f, 0.12f, 0.65f));
                _deathTrigger = true;
                StartCoroutine(Restart());
            }
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
        _chromaticAberration.intensity.value = 0.5f;
        StartCoroutine(FinishAnimation());
        if (_currentLevel.buildIndex >= PlayerPrefs.GetInt("levels"))
        {
            if (SceneManager.sceneCountInBuildSettings - 1 > _currentLevel.buildIndex)
                StartCoroutine(NextLevelActive());
            //_nextButton.interactable = true;
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
        yield return new WaitForSeconds(0.5f);
        if (!IsMenuShown)
        {
            LoadCurrentLevel();
        }
    }
    public IEnumerator LoadScene(int levelIndex)
    {
        IsMenuShown = false;
        _menu.SetActive(IsMenuShown);
        Menu();
        _animator.SetTrigger("Start");
        StartCoroutine(SmoothVolumeDown(2f));
        yield return new WaitForSeconds(_transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
    public void LoadCurrentLevel()
    {
        Rocket.instance.Status = Rocket.State.Restart;
        /*if (Input.GetKeyDown(KeyCode.Escape) && !IsMenuShown && _menu != null)
        {
            IsMenuShown = true;
            _menu.SetActive(IsMenuShown);
            Menu();
            //yield return null;
        }
        else
        {
            IsMenuShown = false;
            _menu.SetActive(IsMenuShown);
            Menu();
        }*/
        //SceneManager.LoadScene(_currentLevel.buildIndex);
        StartCoroutine(LoadScene(_currentLevel.buildIndex));
    }

    public void LoadNextLevel()
    {
        if (SceneManager.sceneCountInBuildSettings - 1 != _currentLevel.buildIndex)
        {
            Rocket.instance.Status = Rocket.State.NextLevel;
            StartCoroutine(LoadScene(_currentLevel.buildIndex + 1));
            //SceneManager.LoadScene(_currentLevel.buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    public void LoadMenu()
    {
        Rocket.instance.Status = Rocket.State.Restart;
        StartCoroutine(LoadScene(0));
        //SceneManager.LoadScene(0);
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
            //AudioManager.instance.StopAllCoroutines();
            AudioManager.instance.Play("Click1");
            if (Rocket.instance.Status == Rocket.State.Playing) { /*AudioManager.instance.Stop("Hover", 5f);*/ StartCoroutine(AudioManager.instance.SmoothPitchDown("Hover", 0f, 0.4f)); }
            _chromaticAberration.intensity.value = 0.5f;
            _intro.SetActive(false);
            Time.timeScale = 0.00000001f;
        }
        else
        {
            Time.timeScale = 1;
            _chromaticAberration.intensity.value = 0;
            //AudioManager.instance.StopAllCoroutines();
            //this.StopAllCoroutines();
            if (Rocket.instance.Status == Rocket.State.Playing && Rocket.instance.EnergyTotal > 0)
            {
                AudioManager.instance.Play("Click1");
                AudioManager.instance.Play("Hover");
                AudioManager.instance.SetVolume("Hover", 1f);
                StartCoroutine(AudioManager.instance.SmoothPitchUp("Hover", 1f, 0.3f));
            }
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
            if (_timer < 0)
            {
                _timerUI.text = "0,000";
            }
            else if (_timer < 60f)
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
        if (_requireTimeUI2 != null)
        {
            if (_timer < 0)
            {
                _requireTimeUI2.text = string.Format("{0:0.000}", _levelManager.Levels[_currentLevel.buildIndex - 1].ThreeStarsTime);
            }
            else
            {
                _timeLeft = _levelManager.Levels[_currentLevel.buildIndex-1].ThreeStarsTime - _timer;
                _requireTimeUI2.text = string.Format("{0:0.000}", _timeLeft);
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
        if (Rocket.instance.Status != Rocket.State.Dead && Rocket.instance.Status != Rocket.State.Restart)
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
    public IEnumerator PostEffect(float fadeInSpeed, float fadeOutSpeed, float duration, float maxValue = 1f)
    {
        float value = 0;
        //Debug.Log($"Время до {Time.time}");
        while (value < maxValue)
        {
                
            value += Time.deltaTime / fadeInSpeed;
            _chromaticAberration.intensity.value = value;
            yield return null;
        }
        //Debug.Log($"Время после {Time.time}");
        yield return new WaitForSeconds(duration);
        while (value>0)
        {
            value -= Time.deltaTime / fadeOutSpeed;
            _chromaticAberration.intensity.value = value;
            yield return null;
        }
        //Debug.Log(_chromaticAberration.intensity.value);
    }
    public IEnumerator SmoothVolumeUp(float time = 0f)
    {
        _volume = -40f;
        while (_volume < 0)
        {
            _volume += Time.unscaledDeltaTime / time * 40f;
            _volume = Mathf.Clamp(_volume, -40, 0);
            _mixer.audioMixer.SetFloat("MasterVolume", _volume);
            yield return null;
        }
    }
    public IEnumerator SmoothVolumeDown(float time = 0f)
    {
        _volume = 0;
        while (_volume > -40)
        {
            _volume -= Time.unscaledDeltaTime / time * 40f;
            _volume = Mathf.Clamp(_volume, -40, 0);
            _mixer.audioMixer.SetFloat("MasterVolume", _volume);
            yield return null;
        }
    }
    public IEnumerator NextLevelActive()
    {
        _finishButton.SetTrigger("Finish");
        yield return new WaitForSeconds(0.15f);
        _nextButton.interactable = true;
    }
    public IEnumerator FinishAnimation()
    {
        switch (StarsAchieved)
        {
            case 1:
                _finishStars.SetTrigger("1");
                break;
            case 2:
                _finishStars.SetTrigger("2");
                break;
            case 3:
                _finishStars.SetTrigger("3");
                break;
            default:
                break;
        }
        yield return null;
    }
}
