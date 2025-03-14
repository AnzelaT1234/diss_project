using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Random=UnityEngine.Random;

public class CustomerArrival : MonoBehaviour
{
    [SerializeField] private GameObject[] tables;

    // private int prevIndex;
    private int noOfTables;

    private Vector3 customerDisplacement = new Vector3(-1.4f, 1.45f, 0.49f);

    void Start()
    {
        // initialise first index
        // prevIndex = 0;
        noOfTables = tables.Length;

        foreach (GameObject table in tables)
        {
            table.tag = "table_tag";
        }
    }

    public GameObject ChooseRandomTable(GameObject customer, int agentNum)
    {
        int tableNo = Random.Range(0,noOfTables);
        String tag = "agent" + (agentNum+1).ToString() + "goal";

        GameObject table = tables[tableNo];

        
        if (table.CompareTag(tag))
            {
                table.tag = "table_tag";
            }

        while (table.tag != "table_tag")
        {
            tableNo = Random.Range(0,noOfTables);
            table = tables[tableNo];
            if (table.CompareTag(tag))
            {
                table.tag = "table_tag";
            }
        }
        
        

        table.tag = "agent" + (agentNum+1).ToString() + "goal";
        customer.transform.localPosition = table.transform.localPosition + customerDisplacement;
        return table;

    }
}
