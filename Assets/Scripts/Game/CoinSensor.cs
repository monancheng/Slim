using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSensor : MonoBehaviour
{
	public static event Action<int> OnAddCoinsVisual;
//	public static event Action OnCoinEffect;
	
	[HideInInspector] public bool IsVisible;

	private bool _isShowAnimation;
    private Collider _collider;
	private AudioClip _sndTakeCoin;
	private AnimationScript _script;
	private float _currentAngle;
	private bool _isTimeToCreateCoin;
	private MeshRenderer _renderer;
	private bool _isHideAnimation;

	void Awake ()
	{
		_script = GetComponentInChildren<AnimationScript>();
	    _collider = GetComponent<Collider>();
		_renderer = GetComponentInChildren<MeshRenderer>();
		_sndTakeCoin = Resources.Load<AudioClip>("snd/GUI/bonus_spin");
		Hide(false);
	}

	private void OnEnable()
	{
		MyTube.OnCanSpawnCoin += OnCanSpawnCoin;
		TubeManager.OnCreateCoin += OnCreateCoin;
	}

	private void OnCanSpawnCoin()
	{
		if (!IsVisible && _isTimeToCreateCoin)
		{
			Show();
		}
	}

	private void OnCreateCoin()
	{
		_isTimeToCreateCoin = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_isHideAnimation)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.55f, transform.localScale.y + 0.55f, transform.localScale.z + 0.55f);
			if (_renderer.material.color.a > 0f)
			{
				_renderer.material.SetColor("_Color",new Color(
					_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b,
					_renderer.material.color.a - 0.15f));
			}
			else
			{
				_isHideAnimation = false;
				transform.localScale = Vector3.zero;
				IsVisible = false;
			}
		} else 
		if (IsVisible)
		{
			transform.position = new Vector3(transform.position.x,transform.position.y - TubeManager.CurrentSpeed * Time.deltaTime, transform.position.z);
			if (transform.position.y < -24f)
			{
				Hide(false);
			} 
		}
		if (_isShowAnimation)
		{
			if (transform.localScale.x < 2.1f)
			{
				transform.localScale = new Vector3(transform.localScale.x + 0.2f, transform.localScale.y + 0.2f, transform.localScale.z + 0.2f);
			}
			else
			{
				_isShowAnimation = false;
				transform.localScale = new Vector3(2f, 2f, 2f);
			}
		}
	}

    public void Show()
    {
	    _isTimeToCreateCoin = false;
        _collider.enabled = true;
        IsVisible = true;
	    _script.isAnimated = true;
	   
	    _renderer.material.SetColor("_Color",new Color(
		    _renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, 1f));
	    transform.position = new Vector3(Random.Range(-13f, 13f), 200f, 0f);
        transform.localScale = new Vector3 (0f, 0f, 0f);
	    _isShowAnimation = true;
    }

    public void Hide(bool isHideAnimation = true)
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
			GameEvents.Send(OnAddCoinsVisual, 1);
//			GameEvents.Send(OnCoinEffect);
			
			Defs.PlaySound(_sndTakeCoin);

			Hide();
		}
	}
}
