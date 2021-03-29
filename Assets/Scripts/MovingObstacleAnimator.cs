using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MovingObstacleAnimator : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private CameraShake _shaker;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private AudioSource _smashSound;
    [SerializeField] private AudioSource _startSound;
    [SerializeField] private AudioSource _slideSound;
    [SerializeField] private float _delay = 0;
    [SerializeField] private float _startDelay = 0;
    [SerializeField] private float _endDelay = 1;
    [SerializeField] private bool _isTrigger;
    [SerializeField] private bool _isPlaying;
    private void Start()
    {
        StartCoroutine(Delay());
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
    }
    public IEnumerator OnFirstAnimationEnd()
    {
        _isPlaying = true;
        float distance = (Rocket.instance.transform.position - transform.position).magnitude;
        float verticalDistance = Rocket.instance.transform.position.y - transform.position.y;
        if (distance < 80f && verticalDistance < 38 && verticalDistance > -30)
        {
            StartCoroutine(_shaker.Shake(3.5f - distance/22.5f));
            StartCoroutine(SceneController.instance.PostEffect(0.07f, 0.18f, 0.1f, 0.3f - distance/265));
        }
        _particles.Play();
        yield return new WaitForSeconds(_endDelay);
        _animator.SetTrigger("Up");
    }
    public IEnumerator OnSecondAnimationEnd()
    {
        if (!_isTrigger)
        {
            if (_startSound != null) PlayRandomPitch(_startSound);
            yield return new WaitForSeconds(_startDelay);
            _animator.SetTrigger("Down");
        }
        _isPlaying = false;
    }
    public IEnumerator Trigger()
    {
        if (!_isPlaying)
        {
            _isPlaying = true;
            if (_startSound != null) PlayRandomPitch(_startSound);
            yield return new WaitForSeconds(_startDelay);
            _animator.SetTrigger("Down");
        }
    }
    public void Smash()
    {
        //_smashSound.pitch = Random.Range(0.95f, 1.05f);
        //_smashSound.Play();
        if (_smashSound != null) PlayRandomPitch(_smashSound);
    }
    public void OnSecondAnimationStart()
    {
        _isPlaying = true;
        //_slideSound.pitch = Random.Range(0.98f, 1.02f);
        if (_slideSound != null) PlayRandomPitch(_slideSound);
        //_slideSound.Play();
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f + _delay);
        _animator.SetTrigger("Down");
    }
    private void PlayRandomPitch(AudioSource audio)
    {
            audio.pitch = Random.Range(0.95f, 1.02f);
            audio.Play();
    }
}
