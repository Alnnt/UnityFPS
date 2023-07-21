using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerWeapon
{
    public string name = "M16A1";
    public int damage = 10;
    public float range = 100f;     // 射击距离

    public float shootRate = 10f;   // 每秒射出子弹数 如果小于等于零，则为单发

    public GameObject graphics;
}
