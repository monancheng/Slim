using DG.Tweening;
using UnityEngine;

public class PlayerTubeGood : PlayerTube
{
	public void GoodAnimation(int comboCounter)
	{
		transform.DOScale(1.0f + .2f*comboCounter, .15f).SetRelative();
	}
	
	private void Update()
	{
		Move();
		Color color = gameObject.GetComponent<MeshRenderer>().material.color;
		if (color.a > 0f)
		{
			color.a -= 0.1f;
			gameObject.GetComponent<MeshRenderer>().material.color = color;
		}
		else
		{
			Destroy(gameObject);
		}
		if (transform.position.y < -8f) Destroy(gameObject);
	}
}
