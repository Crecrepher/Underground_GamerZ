using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public int maxHp = 50;
    public float speed;
    public float sight;                 // �þ�
    public float sightAngle;        // �þ߰�
    public float detectionRange;    // Ž������

    public float range;         // ���� �����Ÿ�

    public float evasionRate;       // ȸ����
    public float reactionSpeed;     // �����ӵ�

    public int damage;

    public int armor;

    public bool IsLive { get; set; } = true;
    public int Hp { get; set; }

    private void Awake()
    {
        Hp = maxHp;
    }
}
