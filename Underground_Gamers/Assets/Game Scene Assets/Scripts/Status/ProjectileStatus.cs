using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStatus : CharacterStatus
{
    [Header("����ü")]
    public float lifeCycle;
    [Header("���� ����ü")]
    public bool isPenetrating;
    [Header("���� ����ü")]
    public bool isAreaAttack;
    public float explosionRange;

}
