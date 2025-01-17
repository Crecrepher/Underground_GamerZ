using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedFactor : MonoBehaviour
{
    public GameObject factorX1;
    public GameObject factorX2;
    public GameObject factorX4;

    public void OnSpeedFactorX1()
    {
        factorX4.SetActive(false);
        factorX1.SetActive(true);
        Time.timeScale = 1.0f;
        GamePlayerInfo.instance.playSpeed = 1.0f;
    }
    public void OnSpeedFactorX2()
    {
        factorX1.SetActive(false);
        factorX2.SetActive(true);
        Time.timeScale = 2.0f;
        GamePlayerInfo.instance.playSpeed = 2.0f;
    }
    public void OnSpeedFactorX4()
    {
        factorX2.SetActive(false);
        factorX4.SetActive(true);
        Time.timeScale = 4.0f;
        GamePlayerInfo.instance.playSpeed = 4.0f;
    }
}
