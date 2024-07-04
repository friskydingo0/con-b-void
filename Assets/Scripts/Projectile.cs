using UnityEngine;

public class Projectile : MonoBehaviour, IRevivalListener
{
    // Preset parameters
    public float _Speed = 1f;
	public bool IsPlayerShot = false;

	public void Initialize(float speed, bool isplayerShot)
	{
		GameManager.Instance.OnPlayerRevival += OnPlayerRevival;
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
		Recycle();
	}

	public void Recycle()
	{
		GameManager.Instance.OnPlayerRevival -= OnPlayerRevival;
		ProjectileManager.Instance.ReturnProjectile(this);
	}

	public void OnPlayerRevival(bool isDone)
	{
		if (!isDone)
			Recycle();
	}
}
