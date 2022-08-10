using Unity.Netcode;

public class ParentPlayerToInSceneNetworkObject : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Server subscribes to the NetworkSceneManager.OnSceneEvent event
            NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;

            // Server player is parented under this NetworkObject
            SetPlayerParent(NetworkManager.LocalClientId);
        }
    }

    private void SetPlayerParent(ulong clientId)
    {
        if (IsSpawned && IsServer)
        {
            // As long as the client (player) is in the connected clients list
            if (NetworkManager.ConnectedClients.ContainsKey(clientId))
            {
                // Set the player as a child of this in-scene placed NetworkObject 
                NetworkManager.ConnectedClients[clientId].PlayerObject.transform.parent = transform;
            }
        }
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        // OnSceneEvent is very useful for many things
        switch (sceneEvent.SceneEventType)
        {
            // The C2S_SyncComplete event tells the server that a client-player has:
            // 1.) Connected and Spawned
            // 2.) Loaded all scenes that were loaded on the server at the time of connecting
            // 3.) Synchronized (instantiated and spawned) all NetworkObjects in the network session
            case SceneEventType.SynchronizeComplete:
                {
                    // As long as we are not the server-player
                    if (sceneEvent.ClientId != NetworkManager.LocalClientId)
                    {
                        // Set the newly joined and synchronized client-player as a child of this in-scene placed NetworkObject
                        SetPlayerParent(sceneEvent.ClientId);
                    }
                    break;
                }
        }
    }
}