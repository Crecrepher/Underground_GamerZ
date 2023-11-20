using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public int maxHp;
    public float speed;
    public float sight;                 // �þ�
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
