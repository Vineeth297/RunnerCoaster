using DG.Tweening;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	private Rigidbody _rb;
	public float explosiveForce = 5f; 
	public float explosionRadius = 5f; 

	public float forceToApply;
	public float angleOfForce;

	private float _xComponent;
	private float _yComponent;

	private Collider _collider;

	public float directionMultiplier = 5f;
	public float dropHeight = 15f;

	public bool isEngine;
	public bool isForceAdded;
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_rb.isKinematic = false;
		transform.parent = null;
		
		//transform.parent = null;

		//AddExplosiveForce();

		/*transform.DORotate(new Vector3(Random.Range(-60f, 60f), Random.Range(-60f, 60f), Random.Range(-60f, 60f)), 0.25f)
			.SetLoops(5, LoopType.Incremental);
		transform.DOJump(transform.position + new Vector3(Random.Range(-10f,10f),-20f,Random.Range(-10f,10f)),
			30,1,2f);
			*/
		
		// transform.DOJump(new Vector3(Mathf.Cos(Random.Range(0f, 1f)), 
		// 					 0f,
		// 					 Mathf.Sin(Random.Range(0f, 1f))) * directionMultiplier
		// 				 + Vector3.down * dropHeight,
		// 			5,1,2f);
	}

	private void FixedUpdate()
	{
		if (isForceAdded) return;
		AddBackForce();
	}

	private void AddExplosiveForce()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
		
		foreach (var hitCollider in hitColliders)
		{
			var rb = hitCollider.GetComponent<Rigidbody>();
			
			//hitCollider.SendMessage("AddDamage");
			rb.isKinematic = false;
			rb.AddExplosionForce(explosiveForce, transform.position , explosionRadius, 1f,ForceMode.Impulse);

		}
	}
	
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		//Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
		Gizmos.DrawWireSphere (transform.position, explosionRadius);
	}


	private void AddBackForce()
	{
		//if (!isEngine) return;
		print(gameObject.name);
		_rb.AddForce(-transform.forward * explosiveForce, ForceMode.Impulse);
		isForceAdded = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		
	}

	
	private void AddForceAtAnAngle(float force, float angle)
	{
		angle *= Mathf.Deg2Rad;
		_xComponent = Mathf.Cos(angle) * force;
		_yComponent = Mathf.Sin(angle) * force;
        
		var forceApplied = new Vector3(_xComponent, _yComponent, 30);
        
		_rb.AddForce(forceApplied);
	}
}
