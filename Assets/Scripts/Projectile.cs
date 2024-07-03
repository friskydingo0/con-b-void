using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour, IRevivalListener
{
    // Preset parameters
    public float _Speed = 1f;
	private System.Action _hitCallback = null;
	public bool IsPlayerShot = false;

	public void Initialize(float speed, bool isplayerShot, System.Action hitCallback)
	{
		GameManager.Instance.OnPlayerRevival += OnPlayerRevival;
		_hitCallback = hitCallback;
		IsPlayerShot = isplayerShot;
	}

	private void Update()
	{
		transform.position += transform.forward * Time.deltaTime * _Speed;
		if (transform.position.z > GameManager.Instance.Bounds.Far || transform.position.z < GameManager.Instance.Bounds.Near)
		{
			Recycle();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		_hitCallback?.Invoke();
		Recycle();
	}

	public void Recycle()
	{
		GameManager.Instance.OnPlayerRevival -= OnPlayerRevival;
		ProjectileManager.Instance.ReturnProjectile(this);
	}

	public void OnPlayerRevival()
	{
		Recycle();
	}
}
