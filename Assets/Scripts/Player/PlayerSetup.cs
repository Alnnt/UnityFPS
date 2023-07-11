using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentToDisable;

    private Camera sceneCamera;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }

        RegisterPlayer();
    }

    private void RegisterPlayer()
    {
        string name = "Player " + GetComponent<NetworkObject>().NetworkObjectId.ToString();
        Player player = GetComponent<Player>();
        GameManager.Singleton.RegisterPlayer(name, player);
    }

    private void DisableComponents()
    {
        for (int i = 0; i < componentToDisable.Length; ++i)
        {
            componentToDisable[i].enabled = false;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.Singleton.UnRegisterPlayer(transform.name);
    }
}
