using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunkerPiece : MonoBehaviour
{
    private const int totalHits = 4;
	private int hitsRemaining = 4;

	private Renderer _renderer = null;

	private void Awake()
	{
		_renderer = GetComponent<Renderer>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("EnemyBullet") || other.CompareTag("PlayerBullet"))
		{
			hitsRemaining--;
			if (hitsRemaining == 0)
			{
				GetComponent<Collider>().enabled = false;
				gameObject.SetActive(false);
			}
			
			// Update visuals
			float percentage = 1f - (float)hitsRemaining / (float)totalHits;
			_renderer.material.SetFloat("_DissolveAmount", percentage);
		}
		if (other.CompareTag("Enemy"))
		{
			hitsRemaining = 0;
			GetComponent<Collider>().enabled = false;
			gameObject.SetActive(false);
		}
	}
}
