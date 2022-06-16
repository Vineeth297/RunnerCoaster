using DG.Tweening;
using UnityEngine;

public class TrainObstacle : MonoBehaviour
{
	[SerializeField] private BridgeLight bridgeLight;
	[SerializeField] private Transform train;
	[SerializeField] private float travelLoopDistance, travelLoopDuration;

	private void Start()
	{
		train.position -= train.forward * travelLoopDistance / 2;
		train.DOMove(train.position + train.forward * travelLoopDistance, travelLoopDuration);
	}
}