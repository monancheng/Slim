using System;
using PrimitivesPro.GameObjects;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static event Action <float> OnCreateTube;
	private float _startRadius;
	private float _currentRadius;
	private float _height = 6.0f;
	private int _sides = 30;
	private const float ErrorCoeff = 0.2f;
	private bool _isDontMove;
	private Vector3 _startPosition;

	private Cylinder _script;
	private Vector3 _startCursorPoint;
	private bool _isMoveToExit;
	private float _moveToExitSpeed;

	private int _comboCounter;
	private bool _isHaveCollision;
	private const float BonusRadiusCoeff = 0.5f;

	void Start ()
	{
		_startPosition = transform.position;
		Respown();
	}

	private void OnEnable()
	{
		MyTube.OnCanMove += OnCanMove;
		GlobalEvents<OnStartGame>.Happened += StartGame;
		GlobalEvents<OnGameOver>.Happened += GameOver;
	}

	private void GameOver(OnGameOver e)
	{
		Invoke("Respown", 1f);
	}
	
	private void Respown()
	{
		transform.position = _startPosition;
		PepareCylinder();
		_comboCounter = 0;
	}

	private void StartGame(OnStartGame obj)
	{
		GameEvents.Send(OnCreateTube, _currentRadius);
		_isHaveCollision = false;
	}

	private void PepareCylinder()
	{
		_script = GetComponent<Cylinder>();
		if (_currentRadius == 0)
		{
			_startRadius = _script.radius;
		}
		_currentRadius = _startRadius;
		_height = _script.height;
		_sides = _script.sides;
		_script.AddMeshCollider(true);
		
		GetComponent<Renderer>().material.SetColor("_Color", ColorTheme.GetPlayerColor());
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;

		ChangeSize();

		_isMoveToExit = false;
	}
	
	Shader GetDiffuseShader()
	{
		return Shader.Find("Diffuse");
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (_isHaveCollision) return;
		
		_isHaveCollision = true;
		float cutSize = Mathf.Abs(transform.position.x - other.gameObject.transform.position.x);
		if (cutSize > ErrorCoeff)
		{
			_currentRadius -= cutSize;
			_comboCounter = 0;
		}
		else
		{
			++_comboCounter;
		}
		if (_currentRadius > 0f)
		{
			if (cutSize > ErrorCoeff)
			{
				ChangeSize();
				CreateCutTube(cutSize, other.GetComponent<MyTube>().Speed);
				_comboCounter = 0;
			}
			if (_comboCounter >= 3)
			{
				GameEvents.Send(OnCreateTube, _currentRadius + BonusRadiusCoeff);
			}
			else
			{
				GameEvents.Send(OnCreateTube, _currentRadius);
			}
		}
		else
		{
			GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.0f/255f, 0f/255f));
			_isMoveToExit = true;
			_moveToExitSpeed = other.GetComponent<MyTube>().Speed;
			GlobalEvents<OnGameOver>.Call(new OnGameOver());
		}
		_isDontMove = true;
	}

	private void OnCanMove()
	{
		_isDontMove = false;
		if (_comboCounter >= 3)
		{
			_comboCounter = 0;
			_currentRadius += BonusRadiusCoeff;
			ChangeSize();
		}
		_isHaveCollision = false;
	}
	
	private void ChangeSize()
	{
		_script.GenerateGeometry(_currentRadius, _height, _sides, 1,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Center);
		_script.FitCollider();
	}
	
	private void CreateCutTube(float cutSize, float _speed)
	{
		BaseObject _shapeObject;
		_shapeObject = Tube.Create(_currentRadius, _currentRadius + cutSize, _height, _sides, 1, 0.0f, false,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Center);
		
		GameObject go = _shapeObject.gameObject;
		go.GetComponent<Renderer>().material = new Material(GetDiffuseShader());
		go.GetComponent<Renderer>().material.SetColor("_Color", new Color(255f / 255.0f, 201f / 255f, 104f / 255f));
		go.transform.position = transform.position;
		PlayerTube pt = go.AddComponent<PlayerTube>();
		pt.Speed = _speed;
	}

	private void Update()
	{
		if (_isMoveToExit)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - _moveToExitSpeed, transform.position.z);
			if (transform.position.y < -24f) Respown();
			return;
		}
		
		if (_isDontMove)
		{
			_startCursorPoint = Input.mousePosition;
			return;
		}
		
		if (InputController.IsTouchOnScreen(TouchPhase.Began))
		{
			_startCursorPoint = Input.mousePosition;
		}

		if (InputController.IsTouchOnScreen(TouchPhase.Moved))
		{
			Vector2 cursorPosition = Input.mousePosition;
			float newX = transform.position.x + (cursorPosition.x - _startCursorPoint.x) / 12f;
			if (newX < -20f) newX = -20f; 
			else if (newX > 20f) newX = 20f; 
			transform.position = new Vector3(newX, transform.position.y, transform.position.z);
			_startCursorPoint = cursorPosition;
		}
	}
}
