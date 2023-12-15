using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackedTakeDamage : MonoBehaviour, IAttackable
{
    public void OnAttack(GameObject attacker, Attack attack)
    {
        CharacterStatus defenderStatus = transform.GetComponent<CharacterStatus>();
        CharacterStatus attackerStatus = attacker.GetComponent<CharacterStatus>();
        AIController controller = transform.GetComponent<AIController>();

        if (!defenderStatus.IsLive)
            return;

        // ���� ���� ����
        if (controller != null)
        {
            if (controller.isInvalid)
            {
                foreach (var buff in controller.appliedBuffs)
                {
                    if (buff is InvalidAttackBuff)
                    {
                        InvalidAttackBuff invalidAttackBuff = buff as InvalidAttackBuff;
                        invalidAttackBuff.invalidAttackCount--;
                        if (invalidAttackBuff.invalidAttackCount <= 0)
                        {
                            invalidAttackBuff.RemoveBuff(controller);
                        }
                        return;
                    }
                }
            }
        }

        defenderStatus.Hp -= attack.Damage;
        defenderStatus.Hp = Mathf.Min(defenderStatus.Hp, defenderStatus.maxHp);
        defenderStatus.Hp = Mathf.Max(0, defenderStatus.Hp);
        defenderStatus.GetHp();

        // ������ �׷��� ��ġ ����
        defenderStatus.takenDamage += attack.Damage;
        attackerStatus.dealtDamage += attack.Damage;

        // �ݰ�, ����
        if (controller != null/* && controller.battleTarget == null*/)
        {
            //controller.battleTarget = attacker.transform;
            //TeamIdentifier targetIdentity = controller.battleTarget.GetComponent<TeamIdentifier>();

            if (!controller.isBattle && !controller.isReloading/* || controller.battleTarget == null || targetIdentity.isBuilding*/)
            {
                controller.SetBattleTarget(attacker.transform);
                controller.SetState(States.Trace);
            }
        }

        TeamIdentifier identity = transform.GetComponent<TeamIdentifier>();

        // �ǹ� Ÿ�� ó��
        if (identity != null && identity.isBuilding)
        {
            identity.SetBuildingTarget(attacker.transform);
        }

        // ��� ó��
        if (defenderStatus.Hp <= 0)
        {
            defenderStatus.IsLive = false;
            if (!identity.isBuilding)
            {
                if (identity.teamLayer == LayerMask.GetMask("PC"))
                {
                    var text = GameObject.FindGameObjectWithTag("NPC_Score").GetComponent<TMP_Text>();
                    text.text = (int.Parse(text.text) + 1).ToString();
                }
                else
                {
                    var text = GameObject.FindGameObjectWithTag("PC_Score").GetComponent<TMP_Text>();
                    text.text = (int.Parse(text.text) + 1).ToString();
                }
            }

            var destroyables = transform.GetComponents<IDestroyable>();
            var respawnables = transform.GetComponents<IRespawnable>();
            if (destroyables != null)
            {
                foreach (var destroyable in destroyables)
                {
                    destroyable.DestoryObject(attacker);
                }
            }
            if (respawnables != null)
            {
                foreach (var respawnable in respawnables)
                {
                    respawnable.ExecuteRespawn(transform.gameObject);
                }
            }
        }
    }
}
