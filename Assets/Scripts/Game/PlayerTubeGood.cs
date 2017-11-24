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
        transform.DOScale(3.0f + 0.25f * comboCounter, .20f);
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
            return;
        }
        if (transform.position.y < -10f) Destroy(gameObject);
    }
}