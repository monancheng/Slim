using UnityEngine;

public class BonusItem : MonoBehaviour {
	[SerializeField] protected ParticleSystem _ps;
	protected bool IsVisible;

	protected Collider _collider;
	protected bool _isHideAnimation;
	protected bool _isActivate;

	protected bool _isShowAnimation;
	protected bool _isTimeToCreate;
	protected AnimationScript _script;
	protected float _masterAlpha;

	protected void Init() {
		_script = GetComponentInChildren<AnimationScript>();
		_collider = GetComponent<Collider>();
		Hide(false);
	}

	protected void OnCreateChar()
	{
		_isTimeToCreate = true;
	}

	virtual protected void OnCanSpawn()
	{
		if (!IsVisible && _isTimeToCreate)
			Show();
	}

	protected void GameOver(OnGameOver obj)
	{
		Hide();
	}

	protected void Activate()
	{
		_isActivate = true;
		_isShowAnimation = false;
		_script.isAnimated = false;

		_collider.enabled = false;

		if (_ps != null)
		{
			_ps.gameObject.SetActive(true);
			_ps.transform.position = transform.position;
			_ps.Play();
		}
	}

	protected void Hide(bool isHideAnimation = true)
	{
		_isHideAnimation = isHideAnimation;
		if (!isHideAnimation)
		{
			transform.localScale = Vector3.zero;
			IsVisible = false;
		}
		_isShowAnimation = false;
		_script.isAnimated = false;
		_collider.enabled = false;
		_isActivate = false;
	}

	protected void Show()
	{
		_isTimeToCreate = false;
		_collider.enabled = true;
		IsVisible = true;
		_script.isAnimated = true;

		_masterAlpha = 1f;
		SetColorAlpha();
		float posX = 22f;
		if (Random.value > 0.5f) posX *= -1;
		transform.position = new Vector3(posX, 700f, 0f);
		transform.localScale = new Vector3(0f, 0f, 0f);
		_isShowAnimation = true;
	}

	protected void MoveUpdate() {
		if (_isActivate)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.55f, transform.localScale.y + 0.55f,
				transform.localScale.z + 0.55f);
			
			if (_masterAlpha > 0f)
			{
				_masterAlpha -= 0.15f;
				SetColorAlpha();
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
				HideAnimation ();
			}
			else if (IsVisible)
			{
				transform.position = new Vector3(transform.position.x,
					transform.position.y - TubeManager.CurrentSpeed * Time.deltaTime, transform.position.z);
				if (transform.position.y < -24f)
					Hide(false);
			}
		if (_isShowAnimation) {
			ShowAnimation ();
		}
	}

	virtual protected void SetColorAlpha()
	{
//		Color oldColor = _renderer.material.GetColor("_Color");
//		_renderer.material.SetColor("_Color", oldColor + new Color(0f, 0f, _masterAlpha));
	}

	virtual protected void HideAnimation() {
		
		if (transform.localScale.x > 0f) {
			transform.localScale = new Vector3 (transform.localScale.x - 0.2f, transform.localScale.y - 0.2f,
				transform.localScale.z - 0.2f);
		} else {
			_isHideAnimation = false;
			transform.localScale = Vector3.zero;
			IsVisible = false;
		}
	}

	virtual protected void ShowAnimation() {
		if (transform.localScale.x < 2.1f) {
			transform.localScale = new Vector3 (transform.localScale.x + 0.2f, transform.localScale.y + 0.2f,
				transform.localScale.z + 0.2f);
		} else {
			_isShowAnimation = false;
			transform.localScale = new Vector3 (2f, 2f, 2f);
		}
	}
}
