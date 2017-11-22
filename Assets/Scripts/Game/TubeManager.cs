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
    [SerializeField] private GameObject[] _smiles;
    private int _smileID;
    
    private const float StartSpeed = 150f;
    private const float StartRadiusMinus = 1.5f;
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
    
    private const float MaxSpeed = 229f;
    private float _acceleration = 2.9f;
    
    private int _counter;
    private bool _isWantBonusTube;
    private float _radiusAddCoeff = 4.5f;
    private readonly List<MyTube> _itemList = new List<MyTube>();
    private int _coinCounter;
    private int _increaseCounter;
    private bool _isWordWait;
    private bool _isWordActive;
    private Color _playerColor;
    private bool _isTubeRotationRight;

    private void Start()
    {
        _radiusAddCoeff = 5f;
        _counter = 0;
        _smileID = 0;
        
        ColorTheme.SetFirstColor();
        CreateTubeStart();
//        GameEvents.Send(OnSendCurrentColorToPlayer, _colors[ScreenSkins.CurrentFaceId]);
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
//        GameEvents.Send(OnSendCurrentColorToPlayer, _colors[ScreenSkins.CurrentFaceId]);
//    }

    private void OnTubeCreateExample(OnTubeCreateExample obj)
    {
        CreateTube(InitRadius, _colors[ScreenSkins.CurrentFaceId], 200f, true);
    }

    private void OnShowMenu(OnShowMenu obj)
    {
        _radiusAddCoeff = 5f;
        _counter = 0;
        _smileID = 0;
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
		_coinCounter = 2;
        GameEvents.Send(OnTubesSpeedScale, CurrentSpeed/MaxSpeed);
    }

    private void CreateTubeStart()
    {
        if (PrefsManager.QuestGameplayCounter >= 1)
            ColorTheme.GetNextRandomId();

//        _tubeAngle = 0;
        
        for (int i = 0; i < 3; i++)
        {
            float newRadius = InitRadius + _radiusAddCoeff;
            
            Color color = _colors[ScreenSkins.CurrentFaceId];
            if (Math.Abs(_colors[ScreenSkins.CurrentFaceId].a) < 0.1f)
            {
                color = ColorTheme.GetTubeColor();
            }
            
            CreateTube(newRadius, color, (i+1)*220f);
            _radiusAddCoeff -= StartRadiusMinus;
            if (_radiusAddCoeff < 0f) _radiusAddCoeff = 0f;
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

        Color color = _colors[ScreenSkins.CurrentFaceId];
        if (Math.Abs(_colors[ScreenSkins.CurrentFaceId].a) < 0.1f)
        {
            color = ColorTheme.GetTubeColor();
        }

        CreateTube(newRadius, color, 660);
        IncreaseSpeed();
        bool isBonusCreated = false;
        
        ++_increaseCounter;
        if (_increaseCounter >= 12)
        {
            if (Random.value > 0.4f) 
            {
                GameEvents.Send(OnCreateBonusIncrease);
                isBonusCreated = true;
            }
            _increaseCounter = 0;
        }

		++_coinCounter;

		if (!isBonusCreated) {
			if (_coinCounter % 6 == 0) {			
			    if (Random.value > 0.25f) {
					GameEvents.Send (OnCreateCoin);
					isBonusCreated = true;
			    }
			}
		}
			
		if (!isBonusCreated && !_isWordWait && _isWordActive) {
			if (_coinCounter % 15 == 0) {
				if (Random.value > 0.3f) {
					GameEvents.Send (OnCreateChar);
					isBonusCreated = true;
				}
			}
		}
    }
    
    private void CreateTube(float radius, Color color, float posY, bool startPos = false)
    {
        float outer = OuterRadius;
        if (ScreenSkins.CurrentFaceId == 0)
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
        Material material = currentTube.GetComponent<Renderer>().material;
        material.SetColor("_Color", color);
        currentTube.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        currentTube.GetComponent<Renderer>().receiveShadows = false;
        material.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
        material.SetFloat("_SpecularHighlights",1f);
        if (startPos) currentTube.transform.position = new Vector3(0f, posY, 0f); 
        else currentTube.transform.position = new Vector3(Random.Range(-12f, 12f), posY, 0f);
        var script = currentTube.AddComponent<MyTube>();
        
        script.ShapeObject = shapeObject;
        if (ScreenSkins.CurrentFaceId == 3)
        {
            _smileID = Random.Range(0, _smiles.Length);
//            if (_counter > 0 && _counter % 10 == 0)
//            {
//                _smileID += 1;
//                if (_smileID >= _smiles.Length) _smileID = 0;
//            }
            script.CreateTubeModel(_smiles[_smileID]);
        }
        else
        {
            script.CreateTubeModel(_tubes[ScreenSkins.CurrentFaceId]);
        }
        script.ChangeRadius(radius/InitRadius);
        _itemList.Add(script);
        currentTube.GetComponent<Collider>().isTrigger = true;
        
        currentTube.layer = LayerMask.NameToLayer("Rings");
        Light lightTmp = currentTube.AddComponent<Light>();
        lightTmp.color = new Color(_playerColor.r,_playerColor.g, _playerColor.b, 1f);
        lightTmp.range = 30*radius/InitRadius;
        lightTmp.intensity = 5;
        lightTmp.cullingMask = 1 << currentTube.layer;
        
        if (ScreenSkins.CurrentFaceId == 7)
        {
            _isTubeRotationRight = !_isTubeRotationRight;
            if (_isTubeRotationRight)
                script.RotationSpeed = 2f; else script.RotationSpeed = -2f;
        }
        else if (ScreenSkins.CurrentFaceId == 2 || ScreenSkins.CurrentFaceId == 9)
        {
            if (_isTubeRotationRight) script.ModelGameObject.transform.localScale = new Vector3(
                script.ModelGameObject.transform.localScale.x*-1f,
                script.ModelGameObject.transform.localScale.y,
                script.ModelGameObject.transform.localScale.z);
            _isTubeRotationRight = !_isTubeRotationRight;
        }

//        currentTube.transform.Rotate(Vector3.up, _tubeAngle);
        
//        if (_isTubeAngleRight)
//        {
//            _tubeAngle += 3f;
//            if (_tubeAngle >= 9f)
//            {
//                _isTubeAngleRight = false;
//            }
//        }
//        else
//        {
//            _tubeAngle -= 3f;
//            if (_tubeAngle <= -9f)
//            {
//                _isTubeAngleRight = true;
//            }
//        }
    }

    private void IncreaseSpeed()
    {
        if (CurrentSpeed < 158f) _acceleration = 3.1f; else
        if (CurrentSpeed < 163f) _acceleration = 2.5f; else
        if (CurrentSpeed < 168f) _acceleration = 1.8f; else
        if (CurrentSpeed < 173f) _acceleration = 1.4f; else
        if (CurrentSpeed < 178f) _acceleration = 1.2f; else
        if (CurrentSpeed < 183f) _acceleration = 1.1f; else
        if (CurrentSpeed < 188f) _acceleration = 1.0f; else
        if (CurrentSpeed < 193f) _acceleration = 0.90f; else
        if (CurrentSpeed < 198f) _acceleration = 0.85f; else
        if (CurrentSpeed < 203f) _acceleration = 0.80f; else
            _acceleration = 0.5f;
        CurrentSpeed += _acceleration;
        if (CurrentSpeed > MaxSpeed) CurrentSpeed = MaxSpeed;
        GameEvents.Send(OnTubesSpeedScale, CurrentSpeed/MaxSpeed);
    }

    private Shader GetShader()
    {
        return Shader.Find("Standard");
    }
}