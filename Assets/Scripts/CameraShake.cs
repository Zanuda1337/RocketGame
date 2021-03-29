
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	// use this script to control a rotation of an object, to turn and face a target object
	// Reference Unity's API: https://docs.unity3d.com/ScriptReference/Quaternion.Lerp.html
	Quaternion targetRotation;
	float step = 0.08f;
	private float _strength = 2f;
	private float _duration = 0.25f;
	public Transform target;
	public float speed = 1F;
	private float _smooth = 0;
	public float ShakeModifier = 1;

	bool rotating = false; 

	float rotationTime = 0;

	float lastTime = 0;

	float _x;
	float _y;
	float _z;
	public static CameraShake instance;
    public void Start()
    {
        if (instance == null)
        {
			instance = this;
        }
		Initialize();
    }
    public IEnumerator Shake(float strength = 2, float duration = 0.25f, float step = 0.08f)
    {
        while (rotationTime < duration)
        {
			if (Time.time > lastTime + step)
			{
				_x = Random.Range(-1, 1) * strength * ShakeModifier;
				_y = Random.Range(-1, 1) * strength * ShakeModifier;
				_z = Random.Range(-1, 1) * strength * ShakeModifier;
				lastTime = Time.time;
			}

			Quaternion targetRotation = Quaternion.Euler(_x, _y, _z);

			rotationTime += Time.deltaTime;
			if (rotationTime < duration * 0.3f)
			{
				_smooth = rotationTime;
			}
			else _smooth = Mathf.Sqrt(rotationTime) * 0.8f;
			transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, _smooth * 0.1f);
			yield return null;
		}
        while (transform.localRotation != Quaternion.Euler(Vector3.zero))
        {
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), 10f * Time.deltaTime);
			yield return null;
        }
		transform.localRotation = Quaternion.Euler(Vector3.zero);
		rotationTime = 0;
    }
	private void Initialize()
    {
		switch (PlayerPrefs.GetInt("Shake"))
		{
			case 0:
				ShakeModifier = 0;
				break;
			case 1:
				ShakeModifier = 0.25f;
				break;
			case 2:
				ShakeModifier = 0.5f;
				break;
			case 3:
				ShakeModifier = 0.75f;
				break;
			case 4:
				ShakeModifier = 1;
				break;
		}
	}
}
