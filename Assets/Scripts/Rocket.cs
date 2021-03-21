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

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _thrustPower;

    [SerializeField] private ParticleSystem _leftThrusterParticles;
    [SerializeField] private ParticleSystem _rightThrusterParticles;
    [SerializeField] private ParticleSystem _deathParticles;
    [SerializeField] private ParticleSystem _finishParticles;

    [SerializeField] public float EnergyTotal;
    [SerializeField] private int _energyConsuption;

    [SerializeField] private Renderer _ufoPart;
    [SerializeField] private Light _light;

    [SerializeField] private GameObject _indicators;

    public SceneController SceneController;

    private bool _isWarning = false;
    private bool _isTankEmpty = false;
    private bool _godModeEnabled = false;
    private bool _noClip = false;

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
        Status = State.Playing;
        Body = GetComponent<Rigidbody>();
        SceneController = FindObjectOfType<SceneController>();
        _ufoPart.material.color = new Vector4(0, ((EnergyTotal * 0.01f) + 1) / 2, ((EnergyTotal * 0.01f) + 1) / 2, 1);
        _ufoPart.material.SetVector("_EmissionColor", new Vector4(0, EnergyTotal * 0.01f, EnergyTotal * 0.01f, 1f));
        AudioManager.instance.Play("Hover");
        AudioManager.instance.SetVolume("Hover", 0f);
        AudioManager.instance.SetPitch("Hover", 0f);
        StartCoroutine(AudioManager.instance.SmoothVolumeUp("Hover", 1f, 0.5f));
        StartCoroutine(AudioManager.instance.SmoothPitchUp("Hover", 1f, 0.3f));
    }

    void Update()
    {
        if (Status == State.Playing && !SceneController.IsMenuShown)
        {
            if (!_isTankEmpty) Thrust();
            Rotation();
        }
        if (Debug.isDebugBuild && !SceneController.IsMenuShown)
        {
            DebugKeys();
        }
        Indicators();
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
        if (Status != State.Playing || _godModeEnabled)
        {
            return;
        }

        switch(collision.gameObject.tag)
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
    private void Lose()
    {
        Status = State.Dead;
        AudioManager.instance.Stop("Hover", 0.1f);
        AudioManager.instance.Play("Boom");
        _leftThrusterParticles.Stop();
        _rightThrusterParticles.Stop();
        _deathParticles.Play();
        Body.freezeRotation = false;
        SetColor(_ufoPart, new Vector4(0.35f,0.25f,0.25f,1), new Vector4(0, 0, 0, 1));
        SetLightIntesity(0f);
    }
    private void Finish()
    {
        Status = State.NextLevel;
        AudioManager.instance.Play("Finish");
        AudioManager.instance.SetPitch("Hover", 0.85f);
        StartCoroutine(AudioManager.instance.SmoothVolumeDown("Hover", 0.2f, 1f));
        _leftThrusterParticles.Stop();
        _rightThrusterParticles.Stop();
        _finishParticles.Play();
    }
    private void Thrust()
    {
        if (Input.GetKey(_accelerateKey) && EnergyTotal>0)
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
                StartCoroutine(AudioManager.instance.SmoothPitchDown("Hover", 0.95f, 5f));
            }
        }
    }
    private void Rotation()
    {

        float deltaZ = _rotationSpeed * Time.deltaTime;

        if (Input.GetKey(_rotateLeftKey))
        {
            Body.transform.Rotate(0, 0, deltaZ);
            Body.angularVelocity = Vector3.zero;
        }
        else if (Input.GetKey(_rotateRightKey))
        {
            Body.transform.Rotate(0, 0, -deltaZ);
            Body.angularVelocity = Vector3.zero;
        }
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
            _ufoPart.material.color = new Vector4(0, ((EnergyTotal * 0.01f) + 1) / 2, ((EnergyTotal * 0.01f) + 1) / 2, 1);
            _ufoPart.material.SetVector("_EmissionColor", new Vector4(0, EnergyTotal * 0.01f, EnergyTotal * 0.01f, 1f));
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
            SetColor(_ufoPart, new Vector4(0, colorSaturation, colorSaturation, 1), new Vector4(0, emissionSaturation, emissionSaturation, 1));

            float lightIntensity = (EnergyTotal / 50);
            SetLightIntesity(lightIntensity);
        }
        if (EnergyTotal >= 10)
        {
            StartCoroutine(AudioManager.instance.SmoothPitchUp("Hover", 1.5f - (Body.velocity.magnitude / 50), 5f));
            StartCoroutine(AudioManager.instance.SmoothPitchDown("Hover", 1.5f - (Body.velocity.magnitude / 50), 5f));
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
        float exponent = 0.01f;
        float saturation = ((EnergyTotal * exponent) + 1.5f) / 2.5f;
        for (int i = 0; i < _indicators.transform.childCount; i++)
        {
            SetColor(_indicators.transform.GetChild(i).GetComponent<Renderer>(), new Vector4(0, 0, 0, 1), new Vector4(0.2f, 0.2f, 0.2f, 1));
        }
        for (int i = 0; i < SceneController.StarsAchieved; i++)
        {
            SetColor(_indicators.transform.GetChild(i).GetComponent<Renderer>(), new Vector4(saturation, saturation, saturation, 1), new Vector4(saturation, saturation, saturation, 1));
        }
    }

    public enum State {Playing, Dead, NextLevel};
}
