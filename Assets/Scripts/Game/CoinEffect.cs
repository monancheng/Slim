using PrimitivesPro.GameObjects;
using UnityEngine;

public class CoinEffect : MonoBehaviour {
	private void OnEnable()
	{
//		CoinSensor.OnCoinEffect += OnCoinEffect;
	}

	private void OnCoinEffect()
	{
		BaseObject shapeObject = Tube.Create(5f, 5f + 1.0f, 0.4f, 23, 1, 0.0f, false,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Center);
		
		GameObject go = shapeObject.gameObject;
		go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
		go.GetComponent<Renderer>().material.SetColor("_Color", new Color(30f/255f, 180f/255f, 243f/255f));
		go.transform.position = transform.position;
		PlayerTubeGood pt = go.AddComponent<PlayerTubeGood>();
		pt.Speed = TubeManager.CurrentSpeed;
		pt.GoodAnimation(1);
	}
	
	Shader GetTransparentDiffuseShader()
	{
		return Shader.Find("Transparent/Diffuse");
	}
}
