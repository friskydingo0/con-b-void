using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	[SerializeField]
	Vector3 rotationVector = Vector3.zero;
	[SerializeField]
	float animDuration = 1.0f;

	private float _animTimer = 0f;

	// Start is called before the first frame update
	void Start()
	{
		Vector3 startAngles = Vector3.zero + rotationVector;
		Vector3 targetAngles = Vector3.zero - rotationVector;

		Debug.LogFormat("{0} | {1}", startAngles, targetAngles);
	}

	// Update is called once per frame
	void Update()
	{
		// Input
		float dir = Input.GetAxis("Horizontal");

		Vector3 startAngles = Vector3.zero;
		Vector3 targetAngles = Vector3.zero;
		//if (dir != 0)
		{
			startAngles = Vector3.zero + rotationVector;
			targetAngles = Vector3.zero - rotationVector;

			
			float t = (1f + dir) / 2f;
			//Debug.Log(t);
			transform.localEulerAngles = Vector3.Lerp(startAngles, targetAngles, t);
		}
		
		

		/*
		if (dir != 0)
		{
			Vector3 startAngles = Vector3.zero;
			Vector3 targetAngles = startAngles + dir * rotationVector;

			StartCoroutine(DoRotation(startAngles, targetAngles, animDuration));
		}
		else
		{
			Vector3 startAngles = transform.localEulerAngles;
			Vector3 targetAngles = Vector3.zero;

			StartCoroutine(DoRotation(startAngles, targetAngles, animDuration));
		}

		*/
	}

	IEnumerator DoRotation(Vector3 startAngles, Vector3 targetAngles, float duration)
	{
		float elapsed = 0f;

		while (elapsed < duration)
		{
			transform.localEulerAngles = Vector3.Slerp(startAngles, targetAngles, elapsed / duration);
			yield return null;
			elapsed += Time.deltaTime;
		}
	}
}
