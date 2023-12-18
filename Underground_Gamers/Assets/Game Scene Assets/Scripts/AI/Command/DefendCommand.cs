using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendCommand : Command
{
    public override void ExecuteCommand(AIController ai, WayPoint wayPoint)
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        AIController selectAI = ai;

        // ��ü ���
        if (selectAI == null)
        {
            foreach (var aiController in gameManager.aiManager.pc)
            {
                aiController.isDefend = true;
                aiController.isRetreat = false;
                aiController.isAttack = false;
                aiController.isMission = false;
                if (aiController.status.IsLive)
                    aiController.SetState(States.Retreat);

                Debug.Log($"{aiController.aiType.text} : Defend Command Execute");
            }
        }         // �������
        else
        {
            ai.isDefend = true;
            ai.isRetreat = false;
            ai.isAttack = false;
            ai.isMission = false;
            if (ai.status.IsLive)
                ai.SetState(States.Retreat);
            if (ai.teamIdentity.teamType == TeamType.PC)
                gameManager.commandManager.SetActiveCommandButton(ai);
            Debug.Log($"{ai.aiType.text} : Defend Command Execute");
        }
    }
}
