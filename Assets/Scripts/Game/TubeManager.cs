using System;
using System.Collections.Generic;
using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class TubeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _tubes;
    [SerializeField] private Color[] _colors;
    
    public static float CurrentSpeed = StartSpeed;
    public static event Action <float> OnTubesSpeedScale;
    public static event Action OnCreateCoin;
	public static event Action OnCreateBonusIncrease;
	public static event Action OnCreateChar;
//	public static event Action <Color> OnSendCurrentColorToPlayer;
    
    private const float Height = 7f;
    private const int Sides = 32;
    private const float OuterRadius = 14f;
    public static readonly float InitRadius = 7f;
    
    private const float MaxSpeed = 200f;
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
//        GameEvents.Send(OnSendCurrentColorToPlayer, _colors[DefsGame.CurrentFaceId]);
    }

    private void OnEnable()
    {
        MyTube.OnDestroy += RemoveItem;
        MyPlayer.OnTubeCreate += OnTubeCreate;
        GlobalEvents<OnStartGame>.Happened += StartGame;
        GlobalEvents<OnGameOver>.Happened += OnGameOver;
        GlobalEvents<OnShowMenu>.Happened += OnShowMenu;
        GlobalEvents<OnNoGameOverButtons>.Happened += OnNoGameOverButtons;
        GlobalEvents<OnWordNeedToWait>.Happened += OnWordNeedToWait;
        GlobalEvents<OnWordCollected>.Happened += OnWordCollected;
        GlobalEvents<OnWordsAvailable>.Happened += OnWordsAvailable;
        GlobalEvents<OnTubeCreateExample>.Happened += OnTubeCreateExample;
//        GlobalEvents<OnChangeSkin>.Happened += OnChangeSkin;
    }
    
//    private void OnChangeSkin(OnChangeSkin obj)
//    {
//        GameEvents.Send(OnSendCurrentColorToPlayer, _colors[DefsGame.CurrentFaceId]);
//    }

    private void OnTubeCreateExample(OnTubeCreateExample obj)
    {
        CreateTube(InitRadius, _colors[DefsGame.CurrentFaceId], 200f, true);
    }

    private void OnShowMenu(OnShowMenu obj)
    {
        _radiusAddCoeff = 5f;
        _counter = 0;
        CreateTubeStart();
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
        GameEvents.Send(OnTubesSpeedScale, CurrentSpeed/MaxSpeed);
    }

    private void CreateTubeStart()
    {
        ColorTheme.SetFirstColor();
        
        var newRadius = InitRadius + _radiusAddCoeff;
        for (int i = 0; i < 3; i++)
        {
            _radiusAddCoeff -= StartRadiusMinus;
            if (_radiusAddCoeff < 0f) _radiusAddCoeff = 0f;

            Color color = _colors[DefsGame.CurrentFaceId];
            if (Math.Abs(_colors[DefsGame.CurrentFaceId].a) < 0.1f)
            {
                color = ColorTheme.GetTubeColor();
            }
            
            CreateTube(newRadius, color, 600f - i*200f);
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
        
        if (_counter % 12 == 0)
        {
            ColorTheme.GetNextRandomId();
            _increaseCounter = 0;
        }

        var newRadius = radius + _radiusAddCoeff;
        _radiusAddCoeff -= StartRadiusMinus;
        if (_radiusAddCoeff < 0f) _radiusAddCoeff = 0f;

        Color color = _colors[DefsGame.CurrentFaceId];
        if (Math.Abs(_colors[DefsGame.CurrentFaceId].a) < 0.1f)
        {
            color = ColorTheme.GetTubeColor();
        }

        CreateTube(newRadius, color);
        IncreaseSpeed();
        bool isBonusCreated = false;
        
        ++_increaseCounter;
        if (_increaseCounter >= 12)
        {
            GameEvents.Send(OnCreateBonusIncrease);
            isBonusCreated = true;
            _increaseCounter = 0;
        }

		++_coinCounter;

		if (!isBonusCreated) {
			if (_coinCounter % 6 == 0) {			
			    if (Random.value > 0.5f) {
					GameEvents.Send (OnCreateCoin);
					isBonusCreated = true;
			    }
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
    
    private void CreateTube(float radius, Color color, float posY = 600f, bool startPos = false)
    {
        float outer = OuterRadius;
        if (DefsGame.CurrentFaceId == 0)
        {
            outer += 7f;
        }
    
        BaseObject shapeObject = Tube.Create(radius, outer, Height, Sides, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);
        
        shapeObject.AddMeshCollider(true);

        var currentTube = shapeObject.gameObject;
        currentTube.GetComponent<Renderer>().material = new Material(GetShader());
//        StandardShaderUtils.ChangeRenderMode(currentTube.GetComponent<Renderer>().material,
//            StandardShaderUtils.BlendMode.Opaque);
        Material _material = currentTube.GetComponent<Renderer>().material;
        _material.SetColor("_Color", color);
        currentTube.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        currentTube.GetComponent<Renderer>().receiveShadows = false;
        _material.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
        _material.SetFloat("_SpecularHighlights",1f);
        if (startPos) currentTube.transform.position = new Vector3(0f, posY, 0f); 
        else currentTube.transform.position = new Vector3(Random.Range(-12f, 12f), posY, 0f);
        var script = currentTube.AddComponent<MyTube>();
        script.ShapeObject = shapeObject;
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
        if (CurrentSpeed < 175f) _acceleration = 0.95f; else
        if (CurrentSpeed < 180f) _acceleration = 0.90f; else
            _acceleration = 0.5f;
        CurrentSpeed += _acceleration;
        if (CurrentSpeed > MaxSpeed) CurrentSpeed = MaxSpeed;
        GameEvents.Send(OnTubesSpeedScale, CurrentSpeed/MaxSpeed);
    }

    private Shader GetShader()
    {
        return Shader.Find("Standard");
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