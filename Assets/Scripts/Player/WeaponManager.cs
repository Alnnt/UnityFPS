using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon PrimaryWeapon;

    [SerializeField]
    private PlayerWeapon SecondaryWeapon;

    [SerializeField]
    private GameObject weaponHolder;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon(PrimaryWeapon);
    }

    public void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;

        if (weaponHolder.transform.childCount > 0)
        {
            Destroy(weaponHolder.transform.GetChild(0).gameObject);
        }

        GameObject weaponObject = Instantiate(currentWeapon.graphics, weaponHolder.transform.position, weaponHolder.transform.rotation);
        weaponObject.transform.SetParent(weaponHolder.transform);

        currentGraphics = weaponObject.GetComponent<WeaponGraphics>();
    }

    public PlayerWeapon GetPlayerWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    [ClientRpc]
    private void ToggleWeaponClientRpc()
    {
        ToggleWeapon();
    }

    [ServerRpc]
    private void ToggleWeaponServerRpc()
    {
        if (!IsHost)    // ±‹√‚host∂À÷ÿ∏¥÷¥––
        {
            ToggleWeapon();
        }
        ToggleWeaponClientRpc();
    }

    private void ToggleWeapon()
    {
        if (currentWeapon == PrimaryWeapon)
        {
            EquipWeapon(SecondaryWeapon);
        } else
        {
            EquipWeapon(PrimaryWeapon);
        }
    }

    void Update()
    {
        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleWeaponServerRpc();
            }
        }
    }
}
