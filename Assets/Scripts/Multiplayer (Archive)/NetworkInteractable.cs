using Unity.Netcode;
using UnityEngine;

public class NetworkInteractable : NetworkBehaviour
{
    [SerializeField] private GameObject targetVisual;

    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(
        true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        isActive.OnValueChanged += OnActiveChanged;
        ApplyState(isActive.Value);
    }

    public override void OnNetworkDespawn()
    {
        isActive.OnValueChanged -= OnActiveChanged;
    }

    public void Interact()
    {
        if (IsServer)
        {
            isActive.Value = !isActive.Value;
        }
        else
        {
            RequestInteractRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestInteractRpc()
    {
        isActive.Value = !isActive.Value;
    }

    private void OnActiveChanged(bool oldValue, bool newValue)
    {
        ApplyState(newValue);
    }

    private void ApplyState(bool active)
    {
        if (targetVisual != null)
            targetVisual.SetActive(active);
    }
}