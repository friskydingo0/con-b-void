using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public int LivesLeft {  get; private set; }
	public const int MaxLives = 3;
	public static int MaxBullets {  get; private set; }
	private const int DefaultBullets = 1;

	// References
	[SerializeField]
	private Transform _shotPoint = null;

	[SerializeField]
	private Projectile _bulletObject = null;

	// Public parameters
	public float _MoveSpeed = 1.0f;

	// Internal stuff
	private bool isInitialized = false;


	public void Init()
	{
		MaxBullets = DefaultBullets;
		LivesLeft = MaxLives;
		isInitialized = true;
	}

	public void ReviveAtPosition(Vector3 revivePos)
	{
		transform.position = revivePos;
		isInitialized = true;
	}

	/// <summary>
	/// Unused. Use this to increase the no. of shots the player can take before having to wait. Default = 1
	/// </summary>
	/// <param name="maxBullets"></param>
	public void UpdateShootingCapacity(int maxBullets)
	{
		MaxBullets = maxBullets;
	}

	// Update is called once per frame
	void Update()
	{
		if (!isInitialized) return;

		float xMove = Input.GetAxisRaw("Horizontal");
		float yMove = Input.GetAxisRaw("Vertical");

		Vector3 targetPosition = transform.position;
		targetPosition.x = Mathf.Clamp(transform.position.x + xMove * _MoveSpeed * Time.deltaTime, GameManager.Instance.Bounds.Left, GameManager.Instance.Bounds.Right);
		targetPosition.y = Mathf.Clamp(transform.position.y + yMove * _MoveSpeed * Time.deltaTime, GameManager.Instance.Bounds.Bottom, GameManager.Instance.Bounds.Top);
		
		transform.position = targetPosition;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Shoot();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isInitialized) return;

		if (other.CompareTag("EnemyBullet"))
		{
			// Play particle effect
			
			isInitialized = false;
			LivesLeft--;

			if (LivesLeft <= 0)
			{
				// Game over
			}
			else
			{
				GameManager.Instance.RevivePlayer();
			}
		}
	}

	private void Shoot()
	{
		ProjectileManager.Instance.ShootProjectile(_shotPoint, true);
	}
}
