using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // References
    [SerializeField]
    private Transform _shotPoint = null;

    [SerializeField]
    private Projectile _bulletObject = null;

	// Public parameters
	public float _MoveSpeed = 1.0f;

	// Internal stuff


	// Start is called before the first frame update
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Placeholder movement for the Gold Spike
        if (Input.GetKey(KeyCode.A))
        {
            // Move left
            transform.Translate(transform.right * -_MoveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
			// Move right
			transform.Translate(transform.right * _MoveSpeed * Time.deltaTime);
		}
    }
}
