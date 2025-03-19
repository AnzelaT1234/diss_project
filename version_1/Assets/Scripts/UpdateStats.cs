using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateStats : MonoBehaviour
{
    public TextMeshProUGUI carbonEmissions;
    public TextMeshProUGUI modelsUsed;
    public TextMeshProUGUI profit;
    public TextMeshProUGUI total;
    public TextMeshProUGUI modelMoney;
    
    void Start()
    {
        CalcStats();

    }

    void CalcStats()
    {
        MoveToGoalAgent[] agents = UIAspects.Instance.agents;
        float energyConsump = 0f;

        // from 1hr of phone usage source: https://clevercarbon.io/carbon-footprint-of-common-items 
        float energyConst = 172f;
        int numOfAgents = UIAspects.Instance.agentNo;

        for (int i=0; i<numOfAgents; i++)
        {
            energyConsump += agents[i].completedEpisodes * energyConst;
        }

        // 2kg = driving a car for 1hr
        float equiv = energyConsump / 2000;
        float mins = 0f;
        int hrs = 0;
        String text = "";

        if (equiv <= 0)
        {
            mins = equiv * 60;
            text = mins.ToString("F1") + " minutes";
        }
        else 
        {
            hrs = Mathf.RoundToInt(equiv);
            mins = (equiv - Mathf.RoundToInt(equiv)) * 60;
            mins = Math.Abs(mins);
            text = mins.ToString("F1") + " minutes";
            if (hrs>0)
            {
                text = hrs.ToString() + " hours and " + text;
            }

            // text = hrs.ToString() + " hours and " + mins.ToString("F1") + " minutes";
        }

        carbonEmissions.text = "Your agents have emitted a total of " + energyConsump.ToString("F2") + "g of CO2! That is the equivalent of leaving a car running for " + text + "!";
        modelsUsed.text = "No. of agents used: " + UIAspects.Instance.agentNo;
        total.text = "Total: £" + UIAspects.Instance.total.ToString("F2");

        float p = UIAspects.Instance.total - 100;
        if (p < 0)
        {
            p = 0f;
        }
        profit.text = "Profit: £" + p.ToString("F2");
        modelMoney.text = "Money Spent on Agents: £" + UIAspects.Instance.moneySpentOnAgents.ToString("F2");

    }

    // public void BackToTitle()
    // {
    //     SceneManager.LoadScene("TitleScreen");
    // }
}
