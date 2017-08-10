using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour
{
    private const float VelocityMax = 30.0f;
    private float _addAngleCoeff;
    private bool _isAnglePlus;
    private bool _isHideAnimation;
    private bool _isMoveToTarget;
    private bool _isShowAnimation;
    private float _moveAngle;
    private Vector3 _targetPos;
    private float _velocity;

    [HideInInspector] public GameObject ParentObj;
    public static event Action<int> OnAddCoinsVisual;

    public void Show()
    {
        _isShowAnimation = true;
    }

    public void Hide(bool isAnimation)
    {
        _isHideAnimation = isAnimation;

        if (!_isHideAnimation) Destroy(transform.parent.gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isShowAnimation)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.2f, transform.localScale.y - 0.2f, 1f);
            if (transform.localScale.x <= 1f)
            {
                _isShowAnimation = false;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else if (_isHideAnimation)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.1f, transform.localScale.y - 0.1f, 1f);
            if (transform.localScale.x <= 0f) Destroy(transform.parent.gameObject);
        }
        else if (_isMoveToTarget)
        {
            if (transform.localScale.x >= 1)
                transform.localScale = new Vector3(transform.localScale.x - 0.1f,
                    transform.localScale.y - 0.1f, 1f);

            var ang = Mathf.Atan2(_targetPos.y - transform.position.y, _targetPos.x - transform.position.x);

            if (_isAnglePlus)
            {
                _moveAngle += _addAngleCoeff * Mathf.Deg2Rad;
                if (_moveAngle >= 180f * Mathf.Deg2Rad) _moveAngle -= 360f * Mathf.Deg2Rad;
            }
            else
            {
                _moveAngle -= _addAngleCoeff * Mathf.Deg2Rad;
                if (_moveAngle <= -180f * Mathf.Deg2Rad) _moveAngle += 360f * Mathf.Deg2Rad;
            }

            if (_addAngleCoeff < 35f) _addAngleCoeff += 0.5f;

            if (Mathf.Abs(_moveAngle - ang) < _addAngleCoeff * 1.5f * Mathf.Deg2Rad) _moveAngle = ang;

            if (_velocity < VelocityMax) _velocity += 0.5f;
            transform.position = new Vector3(transform.position.x + _velocity * Mathf.Cos(_moveAngle),
                transform.position.y + _velocity * Mathf.Sin(_moveAngle), 1f);

            if (Vector2.Distance(transform.position, _targetPos) <= VelocityMax)
            {
                _isMoveToTarget = false;

                GameEvents.Send(OnAddCoinsVisual, 1);
                Destroy(transform.parent.gameObject);
            }
        }
    }

    public void MoveToEnd()
    {
        transform.position = new Vector3(Screen.width*0.5f + Random.Range(-25, 25), Screen.height*0.5f + Random.Range(-25, 25), 0f);
	    _targetPos = new Vector3(ParentObj.transform.position.x, ParentObj.transform.position.y, transform.position.z);
        _velocity = 5.0f + Random.value * 5.0f;
        if (Random.value < 0.5f) _moveAngle = Random.value * 180f * Mathf.Deg2Rad;
        else _moveAngle = -Random.value * 180f * Mathf.Deg2Rad;

        _isAnglePlus = !(Random.value < 0.5f);

        _addAngleCoeff = 4f;

        _isMoveToTarget = true;

        transform.localScale = new Vector3(2.5f, 2.5f, 1f);
    }
}