using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEnemy : MonoBehaviour
{
	public int MaxPoints = 200;

	private const float _MoveSpeed = 2f;

	private Vector3 _startPos = Vector3.zero;
	private Vector3 _endPos = Vector3.zero;

	private void OnEnable()
	{
		transform.LookAt(_endPos - _startPos);
	}

	public void Init(Vector3 start, Vector3 end)
	{
		_startPos = start;
		_endPos = end;

		transform.position = _startPos;

		gameObject.SetActive(true);
	}

	private void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, _endPos, _MoveSpeed * Time.deltaTime);
		if (Vector3.Distance(transform.position, _endPos) < 0.1f)
		{
			gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerBullet"))
		{
			ApplyDamage();
			gameObject.SetActive(false);
		}
	}

	private void ApplyDamage()
	{
		float distance = Vector3.Distance(transform.position, _endPos);
		float totalDistance = Vector3.Distance(_startPos, _endPos);
		float percentageDist = distance / totalDistance;
		int score = 200;
		if (percentageDist > 0.66f)
			score = 200;
		else if (percentageDist > 0.33f)
			score = 100;
		else
			score = 50;

		EnemyManager.Instance.SpecialEnemyKilled(score);
	}
}
