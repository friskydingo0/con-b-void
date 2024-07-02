using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour
{
    // Preset parameters
    public float _Speed = 1f;
	private System.Action _hitCallback = null;
	public bool IsPlayerShot = false;

	public void Initialize(float speed, bool isplayerShot, System.Action hitCallback)
	{
		_hitCallback = hitCallback;
		IsPlayerShot = isplayerShot;
	}

	private void Update()
	{
		transform.position += transform.forward * Time.deltaTime * _Speed;
		if (transform.position.z > GameManager.Instance.bounds.Far)
		{
			Recycle();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Recycle();
	}

	private void Recycle()
	{
		Assert.IsNotNull(_hitCallback);
		_hitCallback?.Invoke();
		ProjectileManager.Instance.ReturnProjectile(this);
	}
}
