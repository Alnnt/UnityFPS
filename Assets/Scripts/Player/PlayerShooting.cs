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

    enum HitEffectMaterial
    {
        Metal,
        Stone
    }

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        if (!IsLocalPlayer) return;

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
            } else if (Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    private void OnHit(Vector3 pos, Vector3 normal, HitEffectMaterial material) // 击中点的特效。normal - 法向方向
    {
        GameObject hitEffectPrefab;
        if (material == HitEffectMaterial.Metal)
        {
            hitEffectPrefab = weaponManager.GetCurrentGraphics().metalHitEffectPrefab;
        }
        else
        {
            hitEffectPrefab = weaponManager.GetCurrentGraphics().stoneHitEffectPrefab;
        }

        GameObject hitEffectObject = Instantiate(hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        ParticleSystem particleSystem = hitEffectObject.GetComponent<ParticleSystem>();
        particleSystem.Emit(1);
        // particleSystem.Play();
        Destroy(hitEffectObject, 1f);
    }

    [ServerRpc]
    private void OnHitServerRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        if (!IsLocalPlayer)
        {
            OnHit(pos, normal, material);
        }
        OnHitClientRpc(pos, normal, material);
    }

    [ClientRpc]
    private void OnHitClientRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        OnHit(pos, normal, material);
    }

    private void OnShoot()   // 每次射击相关的逻辑，包括粒子特效、音效等
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    [ClientRpc]
    private void OnShootClientRpc()
    {
        OnShoot();
    }

    [ServerRpc]
    private void OnShootServerRpc()
    {
        if (!IsHost)
        {
            OnShoot();
        }
        OnShootClientRpc();
    }

    private void Shoot()
    {
        OnShootServerRpc();

        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if(hit.collider.tag == PLAYER_TAG)
            {
                ShootServerRPC(hit.collider.name, currentWeapon.damage);
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Metal);
            }
            else
            {
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Stone);
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
