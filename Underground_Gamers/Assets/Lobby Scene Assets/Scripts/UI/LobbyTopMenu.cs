using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTopMenu : MonoBehaviour
{
    public TMP_Text TabNameText;
    public TMP_Text[] moneyText;
    public Button backButton;
    public Button homeButton;
    public Stack<Action> functionStack = new Stack<Action>();


    public void ActiveTop(bool on)
    {
        gameObject.SetActive(on);
    }
    public void UpdateMoney()
    {
        moneyText[0].text = GamePlayerInfo.instance.money.ToString();
        moneyText[1].text = GamePlayerInfo.instance.crystal.ToString();
    }

    public void AddFunction(Action action)
    {
        functionStack.Push(action);
    }

    public void ExecuteFunction()
    {
        Action function = functionStack.Pop();
        function.Invoke();
    }
    public void ExecuteFunction(int count)
    {
        int executedCount = 0;

        while (functionStack.Count > 0 && executedCount < count)
        {
            Action function = functionStack.Pop();
            function.Invoke();
            executedCount++;
        }
    }
    public void ExecuteAllFunction()
    {
        while (functionStack.Count > 0)
        {
            Action function = functionStack.Pop();
            function.Invoke();
        }
    }
}
