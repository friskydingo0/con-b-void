using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileManager
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

	private ObjectPool<Projectile> enemyBulletPool;
	private ObjectPool<Projectile> playerBulletPool;

	private Projectile _playerProjectilePf;
	private Projectile _enemyProjectilePf;

	private ProjectileManager() {
		
		_playerProjectilePf = Resources.Load<Projectile>("PlayerBullet");
		_enemyProjectilePf = Resources.Load<Projectile>("EnemyBullet");

		playerBulletPool = new ObjectPool<Projectile>(
			() => { return GameObject.Instantiate<Projectile>(_playerProjectilePf); },
			bullet => { bullet.gameObject.SetActive(true); },
			bullet => { HitObject(bullet); },
			bullet => { GameObject.Destroy(bullet.gameObject); },
			false, 10, 20);

		enemyBulletPool = new ObjectPool<Projectile>(
			() => { return GameObject.Instantiate<Projectile>(_enemyProjectilePf); },
			bullet => { bullet.gameObject.SetActive(true); },
			bullet => { HitObject(bullet); },
			bullet => { GameObject.Destroy(bullet.gameObject); },
			false, 50, 90);
	}

	public void ShootProjectile(Transform spawn, bool isPlayerShot = false, System.Action callback = null)
	{
		Projectile p;
		if (isPlayerShot)
		{
			p = playerBulletPool.Get();
		}
		else
		{
			p = enemyBulletPool.Get();
		}
		p.transform.position = spawn.position;
		p.transform.rotation = spawn.rotation;
		p.Initialize(1f, isPlayerShot, callback);
	}

	public void ReturnProjectile(Projectile projectile)
	{
		if (projectile.IsPlayerShot)
		{
			playerBulletPool.Release(projectile);
		}
		else
		{
			enemyBulletPool.Release(projectile);
		}
	}

	private void HitObject(Projectile projectile)
	{
		projectile.gameObject.SetActive(false);
		// Do something interesting
	}
	
}
