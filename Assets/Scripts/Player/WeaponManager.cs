using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon PrimaryWeapon;

    [SerializeField]
    private GameObject weaponHolder;

    private PlayerWeapon currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon(PrimaryWeapon);
    }

    public void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;

        GameObject weaponObject = Instantiate(currentWeapon.graphics, weaponHolder.transform.position, weaponHolder.transform.rotation);
        weaponObject.transform.SetParent(weaponHolder.transform);
    }

    public PlayerWeapon GetPlayerWeapon()
    {
        return currentWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
