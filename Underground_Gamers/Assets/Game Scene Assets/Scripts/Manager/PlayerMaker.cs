using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaker : MonoBehaviour
{
    public GameManager gameManager;
    private void Awake()
    {
        if (GameInfo.instance != null)
        {
            // ���� �ĺ� ���� ���鶧 ����
            GameInfo.instance.StartGame();
            gameManager.IsStart = false;

            // ���� ������ ������ �� �Լ� ȣ��
            {
                GameInfo.instance.MakePlayers();
                gameManager.settingAIID.SetAIIDs();
                gameManager.entryManager.ResetEntry();
                gameManager.gameRuleManager.SetGameType(GameInfo.instance.gameType);
            }
        }
    }
}
