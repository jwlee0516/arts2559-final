using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] private GameObject targetVisual;

    private bool isActive = true;

    public void Interact()
    {
        isActive = !isActive;

        if (targetVisual != null)
        {
            targetVisual.SetActive(isActive);
        }
    }
}