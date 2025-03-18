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

    [Header("Cameras")]
    [SerializeField] private Camera mainGameCam;
    [SerializeField] private Camera statsCam;
    [SerializeField] private TextMeshProUGUI clock;
    [SerializeField] private UIAspects ui;
    // private TextMeshProUGUI carbonEmissions;
    // private TextMeshProUGUI modelsUsed;
    // private TextMeshProUGUI profit;
    // private TextMeshProUGUI total;
    // private TextMeshProUGUI modelMoney;
    private float minutes;
    private float time;
    private int hours;
    public bool closingSoon;
    private MoveToGoalAgent[] agents;
    void Start()
    {
        // agent.gameObject.SetActive(false);
        // SceneManager.LoadScene("TitleScreen");
        // modelMoney = ui.modelMoney;
        // carbonEmissions = ui.carbonEmissions;
        // modelsUsed = ui.modelsUsed;
        // profit = ui.profit;
        // total = ui.totalText;

        

    }

    void Update()
    {
        // check if game is over
        if (isComplete)
        {
            Debug.Log("COMPLETE!");
            statsCam.enabled = true;
            mainGameCam.enabled = false;
            // CalcStats();
        }
    }
    IEnumerator updateTime()
    {
        while (hours<24)
        {
            // Debug.Log("HOURS: " + hours + " MINUTES: " + minutes + " IS CLOSING SOON: " + (hours>=16 && minutes>=15f));
            minutes += 1.5f;
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

    // void CalcStats()
    // {
    //     agents = ui.agents;
    //     float energyConsump = 0f;

    //     // from 1hr of phone usage source: https://clevercarbon.io/carbon-footprint-of-common-items 
    //     float energyConst = 172f;

    //     foreach (MoveToGoalAgent agent in agents)
    //     {
    //         energyConsump += agent.completedEpisodes * energyConst;
    //     }

    //     // 2kg = driving a car for 1hr
    //     float equiv = energyConsump / 2000;
    //     float mins = 0f;
    //     int hrs = 0;
    //     String text = "";

    //     if (equiv <= 0)
    //     {
    //         mins = equiv * 60;
    //         text = mins.ToString("F1") + " minutes";
    //     }
    //     else 
    //     {
    //         hrs = Mathf.RoundToInt(equiv);
    //         mins = (equiv - Mathf.RoundToInt(equiv)) * 60;
    //         text = hrs.ToString() + " hours and " + mins.ToString("F1") + " minutes";
    //     }

    //     carbonEmissions.text = "Your models have emitted a total of " + energyConsump.ToString("F2") + "g of CO2! That is the equivalent of leaving a car running for " + text + "!";
    //     modelsUsed.text = "No. of Models used: " + agents.Length;
    //     total.text = "Money made: £" + ui.total.ToString("F2");

    //     float p = ui.total - 100;
    //     profit.text = "Profit: £" + p.ToString("F2");
    //     modelMoney.text = "Money Spent on Agents: £" + ui.moneySpentOnAgents.ToString("F2");

    // }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
        isComplete = false;

        mainGameCam.enabled = true;
        statsCam.enabled = false;

        // Display.displays[1].Activate();
        clock.text = "16:00";
        minutes = 0f;
        time = 0f;
        hours = 16;
        StartCoroutine(updateTime());
        closingSoon = false;
    }

}
