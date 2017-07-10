using System;
using UnityEngine;

public class MyTube : MonoBehaviour
{
	public static event Action OnCanMove;
	[HideInInspector] public float Speed = 0.3f;
	private bool _isReadyToDelete;
	private bool _isSentMoveEvent;
	private bool _isHaveCollision;

	private void Start()
	{
		transform.localScale = Vector3.zero;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isHaveCollision) return;
		
		if (other.gameObject.CompareTag("Player"))
		{
			_isHaveCollision = true;
			Destroy(gameObject.GetComponent<Collider>());
		}
	}

	private void Update()
	{
		if (transform.localScale.x < 1f)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, transform.localScale.z + 0.1f);
		}
		transform.position = new Vector3(transform.position.x, transform.position.y - Speed, transform.position.z);
		
		if (!_isSentMoveEvent&&transform.position.y < -8f)
		{
			_isReadyToDelete = true;
			GameEvents.Send(OnCanMove);
			_isSentMoveEvent = true;
		}
		
		if (_isReadyToDelete && transform.position.y < -21f)
		{
			if (!_isHaveCollision) GlobalEvents<OnGameOver>.Call(new OnGameOver());
			Destroy(gameObject);
		}
	}
}
