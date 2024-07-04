using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunker : MonoBehaviour, IGameStateListener
{
	private void Start()
	{
		GameManager.Instance.AddStateListener(OnGameStateChanged);
	}

	public void OnGameStateChanged(GameState fromState, GameState toState)
	{
		if (fromState != toState)
		{
			if (fromState == GameState.MainMenu && toState == GameState.Playing)
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).gameObject.SetActive(true);
				}
			}
			else if (toState == GameState.MainMenu)
			{
				gameObject.SetActive(false);
			}
		}
	}

	private void OnDestroy()
	{
		GameManager.Instance.RemoveStateListener(OnGameStateChanged);
	}
}
