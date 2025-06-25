using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public Canvas canvas;

    void Awake()
    {
        HideTooltip();
    }

    public void ShowTooltip(string message, Vector2 screenPosition)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = message;

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out anchoredPos
        );
        tooltipPanel.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
} 