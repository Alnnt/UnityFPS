using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    private WeaponManager weaponManager;
    private PlayerWeapon currentWeapon;

    [SerializeField]
    private LayerMask mask;

    private Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetPlayerWeapon();

        if (currentWeapon.shootRate <= 0)   // 单发
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        } else // 连发
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.shootRate);
            } else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }

    }

    private void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if(hit.collider.tag == PLAYER_TAG)
            {
                ShootServerRPC(hit.collider.name, currentWeapon.damage);
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
