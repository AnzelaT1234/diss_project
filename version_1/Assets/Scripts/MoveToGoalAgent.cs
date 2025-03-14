using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using System.Diagnostics.Tracing;
using Random=UnityEngine.Random;
using System.Linq.Expressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using Unity.Mathematics;
using UnityEngine.Rendering;

public class MoveToGoalAgent : Agent
{
    // [SerializeField] private Transform targetTransform;
    public Vector3 startPos;

    // [SerializeField] private MoveToKitchenAgent kitchenAgent;
    // [SerializeField] private BringFoodAgent server;
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private Material waitingMat;
    [SerializeField] private Material orderingMat;
    public GameObject kitchen;
    public Material agentMat;
    public UIAspects ui;
    public GameObject customer;
    public CustomerArrival fn;
    
    public Image batteryBar;

    public bool isAlive;

    private GameObject table;
    private Transform targetTransform;
    private Transform _food;

    private Rigidbody rb;
    private float distance;
    private bool takingOrder;
    private bool bringingFood;
    private int completedEpisodes;
    private int successes;
    private float score;
    public float maxBattery = 1.0f;
    public int agentNum;


    public override void Initialize()
    {
        Debug.Log("AGENT INITIALISING...");
        transform.localPosition = startPos;
        rb = GetComponent<Rigidbody>();
        // targetTransform = table.transform;

        _food = transform.Find("plate");
        batteryBar = transform.Find("batteryLife").Find("battery").GetComponent<Image>();

        successes = 0;
        completedEpisodes = 0;
        score = 0;
        isAlive = true;

        Debug.Log("END OF INITIALISATION");
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("BEGINNING EPISODE");
        // transform.localPosition = startPos;
        rb.rotation = Quaternion.identity;
        // transform.localPosition = startPos;
        // ensures a new random table is chosen in case the episode is over but the agent hasn't served a customer
        // if (table.tag=="taking_order" || table.tag == "goal_tag")
        // {
        //     targetTransform.tag = "served";
        // }

        Debug.Log("Getting table...");
        table = fn.ChooseRandomTable(customer);

        customer.transform.Find("status").GetComponent<Renderer>().material = orderingMat;

        kitchen.tag = "kit";
        
        targetTransform = table.transform;
        takingOrder = false;
        bringingFood = false;
        _food.gameObject.SetActive(false);

        Debug.Log("No. of cycles complete: " + successes);
        Debug.Log("No. of episodes complete: " + completedEpisodes);
        Debug.Log("Cumulative Reward from last episode: " + score);

        completedEpisodes++;
        score = 0;
        
    }

    // IEnumerator Wait(float sec)
    // {
    //     Debug.Log("Waiting for " + sec + " seconds...");
    //     yield return new WaitForSeconds(sec); // Waits for 5 real-time seconds
    //     Debug.Log(sec + " seconds passed! Now executing...");

    // }

    public override void CollectObservations(VectorSensor sensor)
    {
        // normalise positions such that the values are between 1 and -1
        // helps with stability in the NN
        float localx = transform.localPosition.x / 19f;
        float localz = transform.localPosition.z / 16f;

        // dividing by 360 gives a value between 0 and 1
        // multiplying by two stretches the values to between 0 and 2 (0 remains 0, 1 goes to 2)
        // -1f ensures the range is [-1,1]
        // Note: if we just -1f then the values remain between [-1,0] which we don't want
        float localRotation = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        float targetx = targetTransform.position.x / 19f;
        float targetz = targetTransform.position.z / 16f;

        float isTakingOrder = takingOrder ? 1.0f : 0.0f;

        sensor.AddObservation(isTakingOrder);
        sensor.AddObservation(localx);
        sensor.AddObservation(localz);
        sensor.AddObservation(localRotation);
        sensor.AddObservation(targetx);
        sensor.AddObservation(targetz);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // float moveSpeed = 7.5f;

        // // rotation should be a float
        // float rot = actions.ContinuousActions[0];

        // // forward is an integer
        // int forward = actions.DiscreteActions[0]; // 0 for stop, 1 for move
        // forward = (forward == 2) ? -1 : forward;
        Move(actions.DiscreteActions[0]);

        Quaternion rot = rb.rotation;
        rb.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);

        // ensure agent doesn't rotate around the other axes
        // transform.rotation = new Quaternion (0, rot.y, 0, 0);


        // rb.MovePosition(transform.localPosition + transform.forward * forward * moveSpeed * Time.deltaTime);
        // transform.Rotate(0f, rot * 90 * Time.deltaTime, 0f, Space.Self);

        // encourage agent to move towards the goal
        float curr_distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        float reward = 1f/curr_distance;
        AddReward(reward);
        score += reward;

        // // encourage agent by giving positive rewards if they are facing and negative if not
        // float dot = Vector3.Dot(transform.forward, (targetTransform.localPosition - transform.localPosition).normalized);
        // AddReward(dot+1);

        // small punishment for every step
        // encourages agent to go faster
        AddReward(-1/MaxStep);
        score -= 1/MaxStep;
        
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[0] = 3;
        }
    }

    public void Move(int actions)
    {

        switch (actions)
        {
            case 0:
            {
                rb.angularVelocity = Vector3.zero;
                break;
            }
            case 1:
            {
                rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
                rb.angularVelocity = Vector3.zero;
                break;
            }
            case 2:
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0));
                break;
            }
            case 3:
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -1 * rotateSpeed * Time.deltaTime, 0));
                break;
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goal_tag"))
        {
            // Debug.Log("Hit Goal!");
            AddReward(0.5f);
            score += 0.5f;
            

            if (takingOrder) // i.e. goes to kitchen successfully
            {
                AddReward(0.3f);
                takingOrder = false;
                bringingFood = true;

                // make food appear
                _food.gameObject.SetActive(true);
                // Debug.Log("Got to kitchen");
                targetTransform = table.transform;
                targetTransform.tag = "goal_tag";
                kitchen.tag = "kit";
                score += 1.0f;
            }
            else if (bringingFood) // i.e. last step of the routine
            {
                AddReward(0.5f);
                // Debug.Log("Served Customer");
                table.tag = "served";
                successes++;
                score += 1.5f;
                ui.updateMoney(true);
                UpdateBatteryLife();
                EndEpisode(); // only end episode when full routine is complete
            }
            else // i.e. hit table for first time
            {
                AddReward(0.2f);
                customer.transform.Find("status").GetComponent<Renderer>().material = waitingMat;
                // Debug.Log("Hit table for first time!");
                table.tag = "taking_order";
                // customer.material = waitingMat;
                targetTransform = kitchen.transform;
                kitchen.tag = "goal_tag";
                takingOrder = true;
                score += 0.5f;

            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("COLLISSION!");
        if (!collision.gameObject.CompareTag("floor"))
        {
            // Debug.Log("Entered Collision");
            AddReward(-0.3f);
            score -= 0.3f;
        }
        // Debug.Log("Collision!");
        // AddReward(-0.5f);
    }

    // While agent is still on wall...
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("floor"))
        {
            // Debug.Log("Staying on collision");
            AddReward(-0.3f);
            score -= 0.3f;
        }
    }

    public void UpdateBatteryLife()
    {
        batteryBar.fillAmount -= 0.5f;
        if (batteryBar.fillAmount <= 0f)
        {
            this.gameObject.SetActive(false);
            ui.LowBattery(agentNum);
            isAlive = false;
        }
    }
}
