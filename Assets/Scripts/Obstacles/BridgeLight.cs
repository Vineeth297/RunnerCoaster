using UnityEngine;

public class BridgeLight : MonoBehaviour
{
	[SerializeField] private Light light;
	[SerializeField] private new Renderer renderer;

	public void ChangeColor(Color color)
	{
		light.color = renderer.material.color = color;
	}
}