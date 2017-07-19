using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    private const float Delay = 0.0f;
    private bool _isPointAdded;
    private bool _isShowAnimation;
    private float _startScale;
    private float _time;
    [SerializeField] private Text textField;

    private void Start()
    {
        textField.text = "0";
        var color = textField.color;
        color.a = 0f;
        textField.color = color;
        _startScale = textField.transform.localScale.x;
    }

    private void OnEnable()
    {
        GlobalEvents<OnPointsShow>.Happened += OnPointsShow;
        GlobalEvents<OnPointsAdd>.Happened += OnPointsAdd;
        GlobalEvents<OnPointsReset>.Happened += OnPointsReset;
    }

    private void OnPointsReset(OnPointsReset obj)
    {
        ResetCounter();
    }

    private void OnPointsShow(OnPointsShow obj)
    {
        ShowAnimation();
    }

    private void OnPointsAdd(OnPointsAdd e)
    {
        AddPoint(e.PointsCount);
//		_poinsBmScript.AddPoints (pointsCount);
    }

    private void ShowAnimation()
    {
        _isShowAnimation = true;
    }

    private void ResetCounter()
    {
        DefsGame.CurrentPointsCount = 0;
        textField.text = "0";
    }

    private void Update()
    {
        if (_isShowAnimation)
        {
            var color = textField.color;
            if (textField.color.a < 1f)
            {
                color.a += 0.1f;
            }
            else
            {
                _isShowAnimation = false;
                color.a = 1f;
            }
            textField.color = color;
        }

        if (_isPointAdded)
        {
            _time += Time.deltaTime;
            if (_time > Delay)
            {
                _time = 0f;
                _isPointAdded = false;
                AddPointVisual();
            }
        }

        if (textField.transform.localScale.x > _startScale)
            textField.transform.localScale = new Vector3(textField.transform.localScale.x - 2.5f * Time.deltaTime,
                textField.transform.localScale.y - 2.5f * Time.deltaTime, 1f);
    }

    private void AddPoint(int count)
    {
        DefsGame.CurrentPointsCount += count;
        if (DefsGame.GameBestScore < DefsGame.CurrentPointsCount) DefsGame.GameBestScore = DefsGame.CurrentPointsCount;
        _isPointAdded = true;
    }

    private void AddPointVisual()
    {
        textField.text = DefsGame.CurrentPointsCount.ToString();
        textField.transform.localScale = new Vector3(_startScale * 1.4f, _startScale * 1.4f, 1f);
    }

//    private void UpdateVisual()
//    {
//        textField.text = DefsGame.CurrentPointsCount.ToString();
//    }
}