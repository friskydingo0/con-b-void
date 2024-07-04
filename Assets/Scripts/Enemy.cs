using System.Collections;
using UnityEngine;

public enum EnemyType
{
	Easy = 0,
	Medium,
	Hard
}

public class Enemy : MonoBehaviour, IRevivalListener
{
	public EnemyType enemyType;
	public int points = 0;
	public float shotInterval = 1.5f;

	private const float _MoveDuration = 0.2f;	// For smoothing. Shouldn't be higher than the fastest move interval (from the EnemyManager)

	[SerializeField, Range(0.1f, 3f)]
	private float MoveOffset = 0.25f;

	[SerializeField]
	private Transform shotSpawn = null;
	private Transform _modelRoot = null;

	[SerializeField]
	private Vector3 _frontTilt = Vector3.zero;
	[SerializeField]
	private Vector3 _sideTilt = Vector3.zero;

	private float _shotTimer = 0f;

	private bool _isInitComplete = false;

	private void Awake()
	{
		_modelRoot = transform.Find("ModelRoot");
	}

	public void Init()
	{
		GameManager.Instance.OnPlayerRevival += OnPlayerRevival;
		EnemyManager.Instance.MoveEvent += Move;
		_shotTimer = 0f;
		_isInitComplete = true;
	}

	private void Move(int dir, bool shouldSmooth = true)
	{
		if (shouldSmooth)
		{
			StartCoroutine(SmoothTilt(dir));
			StartCoroutine(SmoothMove(dir));
		}
		else
		{
			Vector3 newPos = transform.position;
			newPos.x = dir != 0 ? newPos.x + (MoveOffset * dir) : newPos.x;
			newPos.z = dir != 0 ? newPos.z : newPos.z - MoveOffset;

			transform.position = newPos;
		}
	}

	IEnumerator SmoothMove(int dir)
	{
		//yield return new WaitForSeconds(0.2f); // #TODO : a delayed move like the original.

		Vector3 newPos = transform.position;
		newPos.x = dir != 0 ? newPos.x + (MoveOffset * dir) : newPos.x;
		newPos.z = dir != 0 ? newPos.z : newPos.z - MoveOffset;
		float t = 0;
		Vector3 startPos = transform.position;

		while (transform.position != newPos)
		{
			transform.position = Vector3.Lerp(startPos, newPos, t / _MoveDuration);
			t += Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator SmoothTilt(int dir)
	{
		Vector3 startAngles = Vector3.zero;
		Vector3 targetAngles = Vector3.zero;

		if (dir != 0)
		{
			startAngles = Vector3.zero;
			targetAngles = Vector3.zero + dir * _sideTilt;

			yield return StartCoroutine(DoTilt(startAngles, targetAngles, _MoveDuration / 2f));

			startAngles = _modelRoot.localEulerAngles;
			targetAngles = Vector3.zero;

			yield return StartCoroutine(DoTilt(startAngles, targetAngles, _MoveDuration / 2f));
		}
		else
		{
			startAngles = Vector3.zero;
			targetAngles = Vector3.zero + _frontTilt;

			yield return StartCoroutine(DoTilt(startAngles, targetAngles, _MoveDuration / 2f));

			startAngles = _modelRoot.localEulerAngles;
			targetAngles = Vector3.zero;

			yield return StartCoroutine(DoTilt(startAngles, targetAngles, _MoveDuration / 2f));
		}
	}
	
	IEnumerator DoTilt(Vector3 startAngles, Vector3 targetAngles, float duration)
	{
		float elapsed = 0f;

		while (elapsed < duration)
		{
			_modelRoot.localRotation = Quaternion.Lerp(Quaternion.Euler(startAngles), Quaternion.Euler(targetAngles), elapsed / duration);
			//_modelRoot.localEulerAngles = Vector3.Lerp(startAngles, targetAngles, elapsed / duration);
			yield return null;
			elapsed += Time.deltaTime;
		}
	}

	private void CheckAndShoot()
	{
		if (_shotTimer > shotInterval)
		{
			RaycastHit hitInfo;
			if (!Physics.Raycast(transform.position, transform.forward, out hitInfo, 5f, 1 << gameObject.layer))
			{
				if (Random.Range(0f, 1f) > 0.95f)
				{
					ProjectileManager.Instance.ShootProjectile(shotSpawn);
				}
			}
			_shotTimer = 0f;
		}
		else
		{
			_shotTimer += Time.deltaTime;
		}
    }

	private void Update()
	{
		if (!_isInitComplete) return;

		CheckAndShoot();

		if (Mathf.Approximately(transform.position.z, GameManager.Instance.Bounds.Near))
		{
			GameManager.Instance.EndGame(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Wall"))
		{
			EnemyManager.Instance.EdgeReached();
		}
		else if (other.CompareTag("PlayerBullet"))
		{
			if (other.GetComponent<Projectile>().IsPlayerShot) // #TODO : Optimize this!
				ApplyDamage();
		}
	}

	public void ApplyDamage()
	{
		StopAllCoroutines();
		GameManager.Instance.OnPlayerRevival -= OnPlayerRevival;
		EnemyManager.Instance.MoveEvent -= Move;
		EnemyManager.Instance.EnemyKilled(this);
		
		_isInitComplete = false;
	}

	public void OnPlayerRevival()
	{

	}

	private void OnMouseDown()
	{
		ApplyDamage();
	}

	// Move

	// Shoot

	// Collision handling
}
