using TMPro;
using UnityEngine;

public class KartCounter : MonoBehaviour
{
	[SerializeField] private TMP_Text text;
	
	public void UpdateText(int number) => text.text = number.ToString();
}