using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour, IGameStateListener
{
	[SerializeField]
	private GameObject titlePanel;
	[SerializeField]
	private GameObject pausePanel;
	[SerializeField]
	private GameObject winPanel;
	[SerializeField]
	private GameObject losePanel;
	[SerializeField]
	private GameObject hudPanel;

	#region HUD
	[Header("HUD")]
	[SerializeField]
	private TMP_Text scoreText;
	[SerializeField]
	private TMP_Text hiScoreText;
	#endregion

	public void StartGame()
	{
		StartCoroutine(StartSequence());
	}

	IEnumerator StartSequence()
	{
		// Add a screen fade here when there's time.
		yield return new WaitForSeconds(0.5f);

		titlePanel.SetActive(false);
		hudPanel.SetActive(true);
		GameManager.Instance.StartGame();
	}

	public void ShowTitleScreen()
	{
		titlePanel.SetActive(true);
	}

	public void UpdateScore(int score, bool isHiScore = false)
	{
		scoreText.text = string.Format("Score: {0}", score);
		hiScoreText.text = string.Format("Hi-score: {0}", score);
	}

	public void OnGameStateChanged(GameState fromState, GameState toState)
	{
		if (fromState == toState)
			return;

		if (fromState == GameState.Playing)
		{
			if (toState == GameState.Paused)
			{
				pausePanel.SetActive(true);
			}
			else if (toState == GameState.Victory)
			{
				winPanel.SetActive(true);
			}
			else if (toState == GameState.GameOver)
			{
				losePanel.SetActive(true);
			}
		}
	}
}
