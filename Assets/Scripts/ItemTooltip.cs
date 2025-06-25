using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public TextMeshProUGUI tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipText.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipText.text = "";
    }
} 