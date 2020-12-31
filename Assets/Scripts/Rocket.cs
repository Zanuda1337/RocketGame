using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using UnityEngine.UI;
using System;

public class Rocket : MonoBehaviour
{
    private Rigidbody _body;
    private AudioSource _audioSource;
    [SerializeField] private KeyCode _rotateLeftKey;
    [SerializeField] private KeyCode _rotateRightKey;
    [SerializeField] private KeyCode _accelerateKey;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _thrustPower;
    [SerializeField] private AudioClip _flySound;
    [SerializeField] private AudioClip _boomSound;
    [SerializeField] private AudioClip _finishSound;
    [SerializeField] private AudioClip _batteryPicking;
    [SerializeField] private ParticleSystem _leftThrusterParticles;
    [SerializeField] private ParticleSystem _rightThrusterParticles;
    [SerializeField] private ParticleSystem _deathParticles;
    [SerializeField] private ParticleSystem _finishParticles;
    [SerializeField] float _energyTotal;
    [SerializeField] int _energyConsuption;
    [SerializeField] Text _energyText;
    private bool _godModeEnabled = false;
    private bool _noClip = false;
    void Start()
    {
        if (_energyText != null)
        {
        _energyText.text = _energyTotal.ToString();
        }
        _state = State.Playing;
        _body = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Menu();
        if (_state == State.Playing)
        {
            Thrust();
            Rotation();
        }
        if (_state != State.NextLevel)
        {
            Restart();
        }
        if (Debug.isDebugBuild)
        {
            DebugKeys();
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
        else if (Input.GetKeyDown(KeyCode.G))
        {
            _godModeEnabled = !_godModeEnabled;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            _noClip = !_noClip;
            if (_noClip)
            {
                _body.detectCollisions = false;
            }
            else
            {
                _body.detectCollisions = true;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_state != State.Playing || _godModeEnabled)
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
            _audioSource.PlayOneShot(_batteryPicking);
            PowerUp(100f, collision.gameObject);
        }
        if (collision.gameObject.tag == "Finish" && _state == State.Playing)
        {
            Finish();
        }
    }
    private void Lose()
    {
        _state = State.Dead;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_boomSound);
        _leftThrusterParticles.Stop();
        _rightThrusterParticles.Stop();
        _deathParticles.Play();
        _body.freezeRotation = false;
    }
    private void Finish()
    {
        _state = State.NextLevel;
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel >= PlayerPrefs.GetInt("levels"))
        {
            PlayerPrefs.SetInt("levels", currentLevel + 1);
        }
        _audioSource.Stop();
        _audioSource.PlayOneShot(_finishSound);
        _leftThrusterParticles.Stop();
        _rightThrusterParticles.Stop();
        _finishParticles.Play();
        Invoke("LoadNextLevel", 2f);
    }
    private void Thrust()
    {
        if (Input.GetKey(_accelerateKey) && _energyTotal > 0)
        {
            _body.AddRelativeForce(Vector3.up * _thrustPower * Time.deltaTime);
            if (_audioSource.isPlaying == false)
            {
                _audioSource.PlayOneShot(_flySound);
            }
            if (_rightThrusterParticles.isStopped && _leftThrusterParticles.isStopped)
            {
                    _rightThrusterParticles.Play();
                    _leftThrusterParticles.Play();
            }

            if (!_godModeEnabled)
            {
            _energyTotal -= _energyConsuption * Time.deltaTime;
            }
            if (_energyText != null)
            {
            _energyText.text = (Convert.ToInt32(_energyTotal)).ToString();
            }
        }
        else
        {
            _audioSource.Stop();
            _leftThrusterParticles.Stop();
            _rightThrusterParticles.Stop();
        }
    }
    private void Rotation()
    {

        float deltaZ = _rotationSpeed * Time.deltaTime;

        if (Input.GetKey(_rotateLeftKey))
        {
            _body.transform.Rotate(0, 0, deltaZ);
            _body.angularVelocity = Vector3.zero;
        }
        else if (Input.GetKey(_rotateRightKey))
        {
            _body.transform.Rotate(0, 0, -deltaZ);
            _body.angularVelocity = Vector3.zero;
        }
    }
    private void LoadPreviousLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
    private void LoadNextLevel()
    {
        if (SceneManager.sceneCountInBuildSettings - 1 != SceneManager.GetActiveScene().buildIndex)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
            SceneManager.LoadScene(0);
    }
    private void Restart()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadCurrentLevel();
        }
    }
    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void PowerUp(float energy, GameObject battery)
    {
        _energyTotal += energy;
        if (_energyTotal > 100)
        {
            _energyTotal = 100;
        }
        if (_energyText != null)
        {
        _energyText.text = Convert.ToInt32(_energyTotal).ToString();
        }
        Destroy(battery);
    }
    private void Menu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
    private enum State {Playing, Dead, NextLevel};
    private State _state = State.Playing;
}
