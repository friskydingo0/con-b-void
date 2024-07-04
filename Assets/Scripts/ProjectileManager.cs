using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileManager : IGameStateListener
{
	private static ProjectileManager instance;
	public static ProjectileManager Instance {
		get
		{
			if (instance == null)
				instance = new ProjectileManager();
			return instance;
		}
	}

	private ObjectPool<Projectile> _enemyBulletPool;
	private ObjectPool<Projectile> _playerBulletPool;

	private Projectile _playerProjectilePf;
	private Projectile _enemyProjectilePf;

	private ProjectileManager() {
		
		_playerProjectilePf = Resources.Load<Projectile>("PlayerBullet");
		_enemyProjectilePf = Resources.Load<Projectile>("EnemyBullet");

		_playerBulletPool = new ObjectPool<Projectile>(
			() => { return GameObject.Instantiate<Projectile>(_playerProjectilePf); },
			bullet => { bullet.gameObject.SetActive(true); },
			bullet => { HitObject(bullet); },
			bullet => { GameObject.Destroy(bullet.gameObject); },
			false, 10, 20);

		_enemyBulletPool = new ObjectPool<Projectile>(
			() => { return GameObject.Instantiate<Projectile>(_enemyProjectilePf); },
			bullet => { bullet.gameObject.SetActive(true); },
			bullet => { HitObject(bullet); },
			bullet => { GameObject.Destroy(bullet.gameObject); },
			false, 50, 90);
	}

	public void ShootProjectile(Transform spawn, bool isPlayerShot = false)
	{
		Projectile p;
		if (isPlayerShot)
		{
			if (_playerBulletPool.CountActive < PlayerController.MaxBullets)
			{
				p = _playerBulletPool.Get();
			}
			else
				return;
		}
		else
		{
			p = _enemyBulletPool.Get();
		}
		p.transform.position = spawn.position;
		p.transform.rotation = spawn.rotation;
		p.Initialize(1f, isPlayerShot);
	}

	public void ReturnProjectile(Projectile projectile)
	{
		if (projectile.IsPlayerShot)
		{
			_playerBulletPool.Release(projectile);
		}
		else
		{
			_enemyBulletPool.Release(projectile);
		}
	}

	private void HitObject(Projectile projectile)
	{
		projectile.gameObject.SetActive(false);
		// Do something interesting
	}

	public void OnGameStateChanged(GameState fromState, GameState toState)
	{
		throw new System.NotImplementedException();
	}
}
