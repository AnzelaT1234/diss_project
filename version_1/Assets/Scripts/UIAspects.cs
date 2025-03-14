using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random=UnityEngine.Random;
using Unity.VisualScripting;
using System;

public class UIAspects : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI money;
    [SerializeField] TextMeshProUGUI fadeMoneyText;
    [SerializeField] TextMeshProUGUI moneySpentAgentText;
    [SerializeField] GameObject clickToChargeTextPrefab;
    [SerializeField] GameObject room;
    [SerializeField] GameObject agentPrefab;
    [SerializeField] GameObject customerPrefab;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Material[] materials;
    [SerializeField] private CustomerArrival customerArrival;
    [SerializeField] private GameObject kit;
    [SerializeField] private Canvas chargeTextParent;
    [SerializeField] private Button[] agentButtons;
    [SerializeField] private Image chargeImage;

    public bool[] isAgentAlive;
    public bool[] isAgentCharging;
    private MoveToGoalAgent[] agents;
    private int agentNo;
    private float profit;
    private float moneySpentOnAgents;

    void Start()
    {
        fadeMoneyText.gameObject.SetActive(false);
        agentNo = 0;
        profit = 0f;
        moneySpentOnAgents = 0f;
        isAgentAlive = new bool[3];
        isAgentCharging = new bool[3];
        agents = new MoveToGoalAgent[3];

        foreach (Button b in agentButtons)
        {
            b.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        moneySpentAgentText.text = "Amount spent on Agents: £" + moneySpentOnAgents.ToString("F2");
    }

    public void LowBattery(int agentNum)
    {
        Vector3 pos = agentButtons[agentNum].transform.localPosition;
        isAgentAlive[agentNum] = false;
        Debug.Log("AGENT "  + agentNum + "HAS DIED");

        TextMeshProUGUI chargeText = Instantiate(clickToChargeTextPrefab).GetComponent<TextMeshProUGUI>();
        chargeText.transform.SetParent(chargeTextParent.transform);
        Debug.Log(pos);
        pos.x += 25f;

        chargeText.transform.localRotation = Quaternion.identity;
        Debug.Log(pos);
        chargeText.transform.localPosition = pos;
        chargeText.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        StartCoroutine(FadeText(chargeText));

    }

    private void InitMat(MoveToGoalAgent agent, GameObject customer)
    {
        Material mat = materials[agentNo];
        agent.transform.Find("agent").Find("body").GetComponent<Renderer>().material = mat;
        agent.transform.Find("agent").Find("head").GetComponent<Renderer>().material = mat;
        agent.agentMat = mat;

        customer.transform.Find("customer").GetComponent<Renderer>().material = mat;
    }
    public void OnAddButtonPressed()
    {
        if (agentNo >= 3)
        {
            Debug.Log("CANT ADD NEW AGENT");
            return;
        }

        moneySpentOnAgents += 20f;

        Debug.Log("Creating agent...");
        MoveToGoalAgent newAgent = Instantiate(agentPrefab).GetComponent<MoveToGoalAgent>();
        Debug.Log("AGENT CREATED!");
        isAgentAlive[agentNo] = true;
        isAgentCharging[agentNo] = false;

        agentButtons[agentNo].gameObject.SetActive(true);

        GameObject customer = Instantiate(customerPrefab);
        customer.transform.SetParent(room.transform);
        InitMat(newAgent, customer);
        
        newAgent.transform.SetParent(room.transform);
        newAgent.transform.localPosition = startPos;

        newAgent.agentNum = agentNo;

        Camera[] camera = newAgent.GetComponentsInChildren<Camera>();
        camera[0].enabled=false;

        Light[] light = newAgent.GetComponentsInChildren<Light>();
        light[0].enabled=false;

        Camera[] custCam = customer.GetComponentsInChildren<Camera>();
        foreach (Camera c in custCam)
        {
            c.enabled = false;
        }
         Light[] custLight = customer.GetComponentsInChildren<Light>();
        foreach (Light l in custLight)
        {
            l.enabled = false;
        }

        newAgent.customer = customer;
        newAgent.fn = customerArrival;
        newAgent.ui = this;
        newAgent.kitchen = kit;
        agents[agentNo] = newAgent;
        Debug.Log("NEW AGENT ADDED: " + agentNo);
        agentNo++; 
        Debug.Log("Done Settings!");
    }

    void fadeMoneyAmount(float amount)
    {
        fadeMoneyText.gameObject.SetActive(true);
        fadeMoneyText.text = "£" + Math.Abs(amount).ToString("F2");

        if (amount>0)
        {
            fadeMoneyText.text = "+" + fadeMoneyText.text;
            fadeMoneyText.color = Color.green;
        }
        else {
            fadeMoneyText.text = "-" + fadeMoneyText.text;
            fadeMoneyText.color = Color.red;
        }

        StartCoroutine(FadeText(fadeMoneyText));
    }

    
    private IEnumerator FadeText(TextMeshProUGUI text)
    {
        text.CrossFadeAlpha(1f, 0f, true);
        text.CrossFadeAlpha(0f, 2.5f, true);
        
        yield return new WaitForSeconds(5f);
        
        text.gameObject.SetActive(false);
    }

    public void updateMoney(bool isCustomerServed, float amountToAdd = 0f)
    {
        float currAmount = float.Parse(money.text.Replace("£", "").Trim());

        if (amountToAdd==0f)
        {
            if (isCustomerServed)
            {
                float tip = Random.Range(0f, 10f);
                amountToAdd = 20f + tip;
            }
            else {
                amountToAdd = -10f;
            }
        }

        currAmount+= amountToAdd;
        fadeMoneyAmount(amountToAdd);
        profit += amountToAdd;

        if (currAmount<0)
        {
            money.text = "-£" + Math.Abs(currAmount).ToString("F2");
            moneySpentOnAgents += Math.Abs(currAmount);
        }
        else
        {
            money.text = "£" + currAmount.ToString("F2");
        }
    }

    private void ReviveAgent(int num)
    {
        agents[num].transform.localPosition = new Vector3(0f,0f,-14.1f);
        agents[num].batteryBar.fillAmount = 1.0f;
        agents[num].gameObject.SetActive(true);
    }

    IEnumerator UpdateChargeCost(float interval, float duration, int num, Image icon)
    {
        float endTime = Time.time + duration;
        isAgentCharging[num] = true;

        while (Time.time < endTime)
        {
            updateMoney(false, -1f);
            yield return new WaitForSeconds(interval);
        }

        ReviveAgent(num);
        isAgentCharging[num] = false;
        Destroy(icon);



    }

    private Image placeChargeIcon(Button button)
    {
        Image charge = Instantiate(chargeImage).gameObject.GetComponent<Image>();
        charge.transform.SetParent(chargeTextParent.transform);
        charge.transform.localScale = new Vector3(0.1f, 0.075f, 0.1f);
        Vector3 pos = button.transform.localPosition;
        pos.y += 1f;
        charge.transform.localPosition = pos;
        charge.transform.localRotation = Quaternion.identity;
        charge.transform.SetParent(chargeTextParent.transform.Find(button.tag));
        return charge;
    }

    public void OnCharge()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        
        // Debug.Log("Button pressed" + button);

        String tag = button.tag;
        int agentToCheck;
        if (tag.Contains("1"))
        {
            agentToCheck = 0;
        }
        else if (tag.Contains("2"))
        {
            agentToCheck = 1;
        }
        else 
        {
            agentToCheck = 2;
        }

        if (isAgentAlive[agentToCheck] || isAgentCharging[agentToCheck])
            {
                Debug.Log("ALIVE or Charging");
            }
        else
            {
                Image icon = placeChargeIcon(button);
                StartCoroutine(UpdateChargeCost(2f, 10f, 0, icon));
                updateMoney(false);
                moneySpentOnAgents += 10f;
            }

    }
}
