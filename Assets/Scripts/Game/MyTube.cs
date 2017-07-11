using System;
using cakeslice;
using UnityEngine;

public class MyTube : MonoBehaviour
{
	public static event Action OnCanMove;
	[HideInInspector] public float Speed = 0.3f;
	private bool _isReadyToDelete;
	private bool _isSentMoveEvent;
	private bool _isHaveCollision;
	[HideInInspector] public Outline Outline;

	private void Start()
	{
		transform.localScale = Vector3.zero;
		Outline = gameObject.AddComponent<Outline>();
		Outline.enabled = false;
	}

	private void OnEnable()
	{
		Player.OnTubeGoodAnimation += GoodAnimation;
	}

	private void OnDisable()
	{
		Player.OnTubeGoodAnimation -= GoodAnimation;
	}
	
	private void GoodAnimation()
	{
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isHaveCollision) return;
		
		if (other.gameObject.CompareTag("Player"))
		{
			_isHaveCollision = true;
			Destroy(gameObject.GetComponent<Collider>());
			Outline.enabled = true;
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
			if (!_isHaveCollision) GlobalEvents<OnGameOver>.Call(new OnGameOver());
			_isSentMoveEvent = true;
			Outline.enabled = false;
		}
		
		if (_isReadyToDelete && transform.position.y < -24f)
		{
			Destroy(gameObject);
		}
	}
}
