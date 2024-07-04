using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

	[SerializeField]
	private RectTransform playerLivesContainer;
	#endregion

	[SerializeField] TMP_Text victoryScore;
	[SerializeField] TMP_Text victoryHiScore;

	[SerializeField] TMP_Text defeatScore;
	[SerializeField] TMP_Text defeatHiScore;

	public void StartGame()
	{
		StartCoroutine(StartSequence());
	}

	IEnumerator StartSequence()
	{
		// Add a screen fade here when there's time.
		yield return new WaitForSeconds(0.5f);
		scoreText.text = string.Format("Score: {0}", 0);
		hiScoreText.text = string.Format("Hi-score: {0}", GameManager.Instance.HiScore);
		titlePanel.SetActive(false);
		hudPanel.SetActive(true);

		GameManager.Instance.StartGame();

		Transform lifeIcon = playerLivesContainer.GetChild(0);
		for (int i = 0; i < GameManager.Instance.LivesLeft; i++)
		{
			var newIcon = Instantiate(lifeIcon, playerLivesContainer);
			newIcon.gameObject.SetActive(true);
		}
	}

	public void ShowTitleScreen()
	{
		pausePanel.SetActive(false);
		hudPanel.SetActive(false);
		titlePanel.SetActive(true);
		winPanel.SetActive(false);
		losePanel.SetActive(false);
		GameManager.Instance.BackToTitleScreen();
	}

	public void PauseGame()
	{
		GameManager.Instance.Pause();
		pausePanel.SetActive(true);
	}

	public void ResumeGame()
	{
		GameManager.Instance.Resume();
		pausePanel.SetActive(false);
	}

	public void EndGame()
	{
		GameManager.Instance.EndGame(false);
	}

	public void UpdateScore(int score, bool isHiScore = false)
	{
		scoreText.text = string.Format("Score: {0}", score);
		if (isHiScore)
		{
			hiScoreText.text = string.Format("Hi-score: {0}", score);
		}
	}

	public void UpdatePlayerLives(int playerLives)
	{
		Destroy(playerLivesContainer.GetChild(playerLivesContainer.childCount - 1).gameObject);
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
				victoryScore.text = GameManager.Instance.Score.ToString();
				victoryHiScore.text = GameManager.Instance.HiScore.ToString();
				winPanel.SetActive(true);
			}
			else if (toState == GameState.GameOver)
			{
				defeatScore.text = GameManager.Instance.Score.ToString();
				defeatHiScore.text = GameManager.Instance.HiScore.ToString();
				losePanel.SetActive(true);
			}
		}
	}
}
