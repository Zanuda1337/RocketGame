
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

	float _rotationTime = 0;

	float _lastTime = 0;

	float _x;
	float _y;
	float _z;
	public static CameraShake Instance;
    public void Awake()
    {
        if (Instance == null)
        {
			Instance = this;
        }
		Initialize();
	}
    public void Start()
    {
		//Initialize();
	}
    public IEnumerator Shake(float strength = 5, float duration = 0.25f, float step = 0.08f)
    {
		Debug.Log("Множитель тряски: " + ShakeModifier);
		float pointTime = 0;
		while (_rotationTime < duration)
        {
			if (Time.time > _lastTime + step)
			{
				_x = Random.Range(-1f, 1f) * strength * ShakeModifier;
				_y = Random.Range(-1f, 1f) * strength * ShakeModifier;
				_z = Random.Range(-1f, 1f) * strength * ShakeModifier;
				_lastTime = Time.time;
				int i = 0;
				i++;
				Debug.Log($"Координата {i}:  {_x}, {_y}, {_z}");
				pointTime = 0;
			}

			Quaternion targetRotation = Quaternion.Euler(_x, _y, _z);
			_rotationTime += Time.deltaTime;
			pointTime += Time.deltaTime;
			if (_rotationTime < duration * 0.3f)
			{
				_smooth = _rotationTime;
			}
			else _smooth = Mathf.Sqrt(_rotationTime) * 0.8f;
			transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, pointTime * 0.25f);
			Debug.Log(transform.localRotation.eulerAngles);
			yield return null;
		}
        while (transform.localRotation != Quaternion.Euler(Vector3.zero))
        {
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), 10f * Time.deltaTime);
			yield return null;
		}
		transform.localRotation = Quaternion.Euler(Vector3.zero);
		_rotationTime = 0;
    }
	public void ShakeCaller()
    {
		StartCoroutine(Shake(3.8f, 0.25f, 0.08f));
    }
	public void kekmek()
    {

    }
	public void Initialize()
    {
		switch (PlayerPrefs.GetInt("Shake", 4))
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
