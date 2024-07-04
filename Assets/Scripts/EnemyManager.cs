using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyManager : MonoBehaviour, IGameStateListener
{
	/// <summary>
	/// Pseudo-singleton class for maintaining enemy updates
	/// </summary>
	/// 

	#region Singleton
	private static EnemyManager _instance = null;
	public static EnemyManager Instance {  get { return _instance; } }

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

	[SerializeField]
	private Enemy enemyPrefab = null;
	[SerializeField]
	private Enemy[] enemyPrefabs = null;

	private Dictionary<EnemyType, ObjectPool<Enemy>> _enemyPools = new Dictionary<EnemyType, ObjectPool<Enemy>>();
	
	private List<Enemy> _enemies = new List<Enemy>();	// Kinda wanna keep this to check how many enemies are still active. Maybe to affect the game based on how many left?
	private const int MaxRows = 6;
	private const int MaxColumns = 11;
	private const int MaxStacks = 1;
	private const int MaxEnemies = 50;

	[SerializeField, Range(0, 2)]
	private float OffsetX = 1f;

	[Range(0, 2)]
	public float OffsetY = 1f;

	[SerializeField]
	private Transform spawnOrigin = null;

	[SerializeField]
	private float MarchInterval = 1f; // #TODO : Set this from the Level Data

	public System.Action<int, bool> MoveEvent = null;
	
	private float _currentMarchInterval = 1f;
	private float _updateTimer = 0f;

	private int _currentDirection = 1;
	private int _previousDirection = -1;

	private bool _isInitialized = false;

	private void Init()
	{
		if (_enemyPools.Count == 0)
		{
			InitializePools();
		}

		_currentMarchInterval = MarchInterval;
		_updateTimer = 0f;
		_currentDirection = 1;
		_previousDirection = -1;
		_isInitialized = true;
	}

	public void ReInit()
	{
		_isInitialized = false;
		_enemies.Clear();
		Init();
	}

	private void InitializePools()
	{
		ObjectPool<Enemy> pool = new ObjectPool<Enemy>(
			() => { return Instantiate<Enemy>(enemyPrefabs[(int)EnemyType.Easy]); },
			actionOnGet: e => { e.gameObject.SetActive(true); }, defaultCapacity: 40, maxSize: 70);
		_enemyPools.Add(EnemyType.Easy, pool);

		pool = new ObjectPool<Enemy>(
			() => { return Instantiate<Enemy>(enemyPrefabs[(int)EnemyType.Medium]); },
			actionOnGet: e => { e.gameObject.SetActive(true); }, defaultCapacity: 40, maxSize: 70);
		_enemyPools.Add(EnemyType.Medium, pool);

		pool = new ObjectPool<Enemy>(
			() => { return Instantiate<Enemy>(enemyPrefabs[(int)EnemyType.Hard]); },
			actionOnGet: e => { e.gameObject.SetActive(true); }, defaultCapacity: 40, maxSize: 70);
		_enemyPools.Add(EnemyType.Hard, pool);
	}

	public void SpawnLevelEnemies(int level)
	{
		LevelInfo levelInfo = GameManager.Instance.LevelDatabase.levels[level - 1]; // because level data is 0-indexed in the LevelData asset
		
		int startingRow = levelInfo.StartingRow;
		for (int i = 0; i < levelInfo.Rows.Count; i++)
		{
			EnemyType eType = levelInfo.Rows[i];
			for(int j = 0; j < MaxColumns; j++)
			{
				Enemy enemy = GetEnemyToSpawn(eType);
				_enemies.Add(enemy);
				enemy.transform.position = new Vector3(spawnOrigin.position.x + (j * OffsetX), spawnOrigin.position.y, spawnOrigin.position.z + (OffsetY * (-startingRow - i)));
				enemy.transform.rotation = spawnOrigin.rotation;

				enemy.Init();
			}
		}
	}

	private Enemy GetEnemyToSpawn(EnemyType enemyType)
	{
		return _enemyPools[enemyType].Get();
	}

	private void Update()
	{
		if (!_isInitialized) { return; }
		
		if (GameManager.Instance.State != GameState.Playing)
			return;

		if (_updateTimer < _currentMarchInterval)
		{
			_updateTimer += Time.deltaTime;
		}
		else
		{
			_updateTimer = 0f;
			GameManager.Instance.PlayBoom();	// It's so much fun with this effect. Like a marching order.
			MoveEvent?.Invoke(_currentDirection, (_currentMarchInterval <= (MarchInterval + 0.1f)));
			if (_currentDirection == 0)
			{
				_currentDirection = _previousDirection;
				_previousDirection = _currentDirection;
			}
		}
	}

	public void EdgeReached()
	{
		if (_currentDirection != 0)
		{
			_previousDirection = _currentDirection > 0 ? -1 : 1;
			_currentDirection = 0;
		}
	}

	public void EnemyKilled(Enemy enemy)
	{
		GameManager.Instance.AddScore(enemy.points);

		_enemyPools[enemy.enemyType].Release(enemy);
		enemy.gameObject.SetActive(false);
		_enemies.Remove(enemy);

		_currentMarchInterval = Mathf.Max(_currentMarchInterval - 0.02f, 0.2f);
		if (_enemies.Count == 0)
		{
			GameManager.Instance.NextLevel();
		}
	}

	public void OnGameStateChanged(GameState fromState, GameState toState)
	{
		throw new NotImplementedException();
	}

	// Spawn special enemies
}
