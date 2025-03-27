using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    [Header("Information")]
    public GameObject uiPanel;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textTime;
    public TextMeshProUGUI textWeapon;
    [Header("Controller")]
    public GameObject virtualController;
    public ProgressBar grenadeCooldown;

    [Header("Health")]
    public ProgressBar healthBar;

    [Header("Result")]
    public CanvasGroup panelResult;
    public TextMeshProUGUI textResult;
    public TextMeshProUGUI textResultScore;
    public Button btnReplay;
    public Button btnHome;
    public TextMeshProUGUI textBtnReplay;

    [Header("Flash")]
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashAlpha = 0.4f;

    void Awake()
    {
        btnHome.onClick.AddListener(OnExit);
    }

    public void ShowResult(bool win, int score)
    {
        panelResult.gameObject.SetActive(true);
        panelResult.alpha = 0;
        Tween.Alpha(panelResult, 1, 0.5f);
        textResult.text = win ? STATIC_STRING.MISSION_COMPLETE : STATIC_STRING.GAME_OVER;
        textBtnReplay.text = STATIC_STRING.RETRY;
        textResultScore.text = STATIC_STRING.SCORE + score;
        btnReplay.onClick.RemoveAllListeners();
        btnReplay.onClick.AddListener(() =>
        {
            ServiceLocator.Get<GameStateManager>().ChangeState(GAME_STATE.START_GAME);
            SetUIState(true);
        });
    }

    private void OnExit()
    {
        ServiceLocator.Get<GameStateManager>().ChangeState(GAME_STATE.MENU);
    }

    public void UpdateScore(int score)
    {
        textScore.text = STATIC_STRING.SCORE + score.ToString();
    }

    public void UpdateTime(float time)
    {
        textTime.text = string.Format(STATIC_STRING.TIME_LEFT, time.ToString("F2"));
    }

    public void UpdateWeaponInfo(WeaponInformation weaponData)
    {
        textWeapon.text = STATIC_STRING.WEAPON + weaponData.weaponName;
    }

    public void TriggerFlash()
    {
        flashImage.gameObject.SetActive(true);
        var col = flashImage.color;
        col.a = 0;
        flashImage.color = col;

        Sequence.Create().Chain(Tween.Alpha(flashImage, 0.2f, 0.1f)).Chain(Tween.Alpha(flashImage, 0, 0.1f)).OnComplete(() =>
        {
            flashImage.gameObject.SetActive(false);
        });

    }

    public void UpdateHealthBar(float percent)
    {
        healthBar.SetProgress(percent);
    }

    public void SetUIState(bool startGame)
    {
        panelResult.gameObject.SetActive(!startGame);
        uiPanel.gameObject.SetActive(startGame);
        virtualController.gameObject.SetActive(startGame);
    }
}
