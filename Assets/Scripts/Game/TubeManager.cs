using cakeslice;
using PrimitivesPro.GameObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class TubeManager : MonoBehaviour {
	private const float Height = 3.5f;
	private const int Sides = 30;
	private const float OuterRadiusMax = 20f;
	private const float MaxSpeed = 1.2f;
	private const float StartSpeed = 0.6f;
	private const float Acceleration = 0.05f;
	private float _currentSpeed = StartSpeed;

	private void OnEnable()
	{
		Player.OnTubeCreate += TubeCreate;
		GlobalEvents<OnStartGame>.Happened += StartGame;
	}

	private void StartGame(OnStartGame obj)
	{
		_currentSpeed = StartSpeed;
	}

	private void TubeCreate(float radius)
	{
		float outerRadius = Random.Range(radius + 2f, OuterRadiusMax);
		
		BaseObject _shapeObject;
		_shapeObject = Tube.Create(radius, outerRadius, Height, Sides, 1, 0.0f, false,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Botttom);
		
		_shapeObject.AddMeshCollider(true);
		
		GameObject currentTube = _shapeObject.gameObject;
		currentTube.GetComponent<Renderer>().material = new Material(GetDiffuseShader());
		currentTube.GetComponent<Renderer>().material.SetColor("_Color",ColorTheme.GetTubeColor());
		currentTube.transform.position = new Vector3(Random.Range(-8f, 8f), 125f, 0f);
		MyTube script = currentTube.AddComponent<MyTube>();
		_currentSpeed += Acceleration;
		if (_currentSpeed > MaxSpeed) _currentSpeed = MaxSpeed;
		script.Speed = _currentSpeed;
		Rigidbody rb = currentTube.AddComponent<Rigidbody>();
		rb.isKinematic = true;
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
		}
	}
	
}
