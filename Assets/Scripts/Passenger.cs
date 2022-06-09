using UnityEngine;

public class Passenger : MonoBehaviour
{
	private Animator _animator;

	private static readonly int HypeHash = Animator.StringToHash("Hype");

	private void OnEnable()
	{
		GameEvents.GetHyped += OnHype;
		GameEvents.NoHype += OnNoHype;
	}

	private void OnDisable()
	{
		GameEvents.GetHyped -= OnHype;
		GameEvents.NoHype += OnNoHype;
	}

	private void Start()
	{
		_animator = GetComponent<Animator>();
	}

	private void OnHype()
	{
		_animator.SetBool(HypeHash, true);
	}

	private void OnNoHype()
	{
		_animator.SetBool(HypeHash, false);
	}
}