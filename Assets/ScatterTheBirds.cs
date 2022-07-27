using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScatterTheBirds : MonoBehaviour
{
    // Start is called before the first frame update
	[SerializeField] private List<GameObject> birds;
	[SerializeField] private float maxFlySpeed = 1f;
	
	[SerializeField] private List<Animator> _birdAnimators;

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

		StartCoroutine(ScatterTheBirdsAway());
	}

	private IEnumerator ScatterTheBirdsAway()
	{/*
		for (var bird = 0; bird < birds.Count; bird++)
		{
			var x = birds[bird].transform.forward + birds[bird].transform.up;
			birds[bird].transform.DOMove(birds[bird].transform.position + (x * 15f), 1).SetEase(Ease.InCirc);
			_birdAnimators[bird].SetTrigger(FlyHash);
			//_birdAnimators[bird].speed = Random.Range(1, 3);
			yield return new WaitForSeconds(0.1f);
		}*/
		AudioManager.instance.Play("BirdsFlap");
		
		for (var bird = 0; bird < birds.Count; bird++)
		{
			// Vector3.Normalize(transform.forward + transform.right * (.5f * (Random.value > 0.5f ? 1 : -1))) * 50
			// 	+ Vector3.up * 10 ;
			var myBird = birds[bird].transform;
			var pos = transform.localPosition +
					  transform.forward + transform.right * (.5f * (Random.value > 0.5f ? 1 : -1) * 50)
				+ Vector3.up * 10 ;
			
			myBird.DOLocalMoveX(pos.x, 1.5f).SetEase(Ease.Linear).OnComplete(()=>myBird.gameObject.SetActive(false));
			myBird.DOLocalMoveY(pos.y, 1.5f).SetEase(Ease.Linear);
			myBird.DOLocalMoveZ(pos.z + Random.Range(100,200), 1.5f).SetEase(Ease.Linear);

			_birdAnimators[bird].SetTrigger(FlyHash);
			yield return new WaitForSeconds(0.1f);
		}
	}
}
