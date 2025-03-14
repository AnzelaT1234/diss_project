using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Random=UnityEngine.Random;

public class CustomerArrival : MonoBehaviour
{
    [SerializeField] private GameObject[] tables;

    [SerializeField] private Material goalMaterial;
    [SerializeField] private Material invisMaterial;
    [SerializeField] private Material waitingMaterial;
    [SerializeField] private CustomerArrival customerArrival;

    private int prevIndex;
    private int noOfTables;

    private Vector3 customerDisplacement = new Vector3(-1.4f, 1.45f, 0.49f);

    void Start()
    {
        // initialise first index
        prevIndex = 0;
        noOfTables = tables.Length;
    }

    // void Update()
    // {
    //     CheckStatus();
    // }

    void ChangeMat(GameObject tab, Material material)
    {
        Transform child = tab.transform.Find("goal");
        Renderer mat = child.GetComponent<Renderer>();
        mat.material = material;
    }

    public GameObject ChooseRandomTable(GameObject customer)
    {

        Debug.Log("IN CHOOSE RANDOM TABLE");
        // Remove tag from prev
        GameObject prevTable = tables[prevIndex];
        prevTable.tag = "table_tag";

        int tableNo = Random.Range(0,noOfTables);
        // if (prevIndex != tableNo)
        // {
        //     ChangeMat(prevTable, invisMaterial);
        // }

        Debug.Log("Table No. " + tableNo);
        GameObject table = tables[tableNo];
        // ChangeMat(table, goalMaterial);
        table.tag = "goal_tag";
        // GameObject customer = agent.customer;
        customer.transform.localPosition = table.transform.localPosition + customerDisplacement;

        prevIndex = tableNo;
        // Agent.table = tables[tableNo];
        return table;

    }
}
