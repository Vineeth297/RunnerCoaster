using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public int numberOfActiveKarts;
	
	private void Awake()
	{
		DOTween.KillAll();

		if (Instance)
			Destroy(gameObject);
		else
			Instance = this;
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}