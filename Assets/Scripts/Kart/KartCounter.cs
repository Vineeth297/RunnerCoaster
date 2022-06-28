using TMPro;
using UnityEngine;

public class KartCounter : MonoBehaviour
{
	[SerializeField] private TMP_Text text;

	private void OnEnable()
	{
		GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
	}

	private void OnDisable()
	{
		GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
	}

	public void UpdateText(int number) => text.text = number.ToString();

	public void UpdateText(string s) => text.text = s;

	private void OnReachEndOfTrack()
	{
		transform.parent.gameObject.SetActive(false);
	}
}