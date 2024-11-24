using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPopUpUI : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public Image timerImage;
    public TextMeshProUGUI playTimeDesc;
    public TextMeshProUGUI lostHealthDesc;
    public TextMeshProUGUI shootWaterDesc;
    public TextMeshProUGUI hitWaterDesc;
    public TextMeshProUGUI lostWaterDesc;
    public TextMeshProUGUI accuarcyWaterDesc; 
    public TextMeshProUGUI gradeDesc;

    private WaitForSeconds halfSec = new WaitForSeconds(0.5f);

    public void StartDesc(float playTime, float lostHealth, int shootWater, int hitWater){
        StartCoroutine(StartDescCoroutine(playTime, lostHealth, shootWater, hitWater));
    }

    IEnumerator StartDescCoroutine(float playTime, float lostHealth, int shootWater, int hitWater){
        yield return halfSec;
        playTimeDesc.text += String.Format("{0:0.00}", playTime) + "s";

        yield return halfSec;
        lostHealthDesc.text += String.Format("{0:0.00}", lostHealth);

        yield return halfSec;
        shootWaterDesc.text += shootWater.ToString();

        yield return halfSec;
        hitWaterDesc.text += hitWater.ToString();

        yield return halfSec;
        lostWaterDesc.text += (shootWater -hitWater).ToString();

        yield return halfSec;
        float accuracy = (shootWater == 0) ? (0.0f): (hitWater / (float)shootWater);
        accuracy *= 100.0f;
        accuarcyWaterDesc.text += String.Format("{0:0.00}", accuracy) + "%";
    }

    public void SetGrade(string grade){
        gradeDesc.text = grade;
    }
}
