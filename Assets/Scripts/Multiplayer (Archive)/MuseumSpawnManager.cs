using Unity.Netcode;
using UnityEngine;

public class MuseumSpawnManager : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += MovePlayerToSpawnPoint;
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= MovePlayerToSpawnPoint;
    }

    private void MovePlayerToSpawnPoint(ulong clientId)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (playerObject == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        CharacterController controller = playerObject.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        playerObject.transform.SetPositionAndRotation(
            spawnPoint.position,
            spawnPoint.rotation
        );

        if (controller != null)
            controller.enabled = true;
    }
}