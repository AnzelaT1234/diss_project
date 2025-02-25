using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class BringFoodAgent : Agent
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private MainGame main;

    public GameObject target;
    private Transform targetTransform;

    private Rigidbody rb;
    private float distance;
    private float multiplier;
    private Vector3 prevPos;

    public override void Initialize()
    {
        transform.localPosition = startPos;;
        rb = GetComponent<Rigidbody>();
        targetTransform = target.transform;
        distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        multiplier = 30f;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPos;
        targetTransform = target.transform;
        targetTransform = this.gameObject.transform;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveSpeed = 4f;
        int rotation = actions.DiscreteActions[0]; // 0=none, 1=left, 2= right
        int movement = actions.DiscreteActions[1]; // 0=none, 1=forward
        int rot;
        int forward;

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

        float curr_distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        float reward = 1f/curr_distance;

        if (curr_distance>distance)
        {
            reward = -1f * curr_distance * curr_distance;
            // Debug.Log("Negative Reward");

        }

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
        // else {
        //     Debug.Log("Positive Reward");
        // }

        distance = curr_distance;

        AddReward(multiplier * reward);

        if (transform.localPosition == prevPos)
        {
            AddReward(-10f);

        }

        prevPos = transform.localPosition;
        
        
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goal_tag"))
        {
            Debug.Log("F: HIT GOAL!");
            AddReward(150f);
            other.tag = "served";
            this.gameObject.SetActive(false);
            main.isComplete = true;
            EndEpisode();

        }

        if (other.TryGetComponent<wall_tag>(out wall_tag wall) || other.CompareTag("wall_tag"))
        {
            Debug.Log("HIT WALL!");
            AddReward(-50f);
            EndEpisode();
        }

        if (other.CompareTag("table_tag") || other.CompareTag("taking_order"))
        {
            Debug.Log("HIT TABLE!");
            AddReward(-20f);
            EndEpisode();
        }
    }
    
}
