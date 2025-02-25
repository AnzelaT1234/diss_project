using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField] private MoveToGoalAgent agent;
    [SerializeField] private MoveToKitchenAgent kit;
    [SerializeField] private BringFoodAgent server;
    [SerializeField] private Camera mainGameCam;
    [SerializeField] private Camera statsCam;
    public bool isComplete;

    void Start()
    {
        agent.gameObject.SetActive(false);
        kit.gameObject.SetActive(false);
        server.gameObject.SetActive(false);
        isComplete = false;

        mainGameCam.enabled = true;
        statsCam.enabled = false;

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

    public void OnAddButtonClicked()
    {
        agent.gameObject.SetActive(true);
        Debug.Log("ADDED AGENT!");
    }
}
