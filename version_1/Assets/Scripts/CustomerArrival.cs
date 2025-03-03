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
    [SerializeField] private MoveToGoalAgent Agent;
    private int prevIndex;
    private int noOfTables;

    void Start()
    {
        // initialise first index
        prevIndex = 0;
        noOfTables = tables.Length;
        ChooseRandomTable();
        InvokeRepeating(nameof(CheckStatus), 10.0f, 8.0f);
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
        ChangeMat(prevTable, invisMaterial);

        int tableNo = Random.Range(0,noOfTables);
        Debug.Log("Table No. " + tableNo);
        GameObject table = tables[tableNo];
        table.tag = "goal_tag";
        ChangeMat(table, goalMaterial);

        prevIndex = tableNo;
        Agent.table = tables[tableNo];

    }

    void CheckStatus()
    {
        if (tables[prevIndex].tag == "served")
        {
            ChooseRandomTable();
        }
    }
}
