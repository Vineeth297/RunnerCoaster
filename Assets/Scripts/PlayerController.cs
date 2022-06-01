using Dreamteck.Splines;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

	[SerializeField] private GameObject speedParticleSystem;
	private void Start()
	{
		_spline = GetComponent<SplineFollower>();
		_spline.follow = false;
	}
	private void Update()
	{
		_spline.follow = Input.GetMouseButton(0);
		//print(_spline.followSpeed);
		MoveTheKart();
	}

	private void MoveTheKart()
	{
		float dot = Vector3.Dot(this.transform.forward, Vector3.down);
	//	print(dot);
		float dotPercent = Mathf.Lerp(-slopeRange / 90f, slopeRange / 90f, (dot + 1f) / 2f);
		speed -= Time.deltaTime * frictionForce * (1f - brakeForce);
		float speedAdd = 0f;
		float speedPercent = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
		if (dotPercent > 0f)
		{
			speedAdd = gravityForce * dotPercent * speedGain.Evaluate(speedPercent) * Time.deltaTime;
		}
		else
		{
			speedAdd = gravityForce * dotPercent * speedLoss.Evaluate(1f-speedPercent) * Time.deltaTime;
		}
		speed += speedAdd * (1f-brakeForce);
		speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
		if (addForce > 0f) {
			float lastAdd = addForce;
			addForce = Mathf.MoveTowards(addForce, 0f, Time.deltaTime * 30f);
			speed += lastAdd - addForce;
		}
		_spline.followSpeed = speed;
		_spline.followSpeed *= (1f - brakeForce);
		if (brakeTime > Time.time) brakeForce = Mathf.MoveTowards(brakeForce, 1f, Time.deltaTime * brakeSpeed);
		else brakeForce = Mathf.MoveTowards(brakeForce, 0f, Time.deltaTime * brakeReleaseSpeed);

		speedPercent = Mathf.Clamp01(speed/maxSpeed)*(1f-brakeForce);
	}

	public void AssignSlopeSpeed()
	{
		minSpeed = 50f;
		maxSpeed = 70f;
		speedParticleSystem.SetActive(true);
		GameEvents.InvokeGetHyped();
		GameManager.Instance.SpeedPushEffect();

	}
	
	public void AssignCurveSpeed()
	{
		minSpeed = 50f;
		maxSpeed = 90f;
		speedParticleSystem.SetActive(true);
		GameEvents.InvokeGetHyped();		
		GameManager.Instance.SpeedPushEffect();

	}
	
	
	public void ResetMaxSpeed()
	{
		minSpeed = 30f;
		maxSpeed = 50f;
		speedParticleSystem.SetActive(false);
		GameEvents.InvokeNoHype();
		GameManager.Instance.SpeedPullEffect();
	}
}
