using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IGameStateListener
{
	public static int MaxBullets {  get; private set; }
	private const int DefaultBullets = 1;

	// References
	[SerializeField]
	private Transform shotPoint = null;
	[SerializeField]
	private Transform modelRoot = null;
	[SerializeField]
	private Vector3 tiltOffset = Vector3.zero;

	// Public parameters
	public float _MoveSpeed = 1.0f;

	// Internal stuff
	private bool isInitialized = false;
	private Vector3 _tiltMin = Vector3.zero;
	private Vector3 _tiltMax = Vector3.zero;


	private void Awake()
	{
		_tiltMax = Vector3.zero - tiltOffset;
		_tiltMin = Vector3.zero + tiltOffset;
	}

	public void Init()
	{
		MaxBullets = DefaultBullets;
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

	void Update()
	{
		if (!isInitialized) return;

		float xMove = Input.GetAxisRaw("Horizontal");
		float yMove = Input.GetAxisRaw("Vertical");

		Vector3 targetPosition = transform.position;
		targetPosition.x = Mathf.Clamp(transform.position.x + xMove * _MoveSpeed * Time.deltaTime, GameManager.Instance.Bounds.Left, GameManager.Instance.Bounds.Right);
		targetPosition.y = Mathf.Clamp(transform.position.y + yMove * _MoveSpeed * Time.deltaTime, GameManager.Instance.Bounds.Bottom, GameManager.Instance.Bounds.Top);
		
		transform.position = targetPosition;

		float dir = Input.GetAxis("Horizontal");
		float t = Mathf.Clamp((1f + dir) / 2f, -1f, 1f);
		modelRoot.localEulerAngles = Vector3.Lerp(_tiltMin, _tiltMax, t);

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
			StartCoroutine(BlinkOnHit(2));
			
			isInitialized = false;
			GameManager.Instance.OnPlayerHit();
		}
	}

	IEnumerator BlinkOnHit(int timesToBlink)
	{
		int count = 0;
		while (count < timesToBlink)
		{
			modelRoot.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.5f);

			modelRoot.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			count++;
		}
	}

	private void Shoot()
	{
		ProjectileManager.Instance.ShootProjectile(shotPoint, true);
	}

	public void OnGameStateChanged(GameState fromState, GameState toState)
	{
		
	}
}
