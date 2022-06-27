using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour
{
	[SerializeField] private GameObject holdToAim, victory, defeat, nextLevel, retry, constantRetryButton, skipLevel;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private Image red, emoji;
	[SerializeField] private float emojiRotation;

	[SerializeField] private Button nextLevelButton;

	private Color _originalRedColor, _lighterRedColor;
	private bool _hasTapped, _hasLost;
	private Sequence _emojiSequence;

	private void OnEnable()
	{
		GameEvents.KartCrash += OnObstacleCollision;

		GameEvents.PlayerDeath += OnGameLose;
		GameEvents.GameWin += OnGameWin;
	}

	private void OnDisable()
	{
		GameEvents.KartCrash -= OnObstacleCollision;
		
		GameEvents.PlayerDeath -= OnGameLose;
		GameEvents.GameWin -= OnGameWin;
	}

	private void Awake() => DOTween.KillAll();

	private void Start()
	{
		var levelNo = PlayerPrefs.GetInt("levelNo", 1);
		levelText.text = "Level " + levelNo;

		_originalRedColor = red.color;
		_lighterRedColor = _originalRedColor;
		_lighterRedColor.a *= 0.5f;
		
		_emojiSequence = DOTween.Sequence();

		var emojiTransform = emoji.transform;
		const float duration = 0.175f;
		
		_emojiSequence.Append(emoji.DOColor(Color.white, duration));
		_emojiSequence.AppendCallback(() => emojiTransform.localScale = Vector3.zero);
		_emojiSequence.Join(emojiTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * -emojiRotation / 2, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * emojiRotation, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * -emojiRotation, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * emojiRotation, duration));
		_emojiSequence.Append(emojiTransform.DOLocalRotate(Vector3.forward * -emojiRotation / 2, duration));
		_emojiSequence.Append(emoji.DOColor(Color.clear, duration));
		_emojiSequence.Join(emojiTransform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));

		_emojiSequence.SetRecyclable(true);
		_emojiSequence.SetAutoKill(false);
		_emojiSequence.Pause();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.N)) NextLevel();
		
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(_hasTapped) return;
		
		if (!InputExtensions.GetFingerDown()) return;
		
		if(!EventSystem.current) { print("no event system"); return; }
		
		if(!EventSystem.current.IsPointerOverGameObject(InputExtensions.IsUsingTouch ? Input.GetTouch(0).fingerId : -1))
			TapToPlay();
	}

	private void EnableVictoryObjects()
	{
		if(defeat.activeSelf) return;
		
		victory.SetActive(true);
		nextLevelButton.gameObject.SetActive(true);
		nextLevelButton.interactable = true;
		constantRetryButton.SetActive(false);
		
		AudioManager.instance.Play("Win");
	}

	private void EnableLossObjects()
	{
		if(victory.activeSelf) return;

		if (_hasLost) return;
		
		red.enabled = true;
		red.color = Color.clear;

		DOTween.Kill(red);
		red.DOColor(_originalRedColor, 1f);

		defeat.SetActive(true);
		retry.SetActive(true);
		//skipLevel.SetActive(true);
		constantRetryButton.SetActive(false);
		_hasLost = true;
		
		if(AudioManager.instance)
			AudioManager.instance.Play("Lose");
	}

	private void TapToPlay()
	{
		_hasTapped = true;
		holdToAim.SetActive(false);
		skipLevel.SetActive(false);
		
		GameEvents.InvokeTapToPlay();
	}

	public void Retry()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(AudioManager.instance)
			AudioManager.instance.Play("Button");
	}

	public void NextLevel()
	{
		if (PlayerPrefs.GetInt("levelNo", 1) < SceneManager.sceneCountInBuildSettings - 1)
		{
			var x = PlayerPrefs.GetInt("levelNo", 1) + 1;
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		else
		{
			var x = Random.Range(5, SceneManager.sceneCountInBuildSettings - 1);
			if (PlayerPrefs.GetInt("levelNo", 1) % 10 == 0)
				x = 12;
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo", 1) + 1);
		
		if(AudioManager.instance)
			AudioManager.instance.Play("Button");
		Vibration.Vibrate(15);
	}

	private void OnObstacleCollision(Vector3 obj)
	{
		red.enabled = true;
		red.color = Color.clear;
		red.DOColor(_lighterRedColor, 1f).SetLoops(2, LoopType.Yoyo);
		
		_emojiSequence.Restart();
	}

	private void OnGameLose()
	{
		DOVirtual.DelayedCall(1.5f, EnableLossObjects);
	}

	private void OnGameWin()
	{
		DOVirtual.DelayedCall(1f, EnableVictoryObjects);
	}
}
