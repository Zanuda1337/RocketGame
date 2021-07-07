using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RotatingObject : MonoBehaviour
{
    [SerializeField] private Vector3 _moveRotation;
    void Update()
    {
        Vector3 offset = _moveRotation * Time.time;
        transform.localEulerAngles = offset;
    }
}
