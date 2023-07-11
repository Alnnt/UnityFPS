using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button hostBtn;
    [SerializeField]
    private Button serverBtn;
    [SerializeField]
    private Button clientBtn;

    // Start is called before the first frame update
    void Start()
    {
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            DestroyAllButtons();
        });
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            DestroyAllButtons();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            DestroyAllButtons();
        });
    }

    private void DestroyAllButtons()
    {
        Destroy(hostBtn.gameObject);
        Destroy(serverBtn.gameObject);
        Destroy(clientBtn.gameObject);
    }
}
