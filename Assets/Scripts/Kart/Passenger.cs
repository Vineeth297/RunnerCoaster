using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class Passenger : MonoBehaviour
	{
		[Header("Jump"), SerializeField] private float distance;
		[SerializeField] private float upTime, holdTime, downTime;
		private Animator _anim;

		private Sequence _jumpSequence;
		private Transform _transform;
		private float _initLocalY;
		private bool _isJumpSequenceInitialised;

		private static readonly int Hype = Animator.StringToHash("Hype");

		private void OnEnable()
		{
			GameEvents.UpdateHype += OnUpdateHype;
			//GameEvents.PassengerJump += OnPassengerJump;
		}

		private void OnDisable()
		{
			GameEvents.UpdateHype -= OnUpdateHype;
			//GameEvents.PassengerJump -= OnPassengerJump;
		}

		private void Start()
		{
			_anim = GetComponent<Animator>();
			
			_transform = transform;
			_initLocalY = _transform.localPosition.y;
			TryInitialiseJumpSequence();
		}

		private void OnUpdateHype(bool newStatus) => _anim.SetBool(Hype, newStatus);

		public void MakePassengerJump(float delay)
		{
			if (delay < 0.01f)
			{
				_jumpSequence.Restart();
				return;
			}
			DOVirtual.DelayedCall(delay, () => _jumpSequence.Restart());
		}

		private void TryInitialiseJumpSequence()
		{
			if (_isJumpSequenceInitialised) return;

			_jumpSequence = DOTween.Sequence();

			_jumpSequence.AppendCallback(() =>
			{
				_anim.SetBool(Hype, true);
				_transform.localPosition = new Vector3(transform.localPosition.x, _initLocalY, _transform.localPosition.z);
			});
			
			_jumpSequence.Join(_transform.DOLocalMoveY(_initLocalY + distance, upTime));
			_jumpSequence.AppendInterval(holdTime);
			_jumpSequence.AppendCallback(() => _anim.SetBool(Hype, false));
			_jumpSequence.Join(transform.DOLocalMoveY(_initLocalY, downTime));

			_jumpSequence.Pause();
			_isJumpSequenceInitialised = true;
		}
	}
}