using UnityEngine;
using UnityEngine.InputSystem;

public class SinglePlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactableLayer;

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            PlayerInteractable interactable =
                hit.collider.GetComponentInParent<PlayerInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}