using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerClass
{
    Attacker,
    Defender,
    Supporter,
    Sniper
}
public class Player
{
    public int code = -1;
    public int grade = -1;
    public int gearCode = -1;
    public float ID = -1f;
}

public struct PlayerInfo
{
    public int code;
    public string name;
    public string comment;    //���� ����
    public int hp;
    public int atk;
    public float atkRate;
    public int capacity;
    public float range;
    public float reloadingSpeed;

    public float moveSpeed;
    public float sight;
    public float reactionSpeed;
    public float criticalChance;
    public float Accuracy;         //���߷�
    public float avoidRate;        //ȸ����
    public float collectingRate;   //��ź��
    public float detectionRange;   //��������
}
