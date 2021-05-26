using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class AgentScript : Agent
{
    public InputManager IM;
    public SimpleCarController CarScript;
    public CheckPointTest CPT;

    // Start is called before the first frame update
    void Start()
    {
        CarScript = GetComponent<SimpleCarController>();
        IM = GetComponent<InputManager>();
    }

    public override void OnEpisodeBegin()
    {
        IM.reset = true;
        CPT.reset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(CarScript.KPH/200f);
        sensor.AddObservation(CPT.distance);
    }

    // forward
    // forward right
    // right
    // back right
    // back
    // back left
    // left
    // forward left
    // handbrake
    // nothing

    public override void OnActionReceived(ActionBuffers actions)
    {
        var a = actions.DiscreteActions[0];
        IM.getAction(a);
        if (CarScript.roadStatus)
        {
            SetReward(CarScript.KPH/(1 + CPT.distance));
        }
        else if (CarScript.roadStatus == false)
        {
            SetReward(-5);
            EndEpisode();
        }
        
        Debug.Log("AgentScripts: " + a);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions[0];
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D))
                actions = 1;
            else if (Input.GetKey(KeyCode.A))
                actions = 7;
            else
                actions = 0;
        }

        else if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.D))
                actions = 3;
            else if (Input.GetKey(KeyCode.A))
                actions = 5;
            else
                actions = 4;
        }

        else if (Input.GetKey(KeyCode.A))
            actions = 6;
        else if (Input.GetKey(KeyCode.D))
            actions = 2;

        else if (Input.GetKey(KeyCode.LeftShift))
            actions = 8;
        else
            actions = 9;

        actionsOut.DiscreteActions.Array[0] = actions;

    }

}
