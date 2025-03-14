using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField] private MoveToGoalAgent agent;
    public bool isComplete;

    [Header("Cameras")]
    [SerializeField] private Camera mainGameCam;
    [SerializeField] private Camera statsCam;
    void Start()
    {
        // agent.gameObject.SetActive(false);
        isComplete = false;

        mainGameCam.enabled = true;
        statsCam.enabled = false;

        // Display.displays[1].Activate();

        

    }

    void Update()
    {
        // check if game is over
        if (isComplete)
        {
            Debug.Log("COMPLETE!");
            statsCam.enabled = true;
            mainGameCam.enabled = false;
        }
    }
    // void FixedUpdate()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         agent.gameObject.SetActive(true);

    //     }
    // }

    // public void OnAddButtonClicked()
    // {
    //     agent.gameObject.SetActive(true);
    //     Debug.Log("ADDED AGENT!");
    // }
}
