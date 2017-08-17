using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class BestScore : MonoBehaviour
{
    private bool _isHideAnimation;

    private bool _isShowAnimation = true;
    private int _pointsCount;
    private float _startScale;
    [SerializeField] private Image img;

    [SerializeField] private Text textField;

    // Use this for initialization
    private void Start()
    {
        var color = textField.color;
        color.a = 0f;
        textField.color = color;
        img.color = new Color(img.color.r, img.color.g, img.color.b, color.a);
        _startScale = img.transform.localScale.x;
        _pointsCount = DefsGame.GameBestScore;
        textField.text = _pointsCount.ToString();
    }

    public void ShowAnimation()
    {
        _isHideAnimation = false;
        _isShowAnimation = true;
    }

    public void HideAnimation()
    {
        _isShowAnimation = false;
        _isHideAnimation = true;
    }

    // Update is called once per frame
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
            img.color = new Color(img.color.r, img.color.g, img.color.b, color.a);
        }

        if (_isHideAnimation)
        {
            var color = textField.color;
            if (textField.color.a > 0f)
            {
                color.a -= 0.1f;
            }
            else
            {
                _isHideAnimation = false;
                color.a = 0f;
            }
            textField.color = color;
            img.color = new Color(img.color.r, img.color.g, img.color.b, color.a);
        }

        if (img.transform.localScale.x > _startScale)
            img.transform.localScale = new Vector3(img.transform.localScale.x - 2.0f * Time.deltaTime,
                img.transform.localScale.y - 2.0f * Time.deltaTime, 1f);
    }

    private void MakeAnimation()
    {
        _pointsCount = DefsGame.GameBestScore;
        textField.text = _pointsCount.ToString();
        img.transform.localScale = new Vector3(_startScale * 1.4f, _startScale * 1.4f, 1f);
        MasterAudio.PlaySoundAndForget("NewHighScore");
    }

    public void UpdateVisual()
    {
        // Здесь только визуальная обработка. Изменение BestScore в Points
        if (DefsGame.GameBestScore > _pointsCount) MakeAnimation();
    }
}