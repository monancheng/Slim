using UnityEngine;

public class ColorTheme : MonoBehaviour {

	public static Color GetPlayerColor()
	{
		float colorCount = 9f;
		float ran = Random.value;
		if (ran < 1f/colorCount) return new Color(1.0f, 105f/255f, 182f/255f);
		if (ran < 2f/colorCount) return new Color(98f / 255.0f, 201f / 255f, 255f / 255f);
		if (ran < 3f/colorCount) return new Color(203f / 255.0f, 216f / 255f, 235f / 255f);
		if (ran < 4f/colorCount) return new Color(250f / 255.0f, 69f / 255f, 78f / 255f);
//		if (ran < 5f/colorCount) return new Color(253f / 255.0f, 141f / 255f, 38f / 255f);
		if (ran < 5f/colorCount) return new Color(126f / 255.0f, 199f / 255f, 207f / 255f);
		if (ran < 6f/colorCount) return new Color(47f / 255.0f, 201f / 255f, 253f / 255f);
		if (ran < 7f/colorCount) return new Color(160f / 255.0f, 253f / 255f, 78f / 255f);
		if (ran < 8f/colorCount) return new Color(112f / 255.0f, 149f / 255f, 252f / 255f);
		return new Color(107f / 252.0f, 253f / 255f, 185f / 255f);
	}
	
	
	public static Color GetTubeColor()
	{
		float colorCount = 9f;
		float ran = Random.value;
		if (ran < 1f/colorCount) return new Color(1.0f, 105f/255f, 182f/255f);
		if (ran < 2f/colorCount) return new Color(98f / 255.0f, 201f / 255f, 255f / 255f);
		if (ran < 3f/colorCount) return new Color(203f / 255.0f, 216f / 255f, 235f / 255f);
		if (ran < 4f/colorCount) return new Color(250f / 255.0f, 69f / 255f, 78f / 255f);
//		if (ran < 5f/colorCount) return new Color(253f / 255.0f, 141f / 255f, 38f / 255f);
		if (ran < 5f/colorCount) return new Color(126f / 255.0f, 199f / 255f, 207f / 255f);
		if (ran < 6f/colorCount) return new Color(47f / 255.0f, 201f / 255f, 253f / 255f);
		if (ran < 7f/colorCount) return new Color(160f / 255.0f, 253f / 255f, 78f / 255f);
		if (ran < 8f/colorCount) return new Color(112f / 255.0f, 149f / 255f, 252f / 255f);
		return new Color(107f / 252.0f, 253f / 255f, 185f / 255f);
	}
}
