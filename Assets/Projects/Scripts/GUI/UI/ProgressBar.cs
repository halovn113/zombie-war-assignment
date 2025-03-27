using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float progress = 0f;

    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value);
        fillImage.fillAmount = progress;
    }
}
