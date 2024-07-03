using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct LevelBounds
{
	public float Left, Top, Right, Bottom, Near, Far;
}

public enum GameState
{
	MainMenu, Playing, Paused, Victory, GameOver
}

public class GameManager : MonoBehaviour
{
	#region Singleton
	private static GameManager _instance = null;
	public static GameManager Instance { get { return _instance; } }

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Init();
		}
		else
		{
			Destroy(gameObject);
		}
	}
	#endregion

	public LevelBounds Bounds = new LevelBounds();

	[SerializeField] private Transform _leftWall = null;
	[SerializeField] private Transform _rightWall = null;
	[SerializeField] private Transform _topWall = null;
	[SerializeField] private Transform _bottomWall = null;
	[SerializeField] private Transform _farWall = null;

	// Game State
	public GameState State { get; private set; }
	
	[Header("Player")]
	[SerializeField]
	private PlayerController playerPrefab = null;
	[SerializeField]
	private PlayerController playerController = null;
	
	[SerializeField]
	private Transform playerSpawn = null;

	public int Score { get; private set; }

	[Header("Level")]
	public int Level = 1;

	public LevelData LevelDatabase {  get; private set; }

	[Header("Audio")]
	[SerializeField]
	private AudioSource audioSource = null;
	[SerializeField]
	private float[] pitches = null;
	private int pitchIndex = 0;

	public System.Action OnPlayerRevival { get; set; }

	private void Init()
	{
		// Get the bounds and keep them ready
		Bounds.Left = _leftWall.position.x;
		Bounds.Right = _rightWall.position.x;
		Bounds.Top = _topWall.position.y;
		Bounds.Bottom = _bottomWall.position.y;
		Bounds.Near = playerSpawn.position.z;
		Bounds.Far = _farWall.position.z;

		if (playerController == null)
		{
			playerController = Instantiate<PlayerController>(playerPrefab, playerSpawn.position, playerSpawn.rotation);
		}
		playerController.Init();

		LevelDatabase = Resources.Load<LevelData>("LevelData");
	}

	public void UpdateScore(int score)
	{
		Score += score;
		// Update the UI

		Debug.LogFormat("Score: {0}", Score);
	}

	public void StartLevel()
	{
		State = GameState.Playing;
		EnemyManager.Instance.SpawnLevelEnemies(Level);
	}

	public void NextLevel()
	{ 
		// Do the cleanup and reset the level
		Level++;
		EnemyManager.Instance.ReInit();
	}

	public void RevivePlayer()
	{
		StartCoroutine(RevivalSequence());
	}

	IEnumerator RevivalSequence()
	{
		// Pause game
		State = GameState.Paused;

		// Clear all projectiles
		OnPlayerRevival?.Invoke();

		yield return new WaitForSecondsRealtime(1f);

		// Replace player and play effect
		playerController.ReviveAtPosition(playerSpawn.position);

		// Update UI


		// Resume playing
		State = GameState.Playing;
	}

	public void PlayBoom()
	{
		audioSource.pitch = pitches[pitchIndex++ % pitches.Length];
		audioSource.Play();
	}

	public void EndGame(string reason)
	{
		Debug.Log(reason);
	}
}
