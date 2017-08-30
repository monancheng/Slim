using System;
using System.Collections.Generic;
using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;
using Random = UnityEngine.Random;

public class TubeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _tubes;
    [HideInInspector] public const float Height = 3.0f;
    [HideInInspector] public static float CurrentSpeed = StartSpeed;
    public static event Action OnCreateCoin;
	public static event Action OnCreateBonusIncrease;
	public static event Action OnCreateChar;
    public static float RotateSpeed = 1f;

    private const int Sides = 32;
    
    private const float OuterRadiusMul = 1.8f;
    private const float OuterRadiusMaxAdd = 8f;
    private const float OuterRadiusMinAdd = 4f;
    private const float InitRadius = 7f;
    private const float RadiusMin = 10f;
    private const float RadiusMax = 12f;
    
    private const float MaxSpeed = 195f;
    private float _acceleration = 2.9f;
    
    private const float StartSpeed = 133f;
    private const float StartRadiusMinus = 1.25f;
    
    private int _counter;
    private bool _isWantBonusTube;
    private float _radiusAddCoeff = 5.0f;
    private readonly List<MyTube> _itemList = new List<MyTube>();
    private int _coinCounter;
    private int _increaseCounter;
    private bool _isFingerStart;
    private bool _isWordWait;
    private bool _isWordActive;
    

    private void Start()
    {
        _radiusAddCoeff = 5f;
        _counter = 0;
        
        CreateTubeStart();
    }

    private void OnEnable()
    {
        MyTube.OnDestroy += RemoveItem;
        MyPlayer.OnTubeCreate += OnTubeCreate;
//        Player.OnTubeGetBonusTube += OnTubeGetBonusTube;
        GlobalEvents<OnStartGame>.Happened += StartGame;
        GlobalEvents<OnGameOver>.Happened += OnGameOver;
        GlobalEvents<OnShowMenu>.Happened += OnShowMenu;
        GlobalEvents<OnNoGameOverButtons>.Happened += OnNoGameOverButtons;
        GlobalEvents<OnWordNeedToWait>.Happened += OnWordNeedToWait;
        GlobalEvents<OnWordCollected>.Happened += OnWordCollected;
        GlobalEvents<OnWordsAvailable>.Happened += OnWordsAvailable;
    }

    private void OnShowMenu(OnShowMenu obj)
    {
        _radiusAddCoeff = 5f;
        _counter = 0;
        CreateTubeStart();
    }

    public void HideTubes()
    {
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
    }

    private void OnWordsAvailable(OnWordsAvailable obj)
    {
        _isWordActive = obj.IsAvailable;
    }

    private void OnWordNeedToWait(OnWordNeedToWait obj)
    {
        _isWordWait = obj.IsWait;
    }
    
    private void OnWordCollected(OnWordCollected obj)
    {
        _isWordWait = true;
    }

    private void OnNoGameOverButtons(OnNoGameOverButtons obj)
    {
        _isFingerStart = true;
    }

    private void RemoveItem(int id)
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            MyTube item = _itemList[i];
            if (id == item.Id)
            {
                _itemList.RemoveAt(i);
                break;
            }
        }
    }

    private void OnGameOver(OnGameOver obj)
    {
        _radiusAddCoeff = 5f;
        _counter = 0;
        _itemList.Clear();
    }

    private void StartGame(OnStartGame obj)
    {
        CurrentSpeed = StartSpeed;
		_coinCounter = 0;
    }

    private void CreateTubeStart()
    {
        ColorTheme.SetFirstColor();
        
        var newRadius = InitRadius + _radiusAddCoeff;
        for (int i = 0; i < 3; i++)
        {
            _radiusAddCoeff -= StartRadiusMinus;
            if (_radiusAddCoeff < 0f) _radiusAddCoeff = 0f;
        
            float outerRadius = Random.Range(newRadius, newRadius*OuterRadiusMul) - newRadius;
            if (outerRadius > OuterRadiusMaxAdd) outerRadius = OuterRadiusMaxAdd;
            if (outerRadius < OuterRadiusMinAdd) outerRadius = OuterRadiusMinAdd;
            if (newRadius + outerRadius < RadiusMin) outerRadius = RadiusMin - newRadius;
            if (newRadius + outerRadius > RadiusMax) outerRadius = RadiusMax - newRadius;
        
            CreateTube(newRadius, ColorTheme.GetTubeColor(), 600f - i*200f);
        }
        _increaseCounter = 1;
    }

