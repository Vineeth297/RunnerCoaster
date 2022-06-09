using System.Collections;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
	public GameObject cube;
	public float moveSpeed = 0.2f, intervalZ = 0.2f;

	private Vector3 lastpos = Vector3.negativeInfinity;
	private float _prevPoZ;
	public int maxCubes = 100;

	private void Start()
	{
		StartCoroutine(DoSnake());
	}

	private IEnumerator DoSnake()
	{
		var currentTime = 0f;
		do
		{
			var candidate = new Vector3(
				Mathf.Sin(currentTime),
				_prevPoZ, Mathf.Cos(currentTime));

			if (Vector3.Distance(lastpos, candidate) < 0.3f)
			{
				_prevPoZ += intervalZ;
				currentTime += Time.deltaTime * moveSpeed;
				yield return new WaitForSeconds(Time.deltaTime * moveSpeed);

				continue;
			}

			var obj = Instantiate(cube);
			lastpos = obj.transform.position = candidate;
			//obj.transform.parent = transform;

			if (maxCubes-- < 0) yield break;

			_prevPoZ += intervalZ;
			currentTime += Time.deltaTime * moveSpeed;
			yield return new WaitForSeconds(Time.deltaTime * moveSpeed);
		} while (true);
	}
}