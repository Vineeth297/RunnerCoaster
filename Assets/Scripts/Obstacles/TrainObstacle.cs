using DG.Tweening;
using UnityEngine;

public class TrainObstacle : MonoBehaviour
{
	[SerializeField] private Transform train;
	[SerializeField] private float travelLoopDistance, travelLoopDuration;

	private void Start()
	{
		train.DOLocalMove(train.localPosition + train.forward * travelLoopDistance, travelLoopDuration);
	}
}