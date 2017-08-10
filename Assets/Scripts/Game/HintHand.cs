using DG.Tweening;
using DoozyUI;
using UnityEngine;

public class HintHand : MonoBehaviour
{
	private Tweener _tweener;
	// Use this for initialization
	void Start ()
	{
		_tweener = transform.DOMoveX(Screen.width - transform.position.x, 1f);
		_tweener.SetLoops(-1, LoopType.Yoyo);
		_tweener.SetEase(Ease.InOutFlash);
	}

	private void OnEnable()
	{
		GlobalEvents<OnStartGame>.Happened += OnStartGame;
	}

	private void OnStartGame(OnStartGame obj)
	{
		UIManager.HideUiElement("ScreenHint");
		UIManager.HideUiElement("ScreenHintBar");
		UIManager.HideUiElement("ScreenHintHand");
		_tweener.Kill();
		_tweener = null;
		GlobalEvents<OnStartGame>.Happened -= OnStartGame;
	}
}
