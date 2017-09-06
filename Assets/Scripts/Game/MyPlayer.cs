using System;
using DarkTonic.MasterAudio;
using DG.Tweening;
using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;
using UnityEngine.Rendering;

public class MyPlayer : MonoBehaviour
{
    public static event Action<float> OnTubeCreate;
    public static event Action OnTubeMove;
    public static event Action<int, float, Vector3, float> OnCombo;
    public static event Action<float> OnIncreaseTubeRadius;

    private const float MinSize = 1.0f;
    private const float ErrorCoeff = 0.63f;
    private const float BonusRadiusCoeff = 2.0f;
    private const float CirclePositionY = 80f;

    private float _currentAngle;
    private float _currentRadius;
    private float _startScaleValue;
    private float _height;
    private bool _isDontMove;
    private bool _isHaveCollision;
    private bool _isMoveToExit;

    private Vector3 _startCursorPoint;

    private float _startDistance;
    private Vector3 _startPosition;
    private float _startRadius;
    private Vector3 _cameraStartPosition;

    [SerializeField] private Material[] _materials;
    private Renderer _renderer;
    [SerializeField] private Mesh[] _meshes;
    private MeshFilter _mesh;
    

    private void Start()
    {
        _cameraStartPosition = Camera.main.transform.position;
        _startPosition = transform.position;
        _height = 24f;
        _startRadius = 7f;

        _startScaleValue = transform.localScale.x;

        _mesh = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();

        RespownAndWait();

        transform.DORotate(new Vector3(0, 0, 20f), 1, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        _startDistance = Vector3.Distance(new Vector3(0f, CirclePositionY, transform.position.z), transform.position);
    }

    private void OnEnable()
    {
        MyTube.OnCanMove += OnCanMove;
        BonusIncrease.OnBonusGrow += OnBonusGrow;
        GlobalEvents<OnStartGame>.Happened += StartGame;
        GlobalEvents<OnGameOver>.Happened += GameOver;
        GlobalEvents<OnChangeSkin>.Happened += OnChangeSkin;
    }

    private void OnChangeSkin(OnChangeSkin obj)
    {
        _renderer.material = _materials[obj.Id];
        _mesh.mesh = _meshes[obj.Id];
    }

    private void GameOver(OnGameOver e)
    {
        Invoke("RespownAndWait", 1f);
    }

    private void RespownAndWait()
    {
        Respown();
        _isDontMove = true;
    }

    private void Respown()
    {
        transform.position = _startPosition;
        Camera.main.transform.position = _cameraStartPosition;
        _currentAngle = Mathf.Atan2(CirclePositionY - transform.position.y, transform.position.x) * Mathf.Rad2Deg;

        _isMoveToExit = false;
        _currentRadius = _startRadius;
        _isDontMove = false;
        _isHaveCollision = false;

        GetComponent<Renderer>().material.SetColor("_Color", ColorTheme.GetPlayerStartColor());

        ChangeSize();
    }

    private void StartGame(OnStartGame obj)
    {
        // Создаем туб
//        GameEvents.Send(OnTubeCreate, _startRadius);
        // Даем ему команду двигаться
        _isDontMove = false;
        _startCursorPoint = Vector3.zero;
        GameEvents.Send(OnTubeMove);
    }

    private Shader GetTransparentDiffuseShader()
    {
        return Shader.Find("Transparent/Diffuse");
    }

    private void OnTriggerEnter(Collider other)
    {
		if (!other.CompareTag("Tube")) return;

        if (_isHaveCollision) return;

        MyTube tube = other.gameObject.GetComponent <MyTube>();
        Tube tubeProc = other.gameObject.GetComponent <Tube>();
//        if (tube.IsIncreaseSize) _isIncreaseSize = true;

        _isHaveCollision = true;
        var diffX = Mathf.Abs(transform.position.x - other.gameObject.transform.position.x);
        var cutSize = diffX - (tube.Scale*_startRadius - _currentRadius);
        if (cutSize < 0f) cutSize = 0f;

        if (cutSize > ErrorCoeff)
        {
            _currentRadius -= cutSize;
        }

        if (_currentRadius > MinSize)
        {
            if (cutSize > ErrorCoeff)
            {
                ChangeSize();
                CreateCutTube(cutSize);
                MasterAudio.PlaySoundAndForget("Slice");
            }
            else
            {
                GameEvents.Send(OnCombo, 1/*_comboCounter*/, _currentRadius, 
                    new Vector3(transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z),
                    tubeProc.height);
                MasterAudio.PlaySoundAndForget("GoodFit");
            }
            GlobalEvents<OnPointsAdd>.Call(new OnPointsAdd {PointsCount = /*_comboCounter+*/1});
        }
        else
        
        {
            GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.0f / 255f, 0f / 255f));
            _isMoveToExit = true;
            GlobalEvents<OnGameOver>.Call(new OnGameOver());
        }
        _isDontMove = true;
    }

    private void OnCanMove()
    {
        _isDontMove = false;
        _isHaveCollision = false;

//        CheckCombo();

        // Создаем туб
        GameEvents.Send(OnTubeCreate, _currentRadius);
        // Даем ему команду двигаться
        GameEvents.Send(OnTubeMove);
    }
    
    private void OnBonusGrow()
    {
        if (_currentRadius < _startRadius)
        {
            _currentRadius += BonusRadiusCoeff;
        }
        else
        {
            _currentRadius = _startRadius;
        }
        ChangeSize();
        GameEvents.Send(OnIncreaseTubeRadius, _currentRadius/_startRadius);
    }

    private void ChangeSize()
    {
        transform.localScale = new Vector3((_currentRadius / _startRadius)*_startScaleValue,
            (_currentRadius / _startRadius)*_startScaleValue,
            transform.localScale.z);
    }

    private void CreateCutTube(float cutSize)
    {
        BaseObject shapeObject = Tube.Create(_currentRadius, _currentRadius + cutSize, _height, 12, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);

        var go = shapeObject.gameObject;
        go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
        go.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        go.GetComponent<Renderer>().receiveShadows = false;
        go.GetComponent<Renderer>().material.SetColor("_Color", new Color(255f / 255.0f, 201f / 255f, 104f / 255f));
        go.transform.position = transform.position;
        go.AddComponent<PlayerTubeBad>();
    }

    private void Update()
    {
        if (_isMoveToExit)
        {
            transform.position = new Vector3(transform.position.x,
                transform.position.y - TubeManager.CurrentSpeed * Time.deltaTime, transform.position.z);
            return;
        }

//        transform.Rotate(Vector3.up, -1f);

        if (_isDontMove)
        {
            _startCursorPoint = Input.mousePosition;
            return;
        }

        if (InputController.IsTouchOnScreen(TouchPhase.Began))
            _startCursorPoint = Input.mousePosition;

        if (InputController.IsTouchOnScreen(TouchPhase.Moved))
        {
            if (_startCursorPoint == Vector3.zero) _startCursorPoint = Input.mousePosition;
            
            Vector2 cursorPosition = Input.mousePosition;
            var newX = (_startCursorPoint.x - cursorPosition.x) / 7f;
            _currentAngle += newX;
            float xCoeff = _startDistance * Mathf.Cos(_currentAngle * Mathf.Deg2Rad);
//            float yCoeff = _startDistance * Mathf.Sin(_currentAngle * Mathf.Deg2Rad);
            
            transform.position = new Vector3(xCoeff,
                transform.position.y,
//                CirclePositionY - yCoeff,
                transform.position.z);
            
            Camera.main.transform.position = new Vector3(_cameraStartPosition.x + xCoeff*0.05f, _cameraStartPosition.y, _cameraStartPosition.z);
            _startCursorPoint = cursorPosition;
        }
    }
}