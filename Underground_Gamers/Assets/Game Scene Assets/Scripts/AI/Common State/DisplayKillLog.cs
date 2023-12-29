using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayKillLog : MonoBehaviour, IDestroyable
{
    public KillLogPanel killLogPanel;
    public KillLog killLog;
    public GameObject killerPortrait;
    public GameObject deadPortrait;

    private void Start()
    {
        killLogPanel = GameObject.FindGameObjectWithTag("KillLogPanel").GetComponent<KillLogPanel>();
    }

    public void DestoryObject(GameObject attacker)
    {
        var attackerInfo = attacker.GetComponent<Portrait>();
        var attackerIdentity = attacker.GetComponent<TeamIdentifier>();
        var deadInfo = transform.GetComponent<Portrait>();
        var attackerPortrait = attackerInfo.GetPortrait();
        var deadPortrait = deadInfo.GetPortrait();

        KillLog killLog = Instantiate(this.killLog, killLogPanel.transform);
        killLog.destroyedTimer = Time.time;
        killLogPanel.killLogs.Add(killLog);
        // ų�α� ��������, 3�� ����
        if (killLogPanel.killLogs.Count > 3)
        {
            killLogPanel.RefreshKillLogPanel();
        }

        // �ʻ�ȭ ����
        if (attackerPortrait != null && deadPortrait != null)
        {
            killLog.SetKillLog(attackerPortrait, deadPortrait, attackerIdentity);
        }
    }
}
