using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScatterTheBirds : MonoBehaviour
{
    // Start is called before the first frame update
	[SerializeField] private List<GameObject> birds;
	[SerializeField] private float maxFlySpeed = 1f;
	[SerializeField] private float forwardMultiplier = 330f;
	[SerializeField] private List<Animator> _birdAnimators;

	[SerializeField] private bool allToFlyAtSameTime;
	private static readonly int FlyHash = Animator.StringToHash("Fly");
	
	
	
	private void Start()
    {
		_birdAnimators = new List<Animator>();
		foreach (var bird in birds)
		{
			var animator = bird.GetComponent<Animator>();
			_birdAnimators.Add(animator);
			bird.transform.LookAt(transform);
		}
		
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;

		if(!allToFlyAtSameTime)
			StartCoroutine(ScatterTheBirdsAway());
		else
		{
			ScatterTheBirdsAways();
		}
	}

	private IEnumerator ScatterTheBirdsAway()
	{
		AudioManager.instance.Play("BirdsFlap");
		
		for (var bird = 0; bird < birds.Count; bird++)
		{
			var myBird = birds[bird].transform;

			var direction = transform.forward * forwardMultiplier + transform.right * ((Random.value > 0.5f ? 1 : -1) * 50)
											  + Vector3.up * 30;
			
			Debug.DrawRay(myBird.position, direction, Color.red, 3f, false);
			
			var endPos = myBird.position + direction;
					  
			
			myBird.DOMoveX(endPos.x, 1.5f).SetEase(Ease.Linear).OnComplete(()=>myBird.gameObject.SetActive(false));
			myBird.DOMoveY(endPos.y, 1.5f).SetEase(Ease.Linear);
			myBird.DOMoveZ(endPos.z, 1.5f).SetEase(Ease.Linear);

			_birdAnimators[bird].SetTrigger(FlyHash);
			yield return new WaitForSeconds(0.15f);

		}

	}
	
	private void ScatterTheBirdsAways()
	{
		AudioManager.instance.Play("BirdsFlap");
		
		for (var bird = 0; bird < birds.Count; bird++)
		{
			var myBird = birds[bird].transform;

			var direction = transform.forward * forwardMultiplier + transform.right * ((Random.value > 0.5f ? 1 : -1) * 50)
																  + Vector3.up * 30;
			
			Debug.DrawRay(myBird.position, direction, Color.red, 3f, false);
			
			var endPos = myBird.position + direction;
					  
			
			myBird.DOMoveX(endPos.x, 1.5f).SetEase(Ease.Linear).OnComplete(()=>myBird.gameObject.SetActive(false));
			myBird.DOMoveY(endPos.y, 1.5f).SetEase(Ease.Linear);
			myBird.DOMoveZ(endPos.z, 1.5f).SetEase(Ease.Linear);

			_birdAnimators[bird].SetTrigger(FlyHash);
		}

	}
}
