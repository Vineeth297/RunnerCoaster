using UnityEngine;

public class Passenger : MonoBehaviour
{
	private Animator _animator;

	private static readonly int HypeHash = Animator.StringToHash("Hype");

	private void OnEnable()
	{
		GameEvents.UpdateHype += OnUpdateHype;
	}

	private void OnDisable()
	{
		GameEvents.UpdateHype -= OnUpdateHype;
	}

	private void Start()
	{
		_animator = GetComponent<Animator>();
	}

	private void OnUpdateHype(bool newStatus) => _animator.SetBool(HypeHash, newStatus);
}