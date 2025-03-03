using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField] private MoveToGoalAgent agent;
    [SerializeField] private GameObject tutorial;
    public bool isComplete;

    [Header("Cameras")]
    [SerializeField] private Camera mainGameCam;
    [SerializeField] private Camera statsCam;
    private TextMeshPro tutText;
    void Start()
    {
        // agent.gameObject.SetActive(false);
        isComplete = false;

        mainGameCam.enabled = true;
        statsCam.enabled = false;
        tutText = tutorial.GetComponentInChildren<TextMeshPro>();
        tutText.SetText("Press space to add an agent.");

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
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            agent.gameObject.SetActive(true);

        }
    }

    private void Dialogue()
    {
        tutText.SetText("This is Bobert.");
        tutText.SetText("He is an Machine Learning Agent.");
        tutText.SetText("Press space to add an agent.");
    }

    public void OnAddButtonClicked()
    {
        agent.gameObject.SetActive(true);
        Debug.Log("ADDED AGENT!");
    }
}
