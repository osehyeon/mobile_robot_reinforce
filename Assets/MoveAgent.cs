using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveAgent : Agent
{
    float preDist;

    [SerializeField] private Transform target;
    public Transform obstracle;

    public override void CollectObservations(VectorSensor sensor)
    {
        // 목표와 에이전트의 위치
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.position);
        sensor.AddObservation(obstracle.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        AddReward(-0.01f);
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime;;

        float distance = Vector3.Magnitude(transform.position - target.position);

        float reward = preDist - distance;
        AddReward(reward);
        preDist = distance;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("target"))
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (other.collider.CompareTag("wall"))
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-3, 0.25f , 0);
        preDist = Vector3.Magnitude(transform.position - target.position);
        obstracle.localPosition = new Vector3(0.0f, 0.5f, Random.Range(-3.0f, 3.0f));
        //target.localPosition = new Vector3(3.0f, 0.5f, Random.Range(-3.0f, 3.0f));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionOut = actionsOut.ContinuousActions;
        continuousActionOut[0] = Input.GetAxisRaw("Horizontal");
        continuousActionOut[1] = Input.GetAxisRaw("Vertical");
    }
}
