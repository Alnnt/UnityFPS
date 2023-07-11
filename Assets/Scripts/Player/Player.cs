using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    public int maxHealth = 100;
    [SerializeField]
    public Behaviour[] componentsToDisable;
    private bool[] componentsEnabled;
    private bool colliderEnbaled;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();

    public void Setup()
    {
        componentsEnabled = new bool[componentsToDisable.Length];
        for (int i = 0; i < componentsToDisable.Length; ++i)
        {
            componentsEnabled[i] = componentsToDisable[i].enabled;
        }
        Collider collider = GetComponent<Collider>();
        colliderEnbaled = collider.enabled;

        SetDefaults();
    }

    private void SetDefaults()
    {
        for(int i = 0; i < componentsToDisable.Length; ++i)
        {
            componentsToDisable[i].enabled = componentsEnabled[i];
        }
        Collider collider = GetComponent<Collider>();
        collider.enabled = colliderEnbaled;

        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            isDead.Value = false;
        }
    }

    public void TakeDamage(int damage)  // 受到伤害，仅在服务器端被调用
    {
        if (isDead.Value) return;

        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            isDead.Value = true;

            DieOnServer();
            DieClientRpc();
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.Singleton.MatchingSettings.respawnTime);
        SetDefaults();


        if (IsLocalPlayer)
        {
            transform.position = new Vector3(0f, 10f, 0f);
        }
    }

    private void DieOnServer()
    {
        Die();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Die();
    }

    private void Die()
    {
        for (int i = 0; i < componentsToDisable.Length; ++i)
        {
            componentsToDisable[i].enabled = false;
        }
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        StartCoroutine(Respawn());
    }

    public int GetHealth()
    {
        return currentHealth.Value;
    }
}
