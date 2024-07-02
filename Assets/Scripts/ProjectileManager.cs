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

	private Projectile _projectilePrefab;

	private ProjectileManager() {
		//enemyBulletPool = new
		_projectilePrefab = Resources.Load<Projectile>("Bullet");
		playerBulletPool = new ObjectPool<Projectile>(
			() => { return GameObject.Instantiate<Projectile>(_projectilePrefab); },
			bullet => { bullet.gameObject.SetActive(true); },
			bullet => { HitObject(bullet); },
			bullet => { GameObject.Destroy(bullet.gameObject); },
			false, 10, 20);
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