//    private void OnTubeGetBonusTube()
//    {
//        _isWantBonusTube = true;
//    }

    private void OnTubeCreate(float radius)
    {
        ++_counter;
        
        Color color;
        if (_counter % 12 == 0)
        {
            ColorTheme.GetNextRandomId();
            _increaseCounter = 0;
        }
        
        bool isIncreaseSize = false;
        float outerRadius;
        var newRadius = radius + _radiusAddCoeff;
        _radiusAddCoeff -= StartRadiusMinus;
        if (_radiusAddCoeff < 0f) _radiusAddCoeff = 0f;

//        if (_isWantBonusTube)
//        {
//            isIncreaseSize = true;
//            color = new Color(255f / 255.0f, 201f / 255f, 104f / 255f);
//            newRadius += 0.5f;
//            outerRadius = newRadius+OuterRadiusGold;
//            _isWantBonusTube = false;
//        }
//        else
//        {
            color = ColorTheme.GetTubeColor();
            outerRadius = Random.Range(newRadius, newRadius*OuterRadiusMul) - newRadius;
            if (outerRadius > OuterRadiusMaxAdd) outerRadius = OuterRadiusMaxAdd;
            if (outerRadius < OuterRadiusMinAdd) outerRadius = OuterRadiusMinAdd;
            if (newRadius + outerRadius < RadiusMin) outerRadius = RadiusMin - newRadius;
            if (newRadius + outerRadius > RadiusMax) outerRadius = RadiusMax - newRadius;
//        }

        
        CreateTube(newRadius, color, 600f, isIncreaseSize);
        IncreaseSpeed();
        bool isBonusCreated = false;
        if (!isBonusCreated)
        {
            ++_increaseCounter;
            if (_increaseCounter >= 12)
            {
                GameEvents.Send(OnCreateBonusIncrease);
                isBonusCreated = true;
                _increaseCounter = 0;
            }
        }

		++_coinCounter;

		if (!isBonusCreated) {
			
			if (_coinCounter % 5 == 0) {
//				if (Random.value > 0.5f) {
					GameEvents.Send (OnCreateCoin);
					isBonusCreated = true;
//				}
			}
		}
			
		if (!isBonusCreated && !_isWordWait && _isWordActive) {
			if (_coinCounter % 15 == 0) {
				if (Random.value > 0.5f) {
					GameEvents.Send (OnCreateChar);
					isBonusCreated = true;
				}
			}
		}
    }

//    private void CreateTube(float radius, Color color, float posY = 600f,
//        bool isIncreaseSize = false)
//    {
//        GameObject go = Instantiate(_tubes[DefsGame.CurrentFaceId], new Vector3(Random.Range(-12f, 12f), posY, 0f), Quaternion.identity);
//        var script = go.GetComponent<MyTube>();
//        script.IsIncreaseSize = isIncreaseSize;
//        script.ChangeRadius(radius/InitRadius);
//        script.CreateTube(radius, color, posY, isIncreaseSize);
//        _itemList.Add(script);
//    }
    
    private void CreateTube(float radius, Color color, float posY = 600f, bool isIncreaseSize = false)
    {
        BaseObject shapeObject = Tube.Create(radius, InitRadius+5f+2f, Height + Random.value * 2.5f, Sides, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Botttom);
        
        shapeObject.AddMeshCollider(true);

        var currentTube = shapeObject.gameObject;
        currentTube.GetComponent<Renderer>().material = new Material(GetDiffuseShader());
        currentTube.GetComponent<Renderer>().material.SetColor("_Color", color);
        currentTube.transform.position = new Vector3(Random.Range(-12f, 12f), posY, 0f);
        var script = currentTube.AddComponent<MyTube>();
        script.ShapeObject = shapeObject;
        script.IsIncreaseSize = isIncreaseSize;
        script.CreateTubeModel(_tubes[DefsGame.CurrentFaceId]);
        script.ChangeRadius(radius/InitRadius);
        _itemList.Add(script);
        currentTube.GetComponent<Collider>().isTrigger = true;
    }

    private void IncreaseSpeed()
    {
        if (CurrentSpeed < 140f) _acceleration = 2.8f; else
        if (CurrentSpeed < 145f) _acceleration = 2.1f; else
        if (CurrentSpeed < 150f) _acceleration = 1.5f; else
        if (CurrentSpeed < 155f) _acceleration = 1.25f; else
        if (CurrentSpeed < 165f) _acceleration = 1.10f; else
        if (CurrentSpeed < 170f) _acceleration = 1.0f; else
        if (CurrentSpeed < 175f) _acceleration = 0.75f; else
            _acceleration = 0.5f;
        CurrentSpeed += _acceleration;
        if (CurrentSpeed > MaxSpeed) CurrentSpeed = MaxSpeed;
    }

    private Shader GetDiffuseShader()
    {
        return Shader.Find("Diffuse");
    }

    private void Update()
    {
        if ((DefsGame.GameplayCounter == 1||_isFingerStart)
            && (DefsGame.CurrentScreen == DefsGame.SCREEN_MENU || DefsGame.CurrentScreen == DefsGame.SCREEN_NOTIFICATIONS)
            && InputController.IsTouchOnScreen(TouchPhase.Began)
            && InputController.GetPosition().y > 200)
        {
            GlobalEvents<OnStartGame>.Call(new OnStartGame());
            _isFingerStart = false;
        }
    }
}