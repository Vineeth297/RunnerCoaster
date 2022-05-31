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
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
        
		//transform.parent = null;
		AddExplosiveForce();

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
