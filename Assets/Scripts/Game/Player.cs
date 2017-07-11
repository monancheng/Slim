using System;
using cakeslice;
using PrimitivesPro.GameObjects;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static event Action <float> OnTubeCreate;
	public static event Action OnTubeGoodAnimation;
	private float _startRadius;
	private float _currentRadius;
	private float _height = 6.0f;
	private int _sides = 30;
	private const float ErrorCoeff = 0.25f;
	private bool _isDontMove;
	private Vector3 _startPosition;

	private Cylinder _script;
	private Vector3 _startCursorPoint;
	private bool _isMoveToExit;
	private float _moveToExitSpeed;

	private int _comboCounter;
	private bool _isHaveCollision;
	private const float BonusRadiusCoeff = 0.5f;

	private float _startDistance;
	private float _currentAngle;
	private const float CirclePositionY = 50f;

	void Start ()
	{
		_startPosition = transform.position;
		Respown();

		_startDistance = Vector3.Distance(new Vector3(0f, CirclePositionY, transform.position.z), transform.position);
		_currentAngle = Mathf.Atan2(CirclePositionY-transform.position.y, transform.position.x) * Mathf.Rad2Deg;
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
		GameEvents.Send(OnTubeCreate, _currentRadius);
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
		
		if (_currentRadius > ErrorCoeff)
		{
			if (cutSize > ErrorCoeff)
			{
				ChangeSize();
				CreateCutTube(cutSize, other.GetComponent<MyTube>().Speed);
				_comboCounter = 0;
				other.gameObject.GetComponent<Outline>().color = 1;
			}
			if (_comboCounter >= 3)
			{
				float newSize = _currentRadius + BonusRadiusCoeff;
				if (newSize > _startRadius) newSize = _startRadius;
				GameEvents.Send(OnTubeCreate, newSize);
				GameEvents.Send(OnTubeGoodAnimation);
				other.gameObject.GetComponent<Outline>().color = 0;
			}
			else
			{
				GameEvents.Send(OnTubeCreate, _currentRadius);
			}
		}
		else
		{
			other.gameObject.GetComponent<Outline>().color = 2;
			GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.0f/255f, 0f/255f));
			_isMoveToExit = true;
			_moveToExitSpeed = other.GetComponent<MyTube>().Speed;
			GlobalEvents<OnGameOver>.Call(new OnGameOver());
		}
		_isDontMove = true;
//		GetComponent<Outline>().enabled = true;
	}

	private void OnCanMove()
	{
		_isDontMove = false;
		if (_comboCounter >= 3)
		{
			_comboCounter = 0;
			_currentRadius += BonusRadiusCoeff;
			if (_currentRadius > _startRadius) _currentRadius = _startRadius;
			ChangeSize();
		}
		_isHaveCollision = false;
//		GetComponent<Outline>().enabled = false;
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
			float newX = (_startCursorPoint.x - cursorPosition.x) / 6f;
			_currentAngle += newX;
			transform.position = new Vector3 (_startDistance * Mathf.Cos(_currentAngle * Mathf.Deg2Rad),
				CirclePositionY - _startDistance * Mathf.Sin (_currentAngle * Mathf.Deg2Rad), transform.position.z);
			_startCursorPoint = cursorPosition;
		}
	}
}
