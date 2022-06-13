using DG.Tweening;
using UnityEngine;

public class BonusRamp : MonoBehaviour
{
	[SerializeField] private BonusTile[] leftTiles, rightTiles;
	[SerializeField] private Color[] colors;

	public float LowestPointY => transform.position.y;
	
	private void Start()
	{
		DOVirtual.DelayedCall(0.2f, GiveColors);
	}

	private void GiveColors()
	{
		if(leftTiles.Length == 0) return;
		
		var currentStartColor = colors[0];
		var currentEndColor = colors[1];

		var perCombo = leftTiles.Length / colors.Length;
		for (var i = 0; i < leftTiles.Length; i++)
		{
			if (i > 0 && (i % perCombo) == 0)
			{
				print($"{i / perCombo} for {i}");
				currentStartColor = colors[(i / perCombo) - 1];
				currentEndColor = colors[(i / perCombo)];
			}
			var color = Color.Lerp(currentStartColor, currentEndColor, (float) (i % perCombo) / perCombo);
			leftTiles[i].meshRenderer.material.color = rightTiles[i].meshRenderer.material.color = color;
		}
	}
	
	
}
