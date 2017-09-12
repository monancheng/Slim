using System;
using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;

public class MyTube : MonoBehaviour
{
    [HideInInspector] public int Id;
    private bool _isGameOver;
    private bool _isHaveCollision;
    private bool _isMove;
    private bool _isReadyToDelete;
    private bool _isSentMoveEvent;
    [HideInInspector]public BaseObject ShapeObject;

    private float ScaleWeWant;
    private const int IncreaseIterationCount = 20;
    private float _increaseScaleSpeed;
    
   [HideInInspector] public float Scale;
    private bool _isShowAnimation;
    public static event Action OnCanMove;
    public static event Action OnCanSpawnBonus;
    public static event Action <int> OnDestroy;

    private Tube tube;

    private void Awake()
    {
        tag = "Tube";
        _isShowAnimation = true;
        ScaleWeWant = transform.localScale.x;
        _increaseScaleSpeed = ScaleWeWant/IncreaseIterationCount;
        transform.localScale = Vector3.zero;
        tube = GetComponent <Tube>();
        Scale = 1;
    }

    public void CreateTubeModel(GameObject prefab)
    {
        if (prefab == null) return;
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        GlobalEvents<OnGameOver>.Happened += GameOver;
        GlobalEvents<OnHideTubes>.Happened += OnHideTubes;
        MyPlayer.OnTubeMove += OnTubeMove;
        MyPlayer.OnIncreaseTubeRadius += OnIncreaseTubeRadius;
    }

    private void OnDisable()
    {
        GlobalEvents<OnGameOver>.Happened -= GameOver;
        GlobalEvents<OnHideTubes>.Happened -= OnHideTubes;
        MyPlayer.OnTubeMove -= OnTubeMove;
        MyPlayer.OnIncreaseTubeRadius -= OnIncreaseTubeRadius;
    }

    private void OnHideTubes(OnHideTubes obj)
    {
        _isGameOver = true;
    }

    private void OnIncreaseTubeRadius(float scale)
    {
        if (_isHaveCollision) return;

        ChangeRadius(scale);

        Tube obj = ShapeObject.gameObject.GetComponent<Tube>();
        obj.GenerateGeometry(scale*TubeManager.InitRadius, obj.radius1, obj.height, obj.sides, 1, 0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);
        obj.FitCollider();
//        obj.transform.SetParent(transform, false);
//        obj.transform.localPosition = new Vector3(0, tube.height, 0);
    }

    public void ChangeRadius(float scale)
    {
        Scale = scale;
    }

    private void OnTubeMove()
    {
        _isMove = true;
    }

    private void GameOver(OnGameOver obj)
    {
        _isGameOver = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isHaveCollision = true;
            Destroy(GetComponent<Collider>());
        }
        else if (other.CompareTag("CoinPlaneSensor"))
        {
            GameEvents.Send(OnCanSpawnBonus);
        }
    }

    private void Update()
    {
        if (!_isGameOver)
        {
            if (_isShowAnimation)
            {
                if (transform.localScale.x < ScaleWeWant)
                    transform.localScale = new Vector3(transform.localScale.x + _increaseScaleSpeed * Scale,
                        transform.localScale.y + _increaseScaleSpeed * Scale,
                        transform.localScale.z + _increaseScaleSpeed * Scale);
                else
                {
                    _isShowAnimation = false;
                    transform.localScale = new Vector3(ScaleWeWant, ScaleWeWant, ScaleWeWant);
                }
            }
        }

//        transform.Rotate(Vector3.up, TubeManager.RotateSpeed);

        if (_isGameOver && !_isHaveCollision && transform.position.y > 1f)
        {
            if (_isMove)
            {
                _isMove = false;
                Destroy(GetComponent<Collider>());
            }
            
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(transform.localScale.x - _increaseScaleSpeed * Scale*2f,
                    transform.localScale.y - _increaseScaleSpeed * Scale*2f,
                    transform.localScale.z - _increaseScaleSpeed * Scale*2f);
            else
            {
                GameEvents.Send(OnDestroy, Id);
                Destroy(gameObject);
            }
        }
        
        if (_isMove)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - TubeManager.CurrentSpeed * Time.deltaTime,
                transform.position.z);

            if (!_isGameOver && !_isSentMoveEvent && transform.position.y < 1f)
            {
                _isReadyToDelete = true;
                if (!_isHaveCollision)
                    GlobalEvents<OnGameOver>.Call(new OnGameOver());
                else
                    GameEvents.Send(OnCanMove);
                _isSentMoveEvent = true;
            }

            if ((_isGameOver || _isReadyToDelete) && transform.position.y < -24f)
            {
                GameEvents.Send(OnDestroy, Id);
                Destroy(gameObject);
            }   
        }
    }
}