using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaker : MonoBehaviour
{
    public GameManager gameManager;
    private void Awake()
    {
        // ���� �̰��� Fix
        if (GameInfo.instance != null)
        {
            gameManager.IsStart = false;
            gameManager.gameRuleManager.SetGameType(GameInfo.instance.gameType);

            // ���� �ĺ� ���� ���鶧 ����, ��Ʈ�� OK ��ư ������
            GameInfo.instance.StartGame();

            // ��Ʈ���� ���� ��, ����
            //GameInfo.instance.SetEntryPlayer();
            if(gameManager.gameRuleManager.gameType == GameType.Official)
            {
                gameManager.entryPanel.SetActiveEntryPanel(true);
                gameManager.entryPanel.SetPlayerEntrySlotAndBenchSlot();
            }
            else
            {
                gameManager.battleLayoutForge.SetActiveBattleLayoutForge(true);
            }


            // ��Ʈ�� ���� �� ���� ���� �������� ����� �� �͵�
            //GameInfo.instance.MakePlayers();
            //gameManager.settingAIID.SetAIIDs();
            // ���� ���� UI
            //gameManager.entryManager.ResetBattleLayoutForge();
        }
    }
}
