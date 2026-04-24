using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerInteractor : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactableLayer;

    private void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            NetworkInteractable interactable = hit.collider.GetComponentInParent<NetworkInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}