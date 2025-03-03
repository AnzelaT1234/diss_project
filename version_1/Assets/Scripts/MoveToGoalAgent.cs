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
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;

public class MoveToGoalAgent : Agent
{
    // [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private GameObject kitchen;
    // [SerializeField] private MoveToKitchenAgent kitchenAgent;
    // [SerializeField] private BringFoodAgent server;
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private CustomerArrival fn;
    // [SerializeField] private MainGame main;
    // [Header("X walls")]
    // [SerializeField] private Transform[] xwalls;

    // [Header("Z walls")]
    // [SerializeField] private Transform[] zwalls;


    public GameObject table;
    private Transform targetTransform;

    private Rigidbody rb;
    private float distance;
    private bool takingOrder;
    private bool bringingFood;
    private Quaternion r;
    private Transform t;
    private Vector3 s;
    private float wallOffset;
    private float default_offset;


    public override void Initialize()
    {
        transform.localPosition = startPos;
        rb = GetComponent<Rigidbody>();
        targetTransform = table.transform;
        distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        // kitchenAgent.gameObject.SetActive(false);

        // server.target = target;
        // server.gameObject.SetActive(false);

        takingOrder = false;
        bringingFood = false;
        r = Quaternion.identity;
        t = table.transform;
        s = startPos;
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPos;
        targetTransform = t;
        transform.rotation = r;
    }

    IEnumerator Wait(float sec)
    {
        Debug.Log("Waiting for " + sec + " seconds...");
        yield return new WaitForSeconds(sec); // Waits for 5 real-time seconds
        Debug.Log(sec + " seconds passed! Now executing...");

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // normalise positions such that the values are between 1 and -1
        // helps with stability in the NN
        float localx = transform.localPosition.x / 12f;
        float localz = transform.localPosition.z / 12f;

        // dividing by 360 gives a value between 0 and 1
        // multiplying by two stretches the values to between 0 and 2 (0 remains 0, 1 goes to 2)
        // -1f ensures the range is [-1,1]
        // Note: if we just -1f then the values remain between [-1,0] which we don't want
        float localRotation = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        float targetx = targetTransform.localPosition.x / 12f;
        float targetz = targetTransform.localPosition.z / 12f;

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

        // rb.MovePosition(transform.localPosition + transform.forward * forward * moveSpeed * Time.deltaTime);
        // transform.Rotate(0f, rot * 90 * Time.deltaTime, 0f, Space.Self);

        // encourage agent to move towards the goal
        // float curr_distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        // float reward = 1f/curr_distance;
        // AddReward(reward);

        // // encourage agent by giving positive rewards if they are facing and negative if not
        // float dot = Vector3.Dot(transform.forward, (targetTransform.localPosition - transform.localPosition).normalized);
        // AddReward(dot+1);

        // small punishment for every step
        // encourages agent to go faster
        // AddReward(-1/MaxStep);
        
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxis("Horizontal");

        discreteActions[0] = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;//(Input.GetKey(KeyCode.DownArrow) ? 2 : 0);
    }

    public void Move(int actions)
    {
        switch (actions)
        {
            case 1:
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                break;
            }
            case 2:
            {
                transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
                break;
            }
            case 3:
            {
                transform.Rotate(0f, -1*rotateSpeed * Time.deltaTime, 0f);
                break;
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goal_tag"))
        {
            Debug.Log("Hit Goal!");
            AddReward(1.0f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall_tag"))
        {
            Debug.Log("Hit wall");
            AddReward(-0.5f);
        }
    }

    // While agent is still on wall...
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall_tag"))
        {
            Debug.Log("Staying on wall");
            AddReward(-0.5f);
        }
    }
}
