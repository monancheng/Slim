using System;
using PrimitivesPro.GameObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
	public static event Action <float> OnTubeCreate;
	public static event Action OnTubeMove;
	public static event Action OnTubeGetBonusTube;
	public static event Action <int, float, GameObject> OnCombo;
	
	[SerializeField] private AudioClip[] SoundsGood;
	private int _soundGoodID;
	[SerializeField] private AudioClip[] SoundsGrow;
	[SerializeField] private AudioClip SoundSlice;
	
	private float _startRadius;
	private float _currentRadius;
	private float _height;
	private int _sides;
	private const float ErrorCoeff = 0.63f;
	private bool _isDontMove;
	private Vector3 _startPosition;

	private Cylinder _script;
	private Vector3 _startCursorPoint;
	private bool _isMoveToExit;
	private float _moveToExitSpeed;

	private int _comboCounter;
	private int _comboIncreaseCounter;
	private bool _isHaveCollision;
	private const float BonusRadiusCoeff = 0.5f;

	private float _startDistance;
	private float _currentAngle;
	private const int ComboForIncrease = 3;
	private const float CirclePositionY = 80f;
	
	void Start ()
	{
		_startPosition = transform.position;
		_script = GetComponent<Cylinder>();
		_script.AddMeshCollider(true);
		_height = _script.height;
		_sides = _script.sides;
		_startRadius = _script.radius;
		
		Respown();

		_startDistance = Vector3.Distance(new Vector3(0f, CirclePositionY, transform.position.z), transform.position);
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
		
		_currentAngle = Mathf.Atan2(CirclePositionY-transform.position.y, transform.position.x) * Mathf.Rad2Deg;
		
		_comboCounter = 0;
		_comboIncreaseCounter = 0;
		_isMoveToExit = false;
		_currentRadius = _startRadius;
		_isDontMove = false;
		_isHaveCollision = false;
		
		GetComponent<Renderer>().material.SetColor("_Color", ColorTheme.GetPlayerColor());

		ChangeSize();
	}

	private void StartGame(OnStartGame obj)
	{
		// Создаем туб
		GameEvents.Send(OnTubeCreate, _currentRadius);
		// Даем ему команду двигаться
		GameEvents.Send(OnTubeMove);
	}
	
	Shader GetTransparentDiffuseShader()
	{
		return Shader.Find("Transparent/Diffuse");
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Coin")) return;
		
		if (_isHaveCollision) return;
		
		_isHaveCollision = true;
		float diffX = Mathf.Abs(transform.position.x - other.gameObject.transform.position.x);
		float cutSize = diffX - (other.gameObject.GetComponent<Tube>().radius0 - _currentRadius);
		if (cutSize < 0f) cutSize = 0f;
 		
		if (cutSize > ErrorCoeff)
		{
			_currentRadius -= cutSize;
			_comboCounter = 0;
			_comboIncreaseCounter = 0;
		}
		else
		{
			++_comboCounter;
			++_comboIncreaseCounter;
		}
		
		if (_currentRadius > ErrorCoeff)
		{
			if (cutSize > ErrorCoeff)
			{
				ChangeSize();
				CreateCutTube(cutSize, other.GetComponent<MyTube>().Speed);
				_comboCounter = 0;
				_comboIncreaseCounter = 0;
				_soundGoodID = 0;
				Defs.PlaySound(SoundSlice, 0.3f);
			}
			else
			{
				if (_comboIncreaseCounter == ComboForIncrease && _currentRadius < _startRadius)
				{
					GameEvents.Send(OnTubeGetBonusTube);
				}
				
				GameEvents.Send(OnCombo, _comboCounter, _currentRadius, other.gameObject);
				Defs.PlaySound(GetNextGoodSound());
			}
			GlobalEvents<OnPointsAdd>.Call(new OnPointsAdd{PointsCount = /*_comboCounter+*/1});
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
		_isHaveCollision = false;

		CheckCombo();
		
		// Создаем туб
		GameEvents.Send(OnTubeCreate, _currentRadius);
		// Даем ему команду двигаться
		GameEvents.Send(OnTubeMove);
	}

	private void CheckCombo()
	{
		if (_comboIncreaseCounter > ComboForIncrease)
		{
			_comboIncreaseCounter = 0;
			if (_currentRadius < _startRadius)
			{
				_currentRadius += BonusRadiusCoeff;
				Defs.PlaySound(GetRandomGrowSound());
			} else _currentRadius = _startRadius;
			ChangeSize();
			
		}
	}

	private void ChangeSize()
	{
		_script.GenerateGeometry(_currentRadius, _height, _sides, 1,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Center);
		_script.FitCollider();
	}
	
	private void CreateCutTube(float cutSize, float speed)
	{
		BaseObject shapeObject = Tube.Create(_currentRadius, _currentRadius + cutSize, _height, 12, 1, 0.0f, false,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Center);
		
		GameObject go = shapeObject.gameObject;
		go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
		go.GetComponent<Renderer>().material.SetColor("_Color", new Color(255f / 255.0f, 201f / 255f, 104f / 255f));
		go.transform.position = transform.position;
		PlayerTubeBad pt = go.AddComponent<PlayerTubeBad>();
		pt.Speed = speed;
	}

	private void Update()
	{
		if (_isMoveToExit)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - _moveToExitSpeed*Time.deltaTime, transform.position.z);
			return;
		}
		
		transform.Rotate(Vector3.up, -1f);
		
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
			float newX = (_startCursorPoint.x - cursorPosition.x) / 13f;
			_currentAngle += newX;
			transform.position = new Vector3 (_startDistance * Mathf.Cos(_currentAngle * Mathf.Deg2Rad),
				CirclePositionY - _startDistance * Mathf.Sin (_currentAngle * Mathf.Deg2Rad), transform.position.z);
			_startCursorPoint = cursorPosition;
		}
	}
	
	private AudioClip GetNextGoodSound()
	{
		++_soundGoodID;
		if (_soundGoodID > SoundsGood.Length - 1) _soundGoodID = 0;
		return SoundsGood[_soundGoodID];
	}
	
	private AudioClip GetRandomGrowSound() {
		return SoundsGrow[(int) Mathf.Round(Random.value*(SoundsGrow.Length-1))];
	}
}
