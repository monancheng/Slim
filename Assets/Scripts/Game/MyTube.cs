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
   [HideInInspector] public BaseObject ShapeObject;
   [HideInInspector] public bool IsIncreaseSize;
    public static event Action OnCanMove;
    public static event Action OnCanSpawnBonus;
    public static event Action <int> OnDestroy;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        GlobalEvents<OnGameOver>.Happened += GameOver;
        MyPlayer.OnTubeMove += OnTubeMove;
        MyPlayer.OnIncreaseTubeRadius += OnIncreaseTubeRadius;
    }

    private void OnDisable()
    {
        GlobalEvents<OnGameOver>.Happened -= GameOver;
        MyPlayer.OnTubeMove -= OnTubeMove;
        MyPlayer.OnIncreaseTubeRadius -= OnIncreaseTubeRadius;
    }
    
    private void OnIncreaseTubeRadius(float radius)
    {
        if (_isHaveCollision) return;
        
        Tube obj = ShapeObject.gameObject.GetComponent<Tube>();
        obj.GenerateGeometry(radius, obj.radius1 + (radius-obj.radius0), obj.height, obj.sides, 1, 0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);
        obj.FitCollider();
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
        if (transform.localScale.x < 1f)
            transform.localScale = new Vector3(transform.localScale.x + 0.05f, transform.localScale.y + 0.05f,
                transform.localScale.z + 0.05f);
        else
            transform.localScale = Vector3.one;

        transform.Rotate(Vector3.up, TubeManager.RotateSpeed);

        if (_isGameOver && !_isHaveCollision && transform.position.y > 1f)
        {
            if (_isMove)
            {
                _isMove = false;
                Destroy(GetComponent<Collider>());
            }
            
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(transform.localScale.x - 0.1f, transform.localScale.y - 0.1f,
                    transform.localScale.z - 0.1f);
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