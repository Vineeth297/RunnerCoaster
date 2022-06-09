using DG.Tweening;
using UnityEngine;

public class Kart : MonoBehaviour
{
	private Vector3 _initScale;

	private void Start()
	{
		_initScale = transform.localScale;
		transform.DOScale(_initScale * 1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
	}
}