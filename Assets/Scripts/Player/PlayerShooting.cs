using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private PlayerWeapon weapon;
    [SerializeField]
    private LayerMask mask;

    private Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            if(hit.collider.tag == PLAYER_TAG)
            {
                ShootServerRPC(hit.collider.name, weapon.damage);
            }
        }
    }

    [ServerRpc]
    private void ShootServerRPC(string name, int damage)
    {
        Player player = GameManager.Singleton.GetPlayer(name);
        player.TakeDamage(damage);
    }
}
