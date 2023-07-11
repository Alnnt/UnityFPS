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


        Player player = GetComponent<Player>();
        player.Setup();

        string name = "Player " + GetComponent<NetworkObject>().NetworkObjectId.ToString();
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
