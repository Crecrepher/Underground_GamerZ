using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LobbyType
{
    None,
    Base,
    Recruit,
    Upgrade,
    Management,
    Schedule,
    PlayerList
}
public class LobbySceneUIManager : MonoBehaviour
{
    public static LobbySceneUIManager instance
    {
        get
        {
            if (lobbySceneUIManager == null)
            {
                lobbySceneUIManager = FindObjectOfType<LobbySceneUIManager>();
            }
            return lobbySceneUIManager;
        }
    }

    private static LobbySceneUIManager lobbySceneUIManager;

    public void Start()
    {
        SoundPlayer.instance.EnterLobbyMusic(0);
        StoryManager.instance.CheckNeedTutorial();
    }

    private Dictionary<LobbyType, LobbySceneSubscriber> lobbySceneSubscribers;
    [SerializeField]
    public LobbyTopMenu lobbyTopMenu;

    public int currUIIndex = 1;
    public void Subscribe(LobbySceneSubscriber lobbySceneSubscriber, LobbyType type)
    {
        if (lobbySceneSubscribers == null)
        {
            lobbySceneSubscribers = new Dictionary<LobbyType, LobbySceneSubscriber>();
        }
        lobbySceneSubscribers.Add(type, lobbySceneSubscriber);
    }

    public void OpenWindow(int type)
    {
        lobbySceneSubscribers[(LobbyType)currUIIndex].OnExit();
        currUIIndex = type;
        lobbySceneSubscribers[(LobbyType)type].OnEnter();
        lobbyTopMenu.UpdateMoney();
    }

    public void EmergencyOut()
    {
        foreach (var item in lobbySceneSubscribers)
        {
            item.Value.OnExit();
        }
        lobbySceneSubscribers[LobbyType.Base].OnEnter();
        lobbyTopMenu.UpdateMoney();
    }
}
