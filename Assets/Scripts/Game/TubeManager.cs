using System;
using System.Collections.Generic;
using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;
using UnityEngine.Rendering;
using VoxelBusters.Utility;
using Random = UnityEngine.Random;

public class TubeManager : MonoBehaviour
{
    [SerializeField] private GameObject _pointLight;
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
    
    private const float MaxSpeed = 225f;
    private float _acceleration = 2.9f;
    
    private const float StartSpeed = 145f;
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
    private Color _playerColor;
    private bool isTubeAngleRight;
    private float _tubeAngle;

    private void Start()
    {
        _radiusAddCoeff = 5f;
        _counter = 0;
        
        ColorTheme.SetFirstColor();
        CreateTubeStart();
//        GameEvents.Send(OnSendCurrentColorToPlayer, _colors[DefsGame.CurrentFaceId]);
    }

    private void OnEnable()
    {
        MyTube.OnDestroy += RemoveItem;
        MyPlayer.OnTubeCreate += OnTubeCreate;
        MyPlayer.OnChangeColor += OnChangeColor;
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

    private void OnChangeColor(Color obj)
    {
        _playerColor = obj;
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
        if (DefsGame.QUEST_GAMEPLAY_Counter >= 1)
            ColorTheme.GetNextRandomId();

        _tubeAngle = 0;
        
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
            
            CreateTube(newRadius, color, 660f - i*220f);
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

        CreateTube(newRadius, color, 660);
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
			    if (Random.value > 0.3f) {
					GameEvents.Send (OnCreateCoin);
					isBonusCreated = true;
			    }
			}
		}
			
		if (!isBonusCreated && !_isWordWait && _isWordActive) {
			if (_coinCounter % 15 == 0) {
//				if (Random.value > 0.5f) {
					GameEvents.Send (OnCreateChar);
					isBonusCreated = true;
//				}
			}
		}
    }
    
    private void CreateTube(float radius, Color color, float posY, bool startPos = false)
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
        
        currentTube.layer = LayerMask.NameToLayer("Rings");
        Light light = currentTube.AddComponent<Light>();
        light.color = new Color(_playerColor.r,_playerColor.g, _playerColor.b, 1f);
        light.range = 30*radius/InitRadius;
        light.intensity = 5;
        light.cullingMask = 1 << currentTube.layer;

        currentTube.transform.Rotate(Vector3.up, _tubeAngle);
        
        if (isTubeAngleRight)
        {
            _tubeAngle += 2f;
            if (_tubeAngle >= 10f)
            {
                isTubeAngleRight = false;
            }
        }
        else
        {
            _tubeAngle -= 2f;
            if (_tubeAngle <= -10f)
            {
                isTubeAngleRight = true;
            }
        }

//        GameObject pointLightGo = Instantiate(_pointLight);
//        Light light = pointLightGo.GetComponent<Light>();
//        light.color = _playerColor;
//        pointLightGo.transform.SetParent(transform, false);
//        pointLightGo.transform.localPosition = Vector3.zero;
    }

    private void IncreaseSpeed()
    {
        if (CurrentSpeed < 150f) _acceleration = 3.0f; else
        if (CurrentSpeed < 155f) _acceleration = 2.7f; else
        if (CurrentSpeed < 160f) _acceleration = 2.3f; else
        if (CurrentSpeed < 165f) _acceleration = 1.8f; else
        if (CurrentSpeed < 170f) _acceleration = 1.4f; else
        if (CurrentSpeed < 175f) _acceleration = 1.2f; else
        if (CurrentSpeed < 180f) _acceleration = 1.1f; else
        if (CurrentSpeed < 185f) _acceleration = 1.0f; else
        if (CurrentSpeed < 190f) _acceleration = 0.90f; else
        if (CurrentSpeed < 195f) _acceleration = 0.85f; else
        if (CurrentSpeed < 200f) _acceleration = 0.80f; else
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