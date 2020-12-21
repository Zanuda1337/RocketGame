using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MovingObject : MonoBehaviour
{
    [SerializeField] private Vector3 _movePosition;
    [SerializeField] [Range(0,1)] private float _moveProgress;
    [SerializeField] private float _moveSpeed;
    private Vector3 _startPosition;
    private float t = 0.0f;
    void Start()
    {
        _startPosition = transform.position;
    }

    void FixedUpdate()
    {
        _moveProgress = Mathf.Cos(t * _moveSpeed);
        t += Time.deltaTime;
        Vector3 offset = _movePosition * _moveProgress;
        transform.position = _startPosition + offset;
    }
}
