using DG.Tweening;
using UnityEngine;

public class PlayerTubeGood : PlayerTubeBad
{
	/// <summary>
	///     Show Good Animation
	/// </summary>
	/// <param name="comboCounter"></param>
	public void GoodAnimation(int comboCounter)
    {
        transform.DOScale(1.1f + 0.15f * comboCounter, .20f).SetRelative();
    }

    private void Update()
    {
        Move();
        var color = GetComponent<MeshRenderer>().material.color;
        if (color.a > 0f)
        {
            color.a -= 0.09f;
            GetComponent<MeshRenderer>().material.color = color;
        }
        else
        {
            Destroy(gameObject);
        }
        if (transform.position.y < -10f) Destroy(gameObject);
    }
}