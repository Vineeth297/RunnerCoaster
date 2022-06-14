using DG.Tweening;
using UnityEngine;

public class UVMover : MonoBehaviour
{
	[SerializeField] private float oneUvRotationDuration = 0.5f;
	private void Start() =>
		GetComponent<Renderer>().materials[1]
			.DOOffset(Vector2.up * -1, oneUvRotationDuration)
			.SetLoops(-1, LoopType.Restart)
			.SetEase(Ease.Linear);
}