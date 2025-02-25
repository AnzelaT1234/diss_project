using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using System.Diagnostics.Tracing;
using Random=UnityEngine.Random;
using System.Linq.Expressions;

public class MoveToGoalAgent : Agent
{
    // [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private GameObject kitchen;
    [SerializeField] private MoveToKitchenAgent kitchenAgent;
    [SerializeField] private BringFoodAgent server;

    public GameObject target;
    private Transform targetTransform;

    private Rigidbody rb;
    private float distance;
    private Vector3 prevPos;
    private int step;
    


    public override void Initialize()
    {
        transform.localPosition = startPos;
        rb = GetComponent<Rigidbody>();
        targetTransform = target.transform;
        distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        kitchenAgent.gameObject.SetActive(false);

        server.target = target;
        server.gameObject.SetActive(false);

        prevPos = startPos;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPos;
        targetTransform = target.transform;
        transform.rotation = Quaternion.identity;
        step = 0;
    }

    IEnumerator Wait(float sec)
    {
        Debug.Log("Waiting for " + sec + " seconds...");
        yield return new WaitForSeconds(sec); // Waits for 5 real-time seconds
        Debug.Log(sec + " seconds passed! Now executing...");

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(transform.localRotation);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveSpeed = 5f;
        int rotation = actions.DiscreteActions[1]; // 0=none, 1=left, 2= right
        int movement = actions.DiscreteActions[0]; // 0=none, 1=forward
        int forward;
        int rot;

        switch (rotation)
        {
            case 1:
            {
                rot = 1;
                break;
            }
            case 2:
            {
                rot = -1;
                break;
            }
            default:
            {
                rot = 0;
                break;
            }
        }

        // float forward = 0f;

        switch (movement)
        {
            case 1:
            {
                forward = 1;
                break;
            }
            default:
            {
                forward = 0;
                break;
            }
        }

        rb.MovePosition(transform.localPosition + transform.forward * forward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, rot * 90 * Time.deltaTime, 0f, Space.Self);

        // encourage agent to move towards the goal
        float curr_distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        // float reward = 1f/curr_distance;
        if (distance <= curr_distance)
        {
            AddReward(-1f);
        }
        else
        {
            AddReward(1f);
        }

        // encourage agent by giving positive rewards if they are facing and negative if not
        float dot = Vector3.Dot(transform.forward, (targetTransform.localPosition - transform.localPosition).normalized);
        AddReward(dot);

        // AddReward(-1*step);
        // step++;
        
        
    }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    //     continuousActions[0] = Input.GetAxisRaw("Horizontal");
    //     continuousActions[1] = Input.GetAxisRaw("Vertical");
    // }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goal_tag"))
        {
            Debug.Log("HIT GOAL!");
            AddReward(10f);
            // StartCoroutine(Wait(5f));
            // other.tag = "taking_order";

            // Vector3 tableStart = targetTransform.localPosition;
            // tableStart.x -= 4.0f; // ensure agents stands next to table not in it
            // kitchenAgent.startPos = tableStart;
            // kitchenAgent.gameObject.SetActive(true);
            // // kitchen.tag = "goal_tag";
            // kitchenAgent.Initialize();
            // this.gameObject.SetActive(false);
            EndEpisode();


        }
        else 
        {
            // Debug.Log("Lose reward!");
            AddReward(-10f);
            EndEpisode();
            // transform.localPosition = startPos;
            // targetTransform = target.transform;
            // transform.rotation = Quaternion.identity;
        }

        // if (other.CompareTag("wall_tag") || other.CompareTag("kit") || other.CompareTag("kit_wall"))
        // {
        //     Debug.Log("HIT WALL!");
        //     AddReward(-1f);
        //     EndEpisode();
        // }

        // if (other.CompareTag("table_tag") || other.CompareTag("taking_order"))
        // {
        //     Debug.Log("HIT TABLE!");
        //     AddReward(-0.8f);
        //     EndEpisode();
        // }
    }
}
