using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArtworkPopupUI : MonoBehaviour
{
    [Header("Popup Root")]
    [SerializeField] private GameObject popupOverlay;
    [SerializeField] private CanvasGroup popupCanvasGroup;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text promptText;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 10f;

    public bool IsOpen { get; private set; }

    private float targetAlpha;

    private void Awake()
    {
        ForceHide();
    }

    private void Update()
    {
        HandleCloseInput();
        AnimatePopup();
    }

    private void HandleCloseInput()
    {
        if (!IsOpen)
            return;

        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            Hide();
        }
    }

    private void AnimatePopup()
    {
        if (popupCanvasGroup == null)
            return;

        popupCanvasGroup.alpha = Mathf.Lerp(
            popupCanvasGroup.alpha,
            targetAlpha,
            Time.unscaledDeltaTime * fadeSpeed
        );

        if (!IsOpen && popupCanvasGroup.alpha < 0.01f)
        {
            popupCanvasGroup.alpha = 0f;

            if (popupOverlay != null)
                popupOverlay.SetActive(false);
        }
    }

    public void ShowArtwork(ArtworkInteractable artwork)
    {
        if (artwork == null)
            return;

        if (popupOverlay != null)
            popupOverlay.SetActive(true);

        if (titleText != null)
            titleText.text = artwork.ArtworkTitle;

        if (descriptionText != null)
            descriptionText.text = artwork.ArtworkDescription;

        if (promptText != null)
            promptText.text = artwork.GenerationPrompt;

        IsOpen = true;
        targetAlpha = 1f;

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.interactable = true;
            popupCanvasGroup.blocksRaycasts = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void Hide()
    {
        IsOpen = false;
        targetAlpha = 0f;

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.interactable = false;
            popupCanvasGroup.blocksRaycasts = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    private void ForceHide()
    {
        IsOpen = false;
        targetAlpha = 0f;

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0f;
            popupCanvasGroup.interactable = false;
            popupCanvasGroup.blocksRaycasts = false;
        }

        if (popupOverlay != null)
            popupOverlay.SetActive(false);

        Time.timeScale = 1f;
    }
}