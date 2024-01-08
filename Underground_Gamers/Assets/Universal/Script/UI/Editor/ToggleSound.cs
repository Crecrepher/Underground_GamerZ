using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    public EffectType effectType = EffectType.Option_Button;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick; // �̺�Ʈ ���� ����

        // �̺�Ʈ�� ������ �Լ� �߰�
        entry.callback.AddListener((eventData) => SoundPlayer.instance.PlayEffectSound((int)effectType)) ;

        // EventTrigger�� �̺�Ʈ �ڵ鷯 �߰�
        eventTrigger.triggers.Add(entry);
    }
}
