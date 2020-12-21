using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private GameObject _rocket;
    private Vector3 _targetPosition;
    private Vector3 _smooth;
    [SerializeField] [Range(0,10)] private float _smoothness;
    void Start()
    {
        _rocket = GameObject.FindGameObjectsWithTag("Player")[0];
        transform.position = new Vector3(_rocket.transform.position.x, _rocket.transform.position.y, transform.position.z);
    }

    void FixedUpdate()
    {
        _targetPosition = new Vector3(_rocket.transform.position.x, _rocket.transform.position.y, transform.position.z);
        _smooth = Vector3.Lerp(transform.position, _targetPosition, _smoothness * Time.deltaTime);
        transform.position = _smooth;
    }
}
