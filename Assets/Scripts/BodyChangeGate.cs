using System;
using TMPro;
using UnityEngine;

public class BodyChangeGate : MonoBehaviour
{
	public enum ChangeType
	{
		Add,
		Subtract,
		Multiply,
		Divide
	}

	//public GameObject addSprite;
	//public GameObject subtractSprite;
	//public GameObject multiplySprite;
	//public GameObject divideSprite;

	public MeshRenderer meshRenderer;
	
	public ChangeType changeType = ChangeType.Add;
	public TMP_Text text;
	public int factor;

	private bool _hasBeenUsed;
	private bool _isAtExtreme;
	public bool toMoveHorizontally;

	public Color positiveColor;
	public Color negativeColor;

	public float speed = 1f;

	[SerializeField] private RollerCoasterManager rollerCoasterManager;
	private void Start()
	{
		if (changeType == ChangeType.Add || changeType == ChangeType.Multiply)
		{
				//meshRenderer.material.color = positiveColor;
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = positiveColor;
		}
		else
		{
			//meshRenderer.material.color = negativeColor;
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = negativeColor;
		}

	}
	
	
	private void Update()
	{
		if (!toMoveHorizontally) return;
		
		if (!_isAtExtreme)
		{
			transform.position = Vector3.MoveTowards(transform.position,
				transform.position + Vector3.left * 3f,
				Time.deltaTime * speed);
			if (transform.position.x <= -1.5f)
				_isAtExtreme = true;
		}
		else 
		{
			transform.position = Vector3.MoveTowards(transform.position,
				transform.position + Vector3.right * 3f,
				Time.deltaTime * speed);
			if (transform.position.x >= 1.5f)
				_isAtExtreme = false;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (_hasBeenUsed) return;

		if (other.CompareTag("Player"))
		{
			switch (changeType)
			{
				case ChangeType.Add:
				{
					//DO Something
					rollerCoasterManager.SpawnTheKarts(factor);
					break;
				}
				case ChangeType.Subtract:
				{
					//DO Something
					rollerCoasterManager.DisableTheKarts(factor);
					break;
				}
				case ChangeType.Multiply:
				{
					//DO Something
					rollerCoasterManager.SpawnTheKarts(factor);
					break;
				}
				case ChangeType.Divide:
				{
					//DO Something
					rollerCoasterManager.DisableTheKarts(factor);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		_hasBeenUsed = true;
	}

	private void OnValidate()
	{
		if (changeType == ChangeType.Add)
		{
			//addSprite.SetActive(true);
			//subtractSprite.SetActive(false);
			//multiplySprite.SetActive(false);
			//divideSprite.SetActive(false);
			meshRenderer.sharedMaterial.color = positiveColor;
			name = text.text = " + " + factor;
		//	meshRenderer.material.color = Color.HSVToRGB(6, 197, 255);

		}
		else if (changeType == ChangeType.Subtract)
		{
			//addSprite.SetActive(false);
			//subtractSprite.SetActive(true);
			//multiplySprite.SetActive(false);
			//divideSprite.SetActive(false);
			meshRenderer.sharedMaterial.color = negativeColor;
			name = text.text = " - " + factor;
			//meshRenderer.material.color = Color.red;
		}
		else if (changeType == ChangeType.Multiply)
		{
			//addSprite.SetActive(false);
			//subtractSprite.SetActive(false);
			//multiplySprite.SetActive(true);
			//divideSprite.SetActive(false);
			meshRenderer.sharedMaterial.color = positiveColor;
			name = text.text = " + " + factor;
		//	meshRenderer.material.color = Color.HSVToRGB(6, 197, 255);
		}
		else if (changeType == ChangeType.Divide)
		{
			//addSprite.SetActive(false);
			//subtractSprite.SetActive(false);
			//multiplySprite.SetActive(false);
			//divideSprite.SetActive(true);
			meshRenderer.sharedMaterial.color = negativeColor;
			name = text.text = " - " + factor;
			//meshRenderer.material.color = Color.red;
		}
		else
		{
			name = name;
		}
	}
}
