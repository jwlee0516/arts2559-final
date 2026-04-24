using UnityEngine;

public class ArtworkInteractable : MonoBehaviour
{
    [Header("Artwork Info")]
    [SerializeField] private string artworkTitle;
    [TextArea(3, 8)]
    [SerializeField] private string artworkDescription;
    [TextArea(3, 10)]
    [SerializeField] private string generationPrompt;

    [Header("Visual References")]
    [SerializeField] private GameObject highlightBorder;

    public string ArtworkTitle => artworkTitle;
    public string ArtworkDescription => artworkDescription;
    public string GenerationPrompt => generationPrompt;

    private void Awake()
    {
        SetHighlighted(false);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (highlightBorder != null)
        {
            highlightBorder.SetActive(highlighted);
        }
    }

    public void Interact()
    {
        ArtworkPopupUI popup = FindFirstObjectByType<ArtworkPopupUI>();

        if (popup != null)
        {
            popup.ShowArtwork(this);
        }
    }
}