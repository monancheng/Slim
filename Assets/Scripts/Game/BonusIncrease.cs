using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusIncrease : MonoBehaviour
{
    public static event Action OnBonusGrow;
    private Collider _collider;
    private float _currentAngle;
    private bool _isHideAnimation;
    private bool _isActivate;

    private bool _isShowAnimation;
    private bool _isTimeToCreate;
    private MeshRenderer _renderer;
    private AnimationScript _script;

    [SerializeField] private AudioClip[] SoundsGrow;

    [HideInInspector] public bool IsVisible;
    public static event Action<int> OnAddCoinsVisual;

    private void Awake()
    {
        _script = GetComponentInChildren<AnimationScript>();
        _collider = GetComponent<Collider>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        Hide(false);
    }

    private void OnEnable()
    {
        MyTube.OnCanSpawnBonus += OnCanSpawnBonus;
        TubeManager.OnCreateBonusIncrease += OnCreate;
        GlobalEvents<OnGameOver>.Happened += GameOver;
    }

    private void GameOver(OnGameOver obj)
    {
        Hide();
    }

    private void OnCanSpawnBonus()
    {
        if (!IsVisible && _isTimeToCreate)
            Show();
    }

    private void OnCreate()
    {
        _isTimeToCreate = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isActivate)
        {
            transform.localScale = new Vector3(transform.localScale.x + 0.55f, transform.localScale.y + 0.55f,
                transform.localScale.z + 0.55f);
            if (_renderer.material.color.a > 0f)
            {
                _renderer.material.SetColor("_Color", new Color(
                    _renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b,
                    _renderer.material.color.a - 0.15f));
            }
            else
            {
                _isActivate = false;
                transform.localScale = Vector3.zero;
                IsVisible = false;
            }
        }else
        if (_isHideAnimation)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.1f, transform.localScale.y - 0.1f,
                transform.localScale.z - 0.1f);
            if (_renderer.material.color.a > 0f)
            {
                _renderer.material.SetColor("_Color", new Color(
                    _renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b,
                    _renderer.material.color.a - 0.1f));
            }
            else
            {
                _isHideAnimation = false;
                transform.localScale = Vector3.zero;
                IsVisible = false;
            }
        }
        else if (IsVisible)
        {
            transform.position = new Vector3(transform.position.x,
                transform.position.y - TubeManager.CurrentSpeed * Time.deltaTime, transform.position.z);
            if (transform.position.y < -24f)
                Hide(false);
        }
        if (_isShowAnimation)
            if (transform.localScale.x < 2.1f)
            {
                transform.localScale = new Vector3(transform.localScale.x + 0.2f, transform.localScale.y + 0.2f,
                    transform.localScale.z + 0.2f);
            }
            else
            {
                _isShowAnimation = false;
                transform.localScale = new Vector3(2f, 2f, 2f);
            }
    }

    public void Show()
    {
        _isTimeToCreate = false;
        _collider.enabled = true;
        IsVisible = true;
        _script.isAnimated = true;

        _renderer.material.SetColor("_Color", new Color(
            _renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, 1f));
        float posX = Random.Range(14f, 17f);
        if (Random.value > 0.5f) posX *= -1;
        transform.position = new Vector3(posX, 600f, 0f);
        transform.localScale = new Vector3(0f, 0f, 0f);
        _isShowAnimation = true;
    }

    private void Activate()
    {
        _isActivate = true;
        _isShowAnimation = false;
        _script.isAnimated = false;

        _collider.enabled = false;
    }

    private void Hide(bool isHideAnimation = true)
    {
        _isHideAnimation = isHideAnimation;
        if (isHideAnimation)
        {
        }
        else
        {
            transform.localScale = Vector3.zero;
            IsVisible = false;
        }
        _isShowAnimation = false;
        _script.isAnimated = false;

        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Defs.PlaySound(GetRandomGrowSound());
            GameEvents.Send(OnBonusGrow);
            Activate();
        }
    }

    private AudioClip GetRandomGrowSound()
    {
        return SoundsGrow[(int) Mathf.Round(Random.value * (SoundsGrow.Length - 1))];
    }
}