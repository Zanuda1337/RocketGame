using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private float _gravitation;
    private Vector3 _smooth;
    private void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.useGravity = false;
    }
    private void OnTriggerStay(Collider other)
    {
        _smooth = Vector3.Lerp(other.attachedRigidbody.position, transform.position, _gravitation * Time.fixedDeltaTime);
        other.attachedRigidbody.position = _smooth;
    }
    private void OnTriggerExit(Collider other)
    {
        other.attachedRigidbody.useGravity = true;
    }
}
