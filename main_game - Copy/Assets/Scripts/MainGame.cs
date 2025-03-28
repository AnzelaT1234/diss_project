using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour
{
    [SerializeField] private MoveToGoalAgent agent;
    public bool isComplete;

    [SerializeField] private Camera mainGameCam;
    [SerializeField] private TextMeshProUGUI clock;
    [SerializeField] private UIAspects ui;
    [SerializeField] private Canvas stats;
    public String carbonEmissions;
    public String modelsUsed;
    public String profit;
    public String total;
    public String modelMoney;
    private float minutes;
    private int hours;
    public bool closingSoon;

    void Start()
    {
        isComplete = false;

        mainGameCam.enabled = true;

        clock.text = "16:00";
        minutes = 0f;
        hours = 16;
        StartCoroutine(updateTime());
        closingSoon = false;
        carbonEmissions = "";
        modelsUsed = "";
        profit = "";
        modelMoney = "";
        total = "";
    }

    void Update()
    {
        if (isComplete)
        {
            SceneManager.LoadScene("Stats");
        }
    }
    IEnumerator updateTime()
    {

        
        
        while (hours<24)
        {            
            // Debug.Log("HOURS: " + hours + " MINUTES: " + minutes + " IS CLOSING SOON: " + (hours>=16 && minutes>=15f));
            minutes += 3f;
            if (minutes >= 60f)
            {
                minutes = 0f;
                if (hours==23)
                {
                    hours = 0;
                    isComplete = true;
                }
                else
                {
                    hours++;
                }

            }

            if (hours>=23 && minutes>=30f)
            {
                closingSoon = true;
            }

            if (minutes<10)
            {
                clock.text = hours.ToString() + ":0" + Mathf.RoundToInt(minutes);
            }
            else
            {
                clock.text = hours.ToString() + ":" + Mathf.RoundToInt(minutes);
            }

            if (hours==0)
            {
                clock.text = "0" + clock.text;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");

    }


}
