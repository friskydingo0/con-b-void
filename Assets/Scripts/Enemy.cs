using System.Collections;
using UnityEngine;

public enum EnemyType
{
	Easy = 0,
	Medium,
	Hard
}

public class Enemy : MonoBehaviour
{
	public EnemyType enemyType;
	public int points = 0;
	public float shotInterval = 1.5f;

	private const float _MoveDuration = 0.1f;	// For smoothing. Shouldn't be higher than the fastest move interval (from the EnemyManager)

	[SerializeField, Range(0.1f, 3f)]
	private float MoveOffset = 0.25f;

	[SerializeField]
	private Transform shotSpawn = null;
	private Transform _modelRoot = null;

	private float _shotTimer = 0f;

	private bool _isInitComplete = false;

	private void Awake()
	{
		_modelRoot = transform.Find("ModelRoot");
	}

	public void Init()
	{
		EnemyManager.Instance.MoveEvent += Move;
		_shotTimer = 0f;
		_isInitComplete = true;
	}

	private void Move(int dir)
	{
		StartCoroutine(SmoothMove(dir));
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

	private void CheckAndShoot()
	{
		if (_shotTimer > shotInterval)
		{
			RaycastHit hitInfo;
			if (!Physics.Raycast(transform.position, transform.forward, out hitInfo, 1.5f, 1 << gameObject.layer))
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
			// Game over
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
		EnemyManager.Instance.MoveEvent -= Move;
		EnemyManager.Instance.EnemyKilled(this);
		
		_isInitComplete = false;
	}

	private void OnMouseDown()
	{
		ApplyDamage();
	}

	// Move

	// Shoot

	// Collision handling
}
