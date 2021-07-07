using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using UnityEngine.UI;
using System;

public class Rocket : MonoBehaviour
{
    public Rigidbody Body;

    [SerializeField] private KeyCode _rotateLeftKey;
    [SerializeField] private KeyCode _rotateRightKey;
    [SerializeField] private KeyCode _accelerateKey;

    public bool Accelerate = false;
    public bool TurnLeft = false;
    public bool TurnRight = false;

    [SerializeField] private float _damping = 3f;
    [SerializeField] private float _maxAngularMagnitude = 2.6f;
    [SerializeField] private float _angularForce = 22;
    [SerializeField] private float _maxAngle = 0.5f;
    [SerializeField] private float _rotatingAligmentSpeed = 2.2f;
    [SerializeField] private float _aligmentSpeed = 1.5f;
    [SerializeField] private float _thrustPower = 800;
    [SerializeField] private float _maxSpeed = 30f;
    [SerializeField] private bool _isAngleClamped;

    [SerializeField] private ParticleSystem _leftThrusterParticles;
    [SerializeField] private ParticleSystem _rightThrusterParticles;
    [SerializeField] private ParticleSystem _deathParticles;
    [SerializeField] private ParticleSystem _finishParticles;

    [SerializeField] public float EnergyTotal;
    [SerializeField] private int _energyConsuption;

    /*[SerializeField]*/ private Renderer _ufoPart;
    /*[SerializeField]*/ private Light _light;

    /*[SerializeField]*/ private GameObject _indicators;

    public SceneController SceneController;
    //public CameraShake _shaker;

    private bool _isWarning = false;
    private bool _isTankEmpty = false;
    private bool _godModeEnabled = false;
    private bool _noClip = false;
    public bool IsTrapped = false;
    private GameObject _skin;
    private Color _defaultColor;

    public State Status = State.Playing;


