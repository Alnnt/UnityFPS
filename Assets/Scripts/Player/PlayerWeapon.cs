using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerWeapon
{
    public string name = "M16A1";
    public int damage = 10;
    public float range = 100f;     // �������

    public float shootRate = 10f;   // ÿ������ӵ��� ���С�ڵ����㣬��Ϊ����

    public GameObject graphics;
}
