using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AmmunitionUITracker : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<int, int> swapWeapon;
    [SerializeField]
    private UnityEvent<int> updateCurrentAmmo;
    [SerializeField]
    private UnityEvent<string> addIcon;

    /*public void SwitchSelectedWeaponInUI(int weapoinIdx, WeaponContainer weapon)
    {
        swapWeapon.Invoke(weapoinIdx, weapon.Ammunition);
    }*/

    public void UpdateWeaponAmmoInUI(int ammo)
    {
        updateCurrentAmmo.Invoke(ammo);
    }

    public void AddWeaponIcon(string iconPath)
    {
        addIcon.Invoke(iconPath);
    }
}
