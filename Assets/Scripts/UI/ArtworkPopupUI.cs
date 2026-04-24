using TMPro;
using UnityEngine;

public class ArtworkPopupUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text promptText;

    private void Awake()
    {
        Hide();
    }

    public void ShowArtwork(ArtworkInteractable artwork)
    {
        if (artwork == null)
        {
            return;
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = artwork.ArtworkTitle;
        }

        if (descriptionText != null)
        {
            descriptionText.text = artwork.ArtworkDescription;
        }

        if (promptText != null)
        {
            promptText.text = "Prompt:\n" + artwork.GenerationPrompt;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}