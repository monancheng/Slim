using System;
using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip[] SoundsGood;
    [SerializeField] private AudioClip SoundSlice;
    public static event Action<float> OnTubeCreate;
    public static event Action OnTubeMove;
    public static event Action OnTubeGetBonusTube;
    public static event Action<int, float, Vector3, float> OnCombo;
    public static event Action<float> OnIncreaseTubeRadius;

    private const float MinSize = 1.0f;
    private const float ErrorCoeff = 0.63f;
    private const float BonusRadiusCoeff = 2.0f;
    private const int ComboForIncrease = 4;
    private const float CirclePositionY = 80f;

//    private int _comboCounter;
    private int _comboIncreaseCounter;
    private float _currentAngle;
    private float _currentRadius;
    private float _height;
    private bool _isDontMove;
    private bool _isHaveCollision;
    private bool _isMoveToExit;

    private Cylinder _script;
    private int _sides;
    private int _soundGoodId;
    private Vector3 _startCursorPoint;

    private float _startDistance;
    private Vector3 _startPosition;
    private float _startRadius;
    private Vector3 _cameraStartPosition;
    private bool _isIncreaseSize;

    private void Start()
    {
        _cameraStartPosition = Camera.main.transform.position;
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
        BonusIncrease.OnBonusGrow += OnGrow;
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

        _currentAngle = Mathf.Atan2(CirclePositionY - transform.position.y, transform.position.x) * Mathf.Rad2Deg;

//        _comboCounter = 0;
        _comboIncreaseCounter = 0;
        _isMoveToExit = false;
        _currentRadius = _startRadius;
        _isDontMove = false;
        _isHaveCollision = false;

//        GetComponent<Renderer>().material.SetColor("_Color", ColorTheme.GetPlayerRandomColor());
        GetComponent<Renderer>().material.SetColor("_Color", ColorTheme.GetPlayerStartColor());

        ChangeSize();
    }

    private void StartGame(OnStartGame obj)
    {
        // Создаем туб
//        GameEvents.Send(OnTubeCreate, _startRadius);
        // Даем ему команду двигаться
        GameEvents.Send(OnTubeMove);
    }

    private Shader GetTransparentDiffuseShader()
    {
        return Shader.Find("Transparent/Diffuse");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) return;

        if (_isHaveCollision) return;

//        MyTube tube = other.gameObject.GetComponent <MyTube>();
//        if (tube.IsIncreaseSize) _isIncreaseSize = true;

        _isHaveCollision = true;
        var diffX = Mathf.Abs(transform.position.x - other.gameObject.transform.position.x);
        var cutSize = diffX - (other.gameObject.GetComponent<Tube>().radius0 - _currentRadius);
        if (cutSize < 0f) cutSize = 0f;

        if (cutSize > ErrorCoeff)
        {
            _currentRadius -= cutSize;
//            _comboCounter = 0;
            _comboIncreaseCounter = 0;
        }
        else
        {
//            ++_comboCounter;
            ++_comboIncreaseCounter; 
        }

        if (_currentRadius > MinSize)
        {
            if (cutSize > ErrorCoeff)
            {
                ChangeSize();
                CreateCutTube(cutSize);
//                _comboCounter = 0;
                _comboIncreaseCounter = 0;
                _soundGoodId = 0;
                Defs.PlaySound(SoundSlice, 0.3f);
                _isIncreaseSize = false;
            }
            else
            {
//                if (_comboIncreaseCounter == ComboForIncrease && _currentRadius < _startRadius)
//                {
//                    GameEvents.Send(OnTubeGetBonusTube);
//                    _comboIncreaseCounter = 0;
//                }
                
                GameEvents.Send(OnCombo, 1/*_comboCounter*/, _currentRadius, 
                    new Vector3(transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z),
                    other.gameObject.GetComponent<Tube>().height);
                Defs.PlaySound(GetNextGoodSound());
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

//    private void CheckCombo()
//    {
//        if (_isIncreaseSize)
//        {
//            _isIncreaseSize = false;
//            if (_currentRadius < _startRadius)
//            {
//                _currentRadius += BonusRadiusCoeff;
//            }
//            else
//            {
//                _currentRadius = _startRadius;
//            }
//            ChangeSize();
//            GameEvents.Send(OnIncreaseTubeRadius, _currentRadius);
//        }
//    }
    
    private void OnGrow()
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
        GameEvents.Send(OnIncreaseTubeRadius, _currentRadius);
    }

    private void ChangeSize()
    {
        _script.GenerateGeometry(_currentRadius, _height, _sides, 1,
            NormalsType.Vertex,
            PivotPosition.Center);
        _script.FitCollider();
    }

    private void CreateCutTube(float cutSize)
    {
        BaseObject shapeObject = Tube.Create(_currentRadius, _currentRadius + cutSize, _height, 12, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);

        var go = shapeObject.gameObject;
        go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
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

        transform.Rotate(Vector3.up, -1f);

        if (_isDontMove)
        {
            _startCursorPoint = Input.mousePosition;
            return;
        }

        if (InputController.IsTouchOnScreen(TouchPhase.Began))
            _startCursorPoint = Input.mousePosition;

        if (InputController.IsTouchOnScreen(TouchPhase.Moved))
        {
            Vector2 cursorPosition = Input.mousePosition;
            var newX = (_startCursorPoint.x - cursorPosition.x) / 11f;
            _currentAngle += newX;
            float xCoeff = _startDistance * Mathf.Cos(_currentAngle * Mathf.Deg2Rad);
            float yCoeff = _startDistance * Mathf.Sin(_currentAngle * Mathf.Deg2Rad);
            transform.position = new Vector3(xCoeff,
                CirclePositionY - yCoeff, transform.position.z);
            
            Camera.main.transform.position = new Vector3(_cameraStartPosition.x + xCoeff*0.1f, _cameraStartPosition.y, _cameraStartPosition.z);
            _startCursorPoint = cursorPosition;
        }
    }
    
    private AudioClip GetNextGoodSound()
    {
        ++_soundGoodId;
        if (_soundGoodId > SoundsGood.Length - 1) _soundGoodId = 0;
        return SoundsGood[_soundGoodId];
    }
}