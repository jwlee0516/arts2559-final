using Unity.Netcode;
using UnityEngine;

public class NetworkStartUI : MonoBehaviour
{
    private void OnGUI()
    {
        if (NetworkManager.Singleton == null)
            return;

        GUILayout.BeginArea(new Rect(20, 20, 220, 180));

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host"))
            {
                NetworkManager.Singleton.StartHost();
            }

            if (GUILayout.Button("Client"))
            {
                NetworkManager.Singleton.StartClient();
            }

            if (GUILayout.Button("Server"))
            {
                NetworkManager.Singleton.StartServer();
            }
        }
        else
        {
            GUILayout.Label("Connected");
            GUILayout.Label($"Mode: {(NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client")}");
        }

        GUILayout.EndArea();
    }
}