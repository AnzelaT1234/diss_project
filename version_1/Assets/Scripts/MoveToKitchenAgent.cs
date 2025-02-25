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

public class MoveToKitchenAgent : Agent
{
    // [SerializeField] private Transform targetTransform;
    public Vector3 startPos;
    [SerializeField] private BringFoodAgent server;
    [SerializeField] private Transform targetTransform;

    private Rigidbody rb;
    private float distance;
    private float multiplier;
    private Vector3 prevPos;
    private int time;


    public override void Initialize()
    {
        // this.gameObject.SetActive(false);
        startPos = new Vector3(4,0,0);
        transform.localPosition = startPos;
        rb = GetComponent<Rigidbody>();
        distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        multiplier = 20f;
        prevPos = startPos;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPos;
        prevPos = startPos;
        time+=1;
        AddReward(-5f * time);
    }

    IEnumerator Wait(float sec)
    {
        Debug.Log("Waiting for " + sec + " seconds...");
        yield return new WaitForSeconds(sec); // Waits for 5 real-time seconds
        Debug.Log(sec + " seconds passed! Now executing...");

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(targetTransform.localPosition);


    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveSpeed = 4f;
        int rotation = actions.DiscreteActions[0]; // 0=none, 1=left, 2= right
        int movement = actions.DiscreteActions[1]; // 0=none, 1=forward
        // int rot_const = actions.DiscreteActions[2]; // angle of rotation
        float rot_const = 90;

        float rotate = 0f;

        switch (rotation)
        {
            case 1:
            {
                rotate = 1f;
                break;
            }
            case 2:
            {
                rotate = -1f;
                break;
            }
            default:
            {
                rotate = 0f;
                break;
            }
        }

        float forward = 0f;

        switch (movement)
        {
            case 1:
            {
                forward = 1f;
                break;
            }
            default:
            {
                forward = 0f;
                break;
            }
        }

        rb.MovePosition(transform.localPosition + transform.forward * forward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, rotate * rot_const * Time.deltaTime, 0f, Space.Self);

        float curr_distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        float reward = 1f/curr_distance;

        if (distance<curr_distance)
        {
            reward *= -1 * curr_distance * curr_distance;
            // Debug.Log("Negative Reward");

        }
        // else {
        //     Debug.Log("Positive Reward");
        // }
           // Calculate the direction to the target
        Vector3 toTarget = (targetTransform.localPosition - transform.localPosition).normalized;

        // Get the agent's forward direction
        Vector3 agentForward = transform.forward;

        // Compute dot product (ranges from -1 to 1)
        float alignment = Vector3.Dot(agentForward, toTarget);

        // Reward agent when facing the object
        if (alignment > 0.9f) // 0.9 means nearly aligned
        {
            AddReward(5f);
        }
        else {
            AddReward(-1f);
        }

        distance = curr_distance;
        reward *= multiplier;

        AddReward(reward);

        if (transform.localPosition == prevPos)
        {
            AddReward(-20f);

        }

        prevPos = transform.localPosition;
        
        
    }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    //     continuousActions[0] = Input.GetAxisRaw("Horizontal");
    //     continuousActions[1] = Input.GetAxisRaw("Vertical");
    // }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("kit"))
        {
            Debug.Log("K: HIT GOAL!");
            this.gameObject.SetActive(false);
            server.gameObject.SetActive(true);
            server.Initialize();
            AddReward(20f);

            // int start = Random.Range(0,2);
            // Vector3[] pos = {new Vector3(2, 0, 0), new Vector3(-12, 0, 0)};
            // startPos = pos[start];

            EndEpisode();

        }

        if (other.CompareTag("wall_tag"))
        {
            Debug.Log("K: HIT WALL!");
            AddReward(-10f);
            EndEpisode();
        }

        if (other.CompareTag("table_tag") || other.CompareTag("taking_order") || other.CompareTag("goal_tag"))
        {
            Debug.Log("K: HIT TABLE!");
            AddReward(-20f);
            EndEpisode();
        }

        if (other.CompareTag("kit_wall"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}
