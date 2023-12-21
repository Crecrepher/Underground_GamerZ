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
            // ���� ������ ������ �� �Լ� ȣ��
            {
                GameInfo.instance.MakePlayers();
                gameManager.settingAIID.SetAIIDs();
                gameManager.entryManager.InitEntry();
                gameManager.IsStart = false;
                gameManager.gameRuleManager.SetGameType(GameInfo.instance.gameType);
                gameManager.gameRuleManager.SetGameType(GameType.Official);
            }
        }
    }
}
