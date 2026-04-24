using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 20;

    private void Awake()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        int connectedPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;

        bool canJoin = connectedPlayers < maxPlayers;

        response.Approved = canJoin;
        response.CreatePlayerObject = canJoin;
        response.Pending = false;

        if (!canJoin)
        {
            response.Reason = "Server is full.";
        }
    }
}