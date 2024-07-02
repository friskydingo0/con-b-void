using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// References
	[SerializeField]
	private Transform _shotPoint = null;

	[SerializeField]
	private Projectile _bulletObject = null;

	// Public parameters
	public float _MoveSpeed = 1.0f;

	// Internal stuff
	private bool isInitialized = false;
	private bool isShooting = false;


	// Start is called before the first frame update
	public void Init()
	{
		isInitialized = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (!isInitialized) return;

		float xMove = Input.GetAxisRaw("Horizontal");
		float yMove = Input.GetAxisRaw("Vertical");

		Vector3 targetPosition = transform.position;
		targetPosition.x = Mathf.Clamp(transform.position.x + xMove * _MoveSpeed * Time.deltaTime, GameManager.Instance.bounds.Left, GameManager.Instance.bounds.Right);
		targetPosition.y = Mathf.Clamp(transform.position.y + yMove * _MoveSpeed * Time.deltaTime, GameManager.Instance.bounds.Bottom, GameManager.Instance.bounds.Top);
		
		transform.position = targetPosition;

		if (Input.GetKeyDown(KeyCode.Space) && !isShooting)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		ProjectileManager.Instance.ShootProjectile(_shotPoint, true, ReturnShot);
		isShooting = true;
	}

	public void ReturnShot()
	{
		isShooting = false;
	}
}
