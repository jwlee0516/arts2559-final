using UnityEngine;
using UnityEngine.InputSystem;

public class ArtworkInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 8f;
    [SerializeField] private LayerMask artworkLayer;

    private ArtworkInteractable currentHovered;

    private void Update()
    {
        UpdateHover();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryClick();
        }
    }

    private void UpdateHover()
    {
        ArtworkInteractable hovered = GetLookedAtArtwork();

        if (hovered == currentHovered)
        {
            return;
        }

        if (currentHovered != null)
        {
            currentHovered.SetHighlighted(false);
        }

        currentHovered = hovered;

        if (currentHovered != null)
        {
            currentHovered.SetHighlighted(true);
        }
    }

    private void TryClick()
    {
        if (currentHovered != null)
        {
            currentHovered.Interact();
        }
    }

    private ArtworkInteractable GetLookedAtArtwork()
    {
        if (playerCamera == null)
        {
            return null;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, artworkLayer))
        {
            return hit.collider.GetComponentInParent<ArtworkInteractable>();
        }

        return null;
    }
}