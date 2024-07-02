using System.Collections;
using UnityEngine;

public enum EnemyType
{
	Easy,
	Medium,
	Hard
}

public class Enemy : MonoBehaviour
{
	public int points = 0;
	private const float _MoveDuration = 0.1f;	// Shouldn't be higher than the fastest move interval (from the EnemyManager)

	[SerializeField, Range(0.1f, 3f)]
	private float MoveOffset = 0.25f;

	public void Init()
	{
		EnemyManager.Instance.MoveEvent += Move;
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

	private void Update()
	{
		if (Mathf.Approximately(transform.position.z, GameManager.Instance.bounds.Near))
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
		Destroy(gameObject);
	}

	private void OnMouseDown()
	{
		ApplyDamage();
	}

	// Move

	// Shoot

	// Collision handling
}
