using System;
using PrimitivesPro.GameObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class TubeManager : MonoBehaviour {
	public static event Action OnCreateCoin;
	[SerializeField] private AudioClip _gameStart;
	[HideInInspector] public const float Height = 3.0f;
	private const int Sides = 30;
	private const float OuterRadiusMax = 22f;
	private const float MaxSpeed = 145f;
	private const float StartSpeed = 100f;
	private float _acceleration = 2.9f;
	[HideInInspector] public static float CurrentSpeed = StartSpeed;
	private bool _isWantBonusTube;
	private float _radiusAddCoeff = 5.0f;
	private int _counter;
	private const float RadiusMinus = 1.25f;
	public static float RotateSpeed = 1f;

	private void OnEnable()
	{
		Player.OnTubeCreate += OnTubeCreate;
		Player.OnTubeGetBonusTube += OnTubeGetBonusTube;
		GlobalEvents<OnStartGame>.Happened += StartGame;
		GlobalEvents<OnGameOver>.Happened += OnGameOver;
	}

	private void OnGameOver(OnGameOver obj)
	{
		_radiusAddCoeff = 5f;
		_counter = 0;
	}

	private void StartGame(OnStartGame obj)
	{
		CurrentSpeed = StartSpeed;
	}
	
	private void OnTubeGetBonusTube()
	{
		_isWantBonusTube = true;
	}

	private void OnTubeCreate(float radius)
	{
		Color color;
		float outerRadius;
		float newRadius = radius + _radiusAddCoeff;
		_radiusAddCoeff -= RadiusMinus;
		if (_radiusAddCoeff < 0f) _radiusAddCoeff = 0f;
		
		if (_isWantBonusTube)
		{
			color = new Color(255f / 255.0f, 201f / 255f, 104f / 255f);
			newRadius += 0.5f;
			outerRadius = newRadius + 2.5f;
			_isWantBonusTube = false;
		}
		else
		{
			color = ColorTheme.GetTubeColor();
			outerRadius = Random.Range(newRadius + 2.5f, OuterRadiusMax);
		}
		
		++_counter;
		CreateTube(newRadius, outerRadius, color);
		if (_counter != 0 && _counter % 5 == 0) 
//			CreateCoin(currentTub.transform.position, newRadius + currentTub.GetComponent<Tube>().height*0.7f);
		GameEvents.Send(OnCreateCoin);
	}

	private void CreateTube(float radius, float outerRadius, Color color)
	{
		BaseObject shapeObject = Tube.Create(radius, outerRadius, Height + Random.value*2.5f, Sides, 1, 0.0f, false,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Botttom);
		
		shapeObject.AddMeshCollider(true);
		
		GameObject currentTube = shapeObject.gameObject;
		currentTube.GetComponent<Renderer>().material = new Material(GetDiffuseShader());
		currentTube.GetComponent<Renderer>().material.SetColor("_Color", color);
		currentTube.transform.position = new Vector3(Random.Range(-11f, 11f), 200f, 0f);
		MyTube script = currentTube.AddComponent<MyTube>();
		if (CurrentSpeed < 140f) _acceleration = 2.9f; else _acceleration = 2.0f;
		CurrentSpeed += _acceleration;
		if (CurrentSpeed > MaxSpeed) CurrentSpeed = MaxSpeed;
		script.Speed = CurrentSpeed;
		currentTube.GetComponent<Collider>().isTrigger = true;
	}

	Shader GetDiffuseShader()
	{
		return Shader.Find("Diffuse");
	}

	private void Update()
	{
		if (DefsGame.CurrentScreen == DefsGame.SCREEN_MENU
		    && InputController.IsTouchOnScreen(TouchPhase.Began))
		{
			GlobalEvents<OnStartGame>.Call(new OnStartGame());
			Defs.PlaySound(_gameStart);
		}
	}
	
}
