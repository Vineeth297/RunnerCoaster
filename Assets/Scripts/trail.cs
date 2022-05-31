using UnityEngine;

public class trail : MonoBehaviour
{
	private Rigidbody _rb;
	public float explosiveForce = 5f; 
	public float explosionRadius = 5f; 

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
			
			foreach (var hitCollider in hitColliders)
			{
				Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
				
				hitCollider.SendMessage("AddDamage");
				rb.AddExplosionForce(explosiveForce, transform.position , explosionRadius, 1f,ForceMode.Impulse);

			}
		}
	}
	
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		//Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
		Gizmos.DrawWireSphere (transform.position, explosionRadius);
	}
}