    public static Rocket instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        SkinSelection();
        initializeModificators();
        _defaultColor = _ufoPart.material.color;
        Status = State.Playing;
        Body = GetComponent<Rigidbody>();
        SceneController = FindObjectOfType<SceneController>();
        //_ufoPart.material.color = new Vector4(0, ((EnergyTotal * 0.01f) + 1) / 2, ((EnergyTotal * 0.01f) + 1) / 2, 1);
        //_ufoPart.material.SetVector("_EmissionColor", new Vector4(0, EnergyTotal * 0.01f, EnergyTotal * 0.01f, 1f));
        AudioManager.instance.Play("Hover");
        AudioManager.instance.SetVolume("Hover", 0f);
        AudioManager.instance.SetPitch("Hover", 0f);
        StartCoroutine(AudioManager.instance.SmoothVolumeUp("Hover", 1f, 2.5f));
        //StartCoroutine(AudioManager.instance.SmoothPitchUp("Hover", 1f, 1f));
    }

    void FixedUpdate()
    {
        if (Status == State.Playing && !SceneController.IsMenuShown)
        {
            if (!_isTankEmpty) Thrust();
            Rotation();
        }
        /*if (Debug.isDebugBuild && !SceneController.IsMenuShown)
        {
            DebugKeys();
        }*/
        Indicators();
    }
    private void Update()
    {
        if (Debug.isDebugBuild && !SceneController.IsMenuShown)
        {
            DebugKeys();
        }
    }
    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            _godModeEnabled = !_godModeEnabled;
            if (_godModeEnabled)
            {
                SetColor(_ufoPart, new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1));
                SetLightIntesity(2f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            _noClip = !_noClip;
            if (_noClip) Body.detectCollisions = false;
            else Body.detectCollisions = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Status != State.Playing && Status != State.Restart && Status != State.NextLevel && Status != State.Finish || _godModeEnabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                Finish();
                break;
            default:
                Lose();
                break;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Battery")
        {
            AudioManager.instance.Play("PickUp");
            PowerUp(100f, collision.gameObject);
        }
        if (collision.gameObject.tag == "Finish" && Status == State.Playing)
        {
            Finish();
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            StartCoroutine(collision.GetComponentInChildren<MovingObstacleAnimator>().Trigger());
        }
    }
    private void Lose()
    {
        Status = State.Dead;
        AudioManager.instance.Stop("Hover", 0.1f);
        AudioManager.instance.Play("Boom");
        _leftThrusterParticles.Stop();
        _rightThrusterParticles.Stop();
        _deathParticles.Play();
        Body.freezeRotation = false;
        SetColor(_ufoPart, new Vector4(0.35f, 0.25f, 0.25f, 1), new Vector4(0, 0, 0, 1));
        SetLightIntesity(0f);
        StartCoroutine(CameraShake.Instance.Shake(7f, 0.3f, 0.08f));
    }
    private void Finish()
    {
        Status = State.Finish;
        Body.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), 1.5f * Time.deltaTime);
        AudioManager.instance.Play("Finish");
        AudioManager.instance.SetPitch("Hover", 0.85f);
        StartCoroutine(AudioManager.instance.SmoothVolumeDown("Hover", 0.2f, 1f));
        _leftThrusterParticles.Stop();
        _rightThrusterParticles.Stop();
        _finishParticles.Play();
    }
    private void Thrust()
    {
        if (Input.GetKey(_accelerateKey) || Accelerate && EnergyTotal>0)
        {
            Body.AddRelativeForce(Vector3.up * _thrustPower * Time.deltaTime);
            if (_rightThrusterParticles.isStopped && _leftThrusterParticles.isStopped)
            {
                _rightThrusterParticles.Play();
                _leftThrusterParticles.Play();
            }

            if (!_godModeEnabled)
            {
                FuelSpending();
            }
            AudioManager.instance.Play("Hover");
            AudioManager.instance.SetVolume("Hover", 1f);
        }
        else
        {
            if (EnergyTotal <= 0)
            {
                AudioManager.instance.Stop("Hover", 0f);
                AudioManager.instance.Play("Empty");
                SetColor(_ufoPart, new Vector4(0.25f, 0, 0, 1), new Vector4(0.25f, 0, 0, 1));
                _isTankEmpty = true;
            }
            _leftThrusterParticles.Stop();
            _rightThrusterParticles.Stop();
            if (EnergyTotal > 0)
            {
                StartCoroutine(AudioManager.instance.SmoothPitchDown("Hover", 0.95f, 5f, 0.8f, 1.35f));
            }
        }
        Body.velocity = Vector3.ClampMagnitude(Body.velocity, _maxSpeed);
    }
    private void Rotation()
    {

        float deltaZ = _angularForce * Time.deltaTime;

        if (Input.GetKey(_rotateLeftKey) || TurnLeft)
        {
            Body.angularVelocity += new Vector3(0, 0, deltaZ);
            if (_isAngleClamped) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), _rotatingAligmentSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(_rotateRightKey) || TurnRight)
        {
            Body.angularVelocity += new Vector3(0, 0, -deltaZ);
            if (_isAngleClamped) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), _rotatingAligmentSpeed * Time.deltaTime); 
        }
        else if (_isAngleClamped) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), _aligmentSpeed * Time.deltaTime);
        if (_isAngleClamped)
        {
            Body.transform.rotation = new Quaternion(0, 0, Mathf.Clamp(Body.transform.rotation.z, -_maxAngle, _maxAngle), Body.transform.rotation.w);
            Body.drag = 0.5f - Mathf.Abs(Body.transform.rotation.z) / 1.5f;
        }
        Body.angularVelocity = Vector3.ClampMagnitude(Body.angularVelocity, _maxAngularMagnitude);
        Body.angularVelocity = Vector3.Lerp(Body.angularVelocity, Vector3.zero, _damping * Time.deltaTime);
    }
    private void PowerUp(float energy, GameObject battery)
    {
        EnergyTotal += energy;
        if (EnergyTotal > 100)
        {
            EnergyTotal = 100;
        }
        if (Status != State.Dead)
        {
            if (_isTankEmpty)
            {
                _isTankEmpty = false;
                AudioManager.instance.Play("Hover");
            }
            if (_isWarning)
            {
                _isWarning = false;
            }
            SetColor(_ufoPart, Color.Lerp(new Vector4(0.05f, 0.05f, 0.05f, 1), _defaultColor, EnergyTotal * 0.01f), Color.Lerp(new Vector4(0.05f, 0.05f, 0.05f, 1), _defaultColor, EnergyTotal * 0.01f));
            //_ufoPart.material.color = new Vector4(0, ((EnergyTotal * 0.01f) + 1) / 2, ((EnergyTotal * 0.01f) + 1) / 2, 1);
            //_ufoPart.material.SetVector("_EmissionColor", new Vector4(0, EnergyTotal * 0.01f, EnergyTotal * 0.01f, 1f));
            SetLightIntesity(2);
        }
        Destroy(battery);
    }
    private void SetColor(Renderer renderer, Vector4 colorSaturation, Vector4 emissionSaturation)
    {
        if (renderer!=null)
        {
            renderer.material.color = colorSaturation;
            renderer.material.SetVector("_EmissionColor", emissionSaturation);
        }
    }
    private void SetLightIntesity(float argument)
    {
        if (_light!=null)
        {
            _light.intensity = argument;
        }
    }
    private void FuelSpending()
    {
        EnergyTotal -= _energyConsuption * Time.deltaTime;
        if (EnergyTotal >= 10)
        {
            float exponent = 0.01f;
            float colorSaturation = ((EnergyTotal * exponent) + 1) / 2; //Линейная функция y=2x-100 (при 100% топлива насыщенность = 100%, при 0% топлива насыщенность = 50%)
            float emissionSaturation = EnergyTotal * exponent; //Линейная функция y=x
            SetColor(_ufoPart, Color.Lerp(new Vector4(0.05f, 0.05f, 0.05f, 1), _defaultColor, EnergyTotal * 0.01f), Color.Lerp(new Vector4(0.05f, 0.05f, 0.05f, 1), _defaultColor, EnergyTotal * 0.01f));
            Debug.Log(Color.Lerp(new Vector4(0.05f, 0.05f, 0.05f, 1), _defaultColor, EnergyTotal * 0.01f));

            float lightIntensity = (EnergyTotal / 50);
            SetLightIntesity(lightIntensity);
        }
        if (EnergyTotal >= 10)
        {
            StartCoroutine(AudioManager.instance.SmoothPitchUp("Hover", 1.5f - (Body.velocity.magnitude / 50), 5f, 0.8f, 1.35f));
            StartCoroutine(AudioManager.instance.SmoothPitchDown("Hover", 1.5f - (Body.velocity.magnitude / 50), 5f, 0.8f, 1.35f));
        }
        else if (!_isWarning)
        {
            AudioManager.instance.Play("Warning");
            SetColor(_ufoPart, new Vector4(0.5f, 0, 0, 1), new Vector4(0.5f, 0, 0, 1));
            _isWarning = true;
        }
        else StartCoroutine(AudioManager.instance.SmoothPitchDown("Hover", 0.75f, 7.5f));
    }

    private void Indicators()
    {
        Color inactiveColor = new Vector4(0.08f, 0.08f, 0.08f, 1);
        float exponent = 0.01f;
        float saturation = ((EnergyTotal * exponent) + 1.5f) / 2.5f;
        for (int i = _indicators.transform.childCount - 1; i >= 0; i--)
        {
            if (_indicators.transform.GetChild(i).GetComponent<Renderer>().material.color != inactiveColor)
            SetColor(_indicators.transform.GetChild(i).GetComponent<Renderer>(), new Vector4(0, 0, 0, 1), inactiveColor);
        }
        for (int i = 0; i < SceneController.StarsAchieved; i++)
        {
            SetColor(_indicators.transform.GetChild(i).GetComponent<Renderer>(), new Vector4(saturation, saturation, saturation, 1), new Vector4(saturation, saturation, saturation, 1));
        }
    }
    private void initializeModificators()
    {
        _damping += PlayerPrefs.GetFloat("Damping", 0);
        _maxAngularMagnitude += PlayerPrefs.GetFloat("MaxAngularMagnitude", 0);
        _angularForce += PlayerPrefs.GetFloat("AngularForce", 0);
        _maxAngle += PlayerPrefs.GetFloat("MaxAngle", 0);
        _thrustPower += PlayerPrefs.GetFloat("ThrustPower", 0);
        if (PlayerPrefs.GetInt("IsAngleClamped", 1) == 1) _isAngleClamped = true;
        else _isAngleClamped = false;
        if (PlayerPrefs.GetInt("Indicators", 0) == 0) _indicators.SetActive(false);
        else _indicators.SetActive(true);

    }
    public void AccelerateButton(bool accelerate)
    {
        Accelerate = accelerate;
    }
    public void TurnLeftButton(bool turn)
    {
        TurnLeft = turn;
    }
    public void TurnRightButton(bool turn)
    {
        TurnRight = turn;
    }
    private void SkinSelection()
    {
        Debug.Log("Skin");
        for (int i = 0; i < Appearance.Instance.UFOs.Count; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        _skin = transform.GetChild(PlayerPrefs.GetInt("SelectedUFO", 0)).gameObject;
        _skin.SetActive(true);
        _ufoPart = _skin.transform.GetChild(0).transform.GetComponent<Renderer>();
        _indicators = _skin.transform.GetChild(1).gameObject;
        _light = _skin.transform.GetChild(2).transform.GetComponent<Light>();
    }
    public enum State {Playing, Dead, NextLevel, Restart, Finish};
}
