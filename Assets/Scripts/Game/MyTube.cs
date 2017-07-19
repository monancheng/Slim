using System;
using UnityEngine;

public class MyTube : MonoBehaviour
{
	public static event Action OnCanMove;
	public static event Action OnCanSpawnCoin;
	[HideInInspector] public float Speed = 0.3f;
	private bool _isReadyToDelete;
	private bool _isSentMoveEvent;
	private bool _isHaveCollision;
	private bool _isGameOver;
	private bool _isMove;

	private void Start()
	{
		transform.localScale = Vector3.zero;
	}

	private void OnEnable()
	{
		GlobalEvents<OnGameOver>.Happened += GameOver;
		Player.OnTubeMove += OnTubeMove;
	}

	private void OnDisable()
	{
		GlobalEvents<OnGameOver>.Happened -= GameOver;
		Player.OnTubeMove -= OnTubeMove;
	}
	
	private void OnTubeMove()
	{
		_isMove = true;
	}

	private void GameOver(OnGameOver obj)
	{
		_isGameOver = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_isHaveCollision = true;
			Destroy(GetComponent<Collider>());
		} else 
		if (other.CompareTag("CoinPlaneSensor"))
		{
			GameEvents.Send(OnCanSpawnCoin);
		}
	}

	private void Update()
	{
		if (transform.localScale.x < 1f)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f,
				transform.localScale.z + 0.1f);
		}
		else
		{
			transform.localScale = Vector3.one;
		}

		transform.Rotate(Vector3.up, TubeManager.RotateSpeed);

		if (_isMove)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - Speed * Time.deltaTime, transform.position.z);

			if (!_isGameOver && !_isSentMoveEvent && transform.position.y < 5f)
			{
				_isReadyToDelete = true;
				if (!_isHaveCollision)
					GlobalEvents<OnGameOver>.Call(new OnGameOver());
				else
					GameEvents.Send(OnCanMove);
				_isSentMoveEvent = true;
			}

			if ((_isGameOver||_isReadyToDelete) && transform.position.y < -24f)
			{
				Destroy(gameObject);
			} 
		}
	}
}
