using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class PlayerControllerOld : MonoBehaviour
{
	private SplineFollower _spline;

	public float speed = 10f;
	public float minSpeed = 1f;
	public float maxSpeed = 20f;
	public float frictionForce = 0.1f;
	public float gravityForce = 1f;
	public float slopeRange = 60f;

	public AnimationCurve speedGain;
	public AnimationCurve speedLoss;
	public float brakeSpeed = 0f;
	public float brakeReleaseSpeed = 0f;

	private float brakeTime = 0f;
	private float brakeForce = 0f;
	private float addForce = 0f;

	public bool toMove;
	public bool toFly;

	private Rigidbody _rb;
	public float forceAmount = 5f;
	public float minimumMoveSpeed = 5f;
	public float maximumMoveSpeed = 5f;
	public float minimumDownSpeed = 5f;
	public float maximumDownSpeed = 5f;
	public float minimumYThreshold = 0f;

	[SerializeField] private GameObject speedParticleSystem;
	[SerializeField] private Transform finalKartPosition;

	private void OnEnable()
	{
		GameEvents.ObstacleCollision += OnExplosion;
		GameEvents.ReachEndOfTrack += ToFly;
		GameEvents.StopOnBonusRamp += OnStopTheRollerCoaster;
	}

	private void OnDisable()
	{
		GameEvents.ObstacleCollision -= OnExplosion;
		GameEvents.ReachEndOfTrack -= ToFly;
		GameEvents.StopOnBonusRamp -= OnStopTheRollerCoaster;
	}

	private void Start()
	{
		_spline = GetComponent<SplineFollower>();
		_spline.follow = false;
		_rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!toFly)
		{
			if (Input.GetMouseButtonDown(0))
			{
				_spline.follow = true;
				toMove = true;
			}
			else if (Input.GetMouseButton(0))
			{
				toMove = true;
			}
			else
			{
				toMove = false;
			}

			MoveTheKart();
		}
		else
		{
			//FlyTheKart();
			//add force on this kart
			if (Input.GetMouseButton(0))
			{
				var currentPos = transform.position;

				if (currentPos.y <= minimumYThreshold) currentPos.y = minimumYThreshold;
				transform.position = currentPos + transform.forward * (Time.deltaTime * maximumMoveSpeed) +
									 Vector3.down * (Time.deltaTime * minimumDownSpeed);
			}
			else
			{
				var currentPos = transform.position;

				if (currentPos.y <= minimumYThreshold) currentPos.y = minimumYThreshold;
				transform.position = currentPos + transform.forward * (Time.deltaTime * minimumMoveSpeed) +
									 Vector3.down * (Time.deltaTime * maximumDownSpeed);
			}
		}
	}

	public void MoveTheKart()
	{
		var dot = Vector3.Dot(transform.forward, Vector3.down);
		var dotPercent = Mathf.Lerp(-slopeRange / 90f, slopeRange / 90f, (dot + 1f) / 2f);
		speed -= Time.deltaTime * frictionForce * (1f - brakeForce);
		var speedAdd = 0f;
		var speedPercent = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
		if (dotPercent > 0f)
			speedAdd = gravityForce * dotPercent * speedGain.Evaluate(speedPercent) * Time.deltaTime;
		else
			speedAdd = gravityForce * dotPercent * speedLoss.Evaluate(1f - speedPercent) * Time.deltaTime;
		speed += speedAdd * (1f - brakeForce);
		speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
		if (addForce > 0f)
		{
			var lastAdd = addForce;
			addForce = Mathf.MoveTowards(addForce, 0f, Time.deltaTime * 30f);
			speed += lastAdd - addForce;
		}

		if (toMove)
		{
			_spline.followSpeed = speed;
			_spline.followSpeed *= 1f - brakeForce;
		}
		else
		{
			_spline.followSpeed -= Time.deltaTime * speed;
		}

		if (brakeTime > Time.time) brakeForce = Mathf.MoveTowards(brakeForce, 1f, Time.deltaTime * brakeSpeed);
		else brakeForce = Mathf.MoveTowards(brakeForce, 0f, Time.deltaTime * brakeReleaseSpeed);

		speedPercent = Mathf.Clamp01(speed / maxSpeed) * (1f - brakeForce);
	}

	private void FlyTheKart()
	{
		transform.position += transform.forward * (Time.deltaTime * minimumMoveSpeed) + Vector3.down * Time.deltaTime;
	}

	public void AssignSlopeSpeed()
	{
		minSpeed = 50f;
		maxSpeed = 90f;
		speedParticleSystem.SetActive(true);
		GameEvents.InvokeUpdateHype(true);
		CameraFxController.only.DoWideFov();
	}

	public void AssignCurveSpeed()
	{
		minSpeed = 50f;
		maxSpeed = 90f;
		speedParticleSystem.SetActive(true);
		GameEvents.InvokeUpdateHype(true);
		GameEvents.InvokeEnterHelix(true);
		CameraFxController.only.DoWideFov();
	}

	public void ResetMaxSpeed()
	{
		minSpeed = 30f;
		maxSpeed = 50f;
		speedParticleSystem.SetActive(false);
		GameEvents.InvokeUpdateHype(false);
		GameEvents.InvokeResetCameraPosition();
		CameraFxController.only.DoNormalFov();
	}

	private void OnExplosion()
	{
		_spline.follow = false;
		enabled = false;
	}

	private void ToFly()
	{
		_spline.enabled = false;
		/*transform.DOJump(finalKartPosition.position, 10f, 1, 5f).
			OnComplete(()=>
			{
				toFly = true;
				//GetComponent<Collider>().isTrigger = false;
			});*/
		toFly = true;
		transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f);
		//toFly = true;
	}

	private void OnStopTheRollerCoaster()
	{
		minimumMoveSpeed = maximumMoveSpeed = 0f;
		minimumDownSpeed = maximumDownSpeed = 0f;
	}

	public void BonusJumpHype()
	{
		GameEvents.InvokeUpdateHype(true);
	}
}