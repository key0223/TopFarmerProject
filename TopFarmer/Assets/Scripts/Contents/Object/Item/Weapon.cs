using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Weapon : Item
{
    public int MinDamage { get; private set; }
    public int MaxDamage { get; private set; }
    public float Knokback { get; private set; }
    public float Speed { get; private set; }
    public int Defense { get; private set; }
    public WeaponType WeaponType { get; private set; }
    public bool CanBeLostOnDeath { get; private set; }
    public string Projectiles { get; private set; }

    public Weapon(int weaponId)
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(weaponId, out itemData);
        if (itemData == null) return;

        WeaponData data = (WeaponData)itemData;

        string[] damageArray = data.damage.Split(",");
        MinDamage = int.Parse(damageArray[0]);
        MaxDamage = int.Parse(damageArray[1]);
        Knokback = data.knokback;
        Speed = data.speed;
        Defense = data.defense;
        WeaponType = (WeaponType)data.weaponType;
        CanBeLostOnDeath = data.canBeLostOnDeath;
        Projectiles = data.projectiles;
    }
}
