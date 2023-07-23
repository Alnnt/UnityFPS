using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    private WeaponManager weaponManager;
    private PlayerWeapon currentWeapon;

    private float shootCoolDownTime = 0f;   // 距离上次开枪经过的时间（秒）
    private int autoShootCount = 0; // 当前一共连开多少枪

    [SerializeField]
    private LayerMask mask;

    private Camera cam;
    private PlayerController playerController;

    enum HitEffectMaterial
    {
        Metal,
        Stone
    }

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        weaponManager = GetComponent<WeaponManager>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!IsLocalPlayer) return;

        shootCoolDownTime += Time.deltaTime;
        currentWeapon = weaponManager.GetPlayerWeapon();

        if (currentWeapon.shootRate <= 0)   // 单发
        {
            if (Input.GetButtonDown("Fire1") && shootCoolDownTime >= currentWeapon.shootCoolDownTime)
            {
                autoShootCount = 0;
                Shoot();
                shootCoolDownTime = 0f;
            }
        } else // 连发
        {
            if (Input.GetButtonDown("Fire1"))
            {
                autoShootCount = 0;
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
        Destroy(hitEffectObject, 2f);
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

    private void OnShoot(float recoilForce)   // 每次射击相关的逻辑，包括粒子特效、音效等
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        weaponManager.GetCurrentAudioSource().Play();

        if (IsLocalPlayer)  // 后坐力
        {
            playerController.AddRecoilForce(currentWeapon.recoilForce);
        }
    }

    [ClientRpc]
    private void OnShootClientRpc(float recoilForce)
    {
        OnShoot(recoilForce);
    }

    [ServerRpc]
    private void OnShootServerRpc(float recoilForce)
    {
        if (!IsHost)
        {
            OnShoot(recoilForce);
        }
        OnShootClientRpc(recoilForce);
    }

    private void Shoot()
    {
        ++autoShootCount;
        float recoilForce = currentWeapon.recoilForce;

        if (autoShootCount < 4)
        {
            recoilForce *= 0.2f;
        }
        OnShootServerRpc(recoilForce);

        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            
            if(hit.collider.CompareTag(PLAYER_TAG))
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
