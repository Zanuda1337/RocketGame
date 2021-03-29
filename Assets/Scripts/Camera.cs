using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private GameObject _rocket;
    private Vector3 _targetPosition;
    private Vector3 _smooth;
    private float _deltaZ;
    [SerializeField] [Range(0,10)] private float _smoothness;
    void Start()
    {
        _deltaZ = transform.position.z;
        _rocket = GameObject.FindGameObjectsWithTag("Player")[0];
        transform.position = new Vector3(_rocket.transform.position.x, _rocket.transform.position.y, transform.position.z);
    }

    void FixedUpdate()
    {
        _targetPosition = new Vector3(_rocket.transform.position.x, _rocket.transform.position.y, Mathf.Lerp(transform.position.z, _deltaZ - Rocket.instance.Body.velocity.magnitude * 0.3f, 2.5f * Time.deltaTime));
        _smooth = Vector3.Lerp(transform.position, _targetPosition, _smoothness * Time.deltaTime);
        transform.position = _smooth;
    }
}
