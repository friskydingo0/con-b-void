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
	MainMenu, Playing, Paused, Waiting, Victory, GameOver
}

public class GameManager : MonoBehaviour, IGameStateListener
{
	#region Singleton
	private static GameManager _instance = null;
	public static GameManager Instance { get { return _instance; } }

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
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

	// Very basic FSM for tracking game state
	private System.Action<GameState, GameState> _OnGameStateChanged;
	private GameState _state = GameState.MainMenu;
	public GameState State {
		get { 
			return _state;
		}
		set	{
			_OnGameStateChanged?.Invoke(_state, value);
			_state = value;
		}
	}

	[Header("Player")]
	[SerializeField]
	private PlayerController playerPrefab = null;
	[SerializeField]
	private PlayerController playerController = null;
	
	[SerializeField]
	private Transform playerSpawn = null;

	public int LivesLeft { get; private set; }
	public const int MaxLives = 3;

	public int Score { get; private set; }
	public int HiScore { get; private set; }

	private UIHandler _uiHandler = null;

	[Header("Level")]
	public int Level = 1;

	public LevelData LevelDatabase {  get; private set; }

	[Header("Audio")]
	[SerializeField]
	private AudioSource audioSource = null;
	[SerializeField]
	private float[] pitches = null;
	private int pitchIndex = 0;

	public System.Action<bool> OnPlayerRevival { get; set; }

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		// Get the bounds and keep them ready
		Bounds.Left = _leftWall.position.x;
		Bounds.Right = _rightWall.position.x;
		Bounds.Top = _topWall.position.y;
		Bounds.Bottom = _bottomWall.position.y;
		Bounds.Near = playerSpawn.position.z - 1f;
		Bounds.Far = _farWall.position.z;

		_uiHandler = FindFirstObjectByType<UIHandler>();

		if (playerController == null)
		{
			playerController = Instantiate<PlayerController>(playerPrefab, playerSpawn.position, playerSpawn.rotation);
		}
		playerController.Init();
		EnemyManager.Instance.Init();

		LivesLeft = MaxLives;

		LevelDatabase = Resources.Load<LevelData>("LevelData");
		HiScore = PlayerPrefs.GetInt("HiScore", 0);

		RegisterStateChangeListeners();
	}

	public void AddStateListener(System.Action<GameState, GameState> listener)
	{
		_OnGameStateChanged += listener;
	}

	public void RemoveStateListener(System.Action<GameState, GameState> listener)
	{
		_OnGameStateChanged -= listener; 
	}

	private void RegisterStateChangeListeners()
	{
		_OnGameStateChanged += OnGameStateChanged;
		_OnGameStateChanged += _uiHandler.OnGameStateChanged;
		_OnGameStateChanged += playerController.OnGameStateChanged;
		_OnGameStateChanged += EnemyManager.Instance.OnGameStateChanged;
		_OnGameStateChanged += ProjectileManager.Instance.OnGameStateChanged;
	}

	public void AddScore(int score)
	{
		Score += score;
		bool isHiScore = Score > HiScore;
		if (isHiScore)
			HiScore = Score;

		_uiHandler.UpdateScore(Score, isHiScore);
	}

	public void StartGame()
	{
		State = GameState.Playing;
		LivesLeft = MaxLives;
		EnemyManager.Instance.SpawnLevelEnemies(Level);
	}

	public void Pause()
	{
		State = GameState.Paused;
	}

	public void Resume()
	{
		State = GameState.Playing;
	}

	public void NextLevel()
	{ 
		// Do the cleanup and reset the level
		Level++;
		if (Level >= LevelDatabase.levels.Count)
		{
			// Win condition
			State = GameState.Victory;
			EndGame(true);
			return;
		}
		EnemyManager.Instance.ReInit();
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.K))
			EndGame(false);
#endif
	}

	public void OnPlayerHit()
	{
		LivesLeft--;

		if (LivesLeft <= 0)
		{
			// Game over
			EndGame(false);
		}
		else
		{
			StartCoroutine(RevivalSequence());
		}
	}

	IEnumerator RevivalSequence()
	{
		// Pause game
		State = GameState.Waiting;

		// Clear all projectiles
		OnPlayerRevival?.Invoke(false);

		yield return new WaitForSecondsRealtime(1f);

		// Replace player and play effect
		playerController.ReviveAtPosition(playerSpawn.position);

		// Update UI
		_uiHandler.UpdatePlayerLives(LivesLeft);

		yield return new WaitForSecondsRealtime(1f);

		// Resume playing
		State = GameState.Playing;

		OnPlayerRevival?.Invoke(true);
	}

	public void PlayBoom()
	{
		audioSource.pitch = pitches[pitchIndex++ % pitches.Length];
		audioSource.Play();
	}

	public void EndGame(bool didWin)
	{
		PlayerPrefs.SetInt("HiScore", Score);
		PlayerPrefs.Save();

		State = didWin ? GameState.Victory : GameState.GameOver;
	}

	public void BackToTitleScreen()
	{
		State = GameState.MainMenu;

		// Reset score
		Score = 0;
	}

	public void OnGameStateChanged(GameState fromState, GameState toState)
	{
		// Do all the state related handling here
		if (fromState == toState)
			return;

		if (toState == GameState.MainMenu)
		{
			EnemyManager.Instance.ReInit();
		}
	}
}
