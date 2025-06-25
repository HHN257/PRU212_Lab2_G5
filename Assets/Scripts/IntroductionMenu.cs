using UnityEngine;
using UnityEngine.UI;

public class IntroductionMenu : MonoBehaviour
{
    public Button gameplayButton;
    public Button itemButton;
    public GameObject gameplayPanel;
    public GameObject itemPanel;
    public GameObject introductionPanel;

    void Start()
    {
        gameplayButton.onClick.AddListener(ShowGameplay);
        itemButton.onClick.AddListener(ShowItem);

        ShowGameplay(); // Mặc định hiển thị gameplay
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && introductionPanel != null)
        {
            introductionPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        ShowGameplay(); // Luôn hiển thị gameplay khi panel được bật lại
    }

    void ShowGameplay()
    {
        gameplayPanel.SetActive(true);
        itemPanel.SetActive(false);
    }

    void ShowItem()
    {
        gameplayPanel.SetActive(false);
        itemPanel.SetActive(true);
    }
}
