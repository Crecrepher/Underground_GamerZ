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
    public float ID = -1f;

    public int code = -1;
    public int type = -1;
    public int level = 0;
    public int maxLevel = 30;
    public int breakthrough = 0;

    public int gearCode = -1;
    public int gearLevel = 0;
    public float xp = 0;
    public float maxXp = 100;
    public int condition = 0;
    //ability??
}

public struct PlayerInfo
{
    public int code;
    public int type;
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
