using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BtnPlayAnimation : MonoBehaviour {
	private bool _isAnimate;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine(Animation());
	}

	private void OnEnable()
	{
		GlobalEvents<OnShowMenu>.Happened += OnShowMenu;
	}

	private void OnDisable()
	{
		GlobalEvents<OnShowMenu>.Happened += OnShowMenu;
	}

	private void OnShowMenu(OnShowMenu obj)
	{
		_isAnimate = true;
	}

	private IEnumerator Animation()
	{
		while (true)
		{
			if (_isAnimate)
			{
//				Tweener t = transform.DOScale(1.2f, 0.5f).SetLoops(1, LoopType.Yoyo);
//				t.OnComplete (() =>
//				{
//					transform.DOScale(1.3f, 0.3f).SetLoops(1, LoopType.Yoyo);
//				});
			}
			yield return new WaitForSeconds(4.00f);
		}
	}
}
