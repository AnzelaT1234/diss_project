using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class CustomerArrival : MonoBehaviour
{
    [SerializeField] private GameObject[] tables;

    [SerializeField] private Material goalMaterial;
    [SerializeField] private Material invisMaterial;
    [SerializeField] private Material waitingMaterial;
    [SerializeField] private MoveToGoalAgent Agent;
    private int prevIndex;
    private int noOfTables;

    void Start()
    {
        // initialise first index
        prevIndex = 0;
        noOfTables = tables.Length;

        foreach (GameObject table in tables)
        {
            ChangeMat(table, invisMaterial);
        }

        ChooseRandomTable();
    }

    void Update()
    {
        CheckStatus();
    }

    void ChangeMat(GameObject tab, Material material)
    {
        Transform child = tab.transform.Find("goal");
        Renderer mat = child.GetComponent<Renderer>();
        mat.material = material;
    }

    public void ChooseRandomTable()
    {
        // Remove tag from prev
        GameObject prevTable = tables[prevIndex];
        prevTable.tag = "table_tag";

        int tableNo = Random.Range(0,noOfTables);
        if (prevIndex != tableNo)
        {
            ChangeMat(prevTable, invisMaterial);
        }

        Debug.Log("Table No. " + tableNo);
        GameObject table = tables[tableNo];
        ChangeMat(table, goalMaterial);
        table.tag = "goal_tag";

        prevIndex = tableNo;
        Agent.table = tables[tableNo];

    }

    void CheckStatus()
    {
        GameObject tab = tables[prevIndex];
        if (tab.tag == "served")
        {
            ChangeMat(tab, invisMaterial);
            ChooseRandomTable();
        }
        else if (tab.tag == "taking_order")
        {
            ChangeMat(tab, waitingMaterial);
        }
    }
}
