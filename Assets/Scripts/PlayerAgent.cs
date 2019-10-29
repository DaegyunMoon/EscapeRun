using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAgent : Agent
{
    public Rigidbody unitBody;
    public Transform unitTransform;
    public Transform targetTransform;
    private TargetSpawner targetSpawner;
    // Start is called before the first frame update
    void Start()
    {
        targetSpawner = FindObjectOfType<TargetSpawner>();
    }

    public override void AgentReset()
    {
        base.AgentReset();
    }
    public override void CollectObservations()
    {
        base.CollectObservations();
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
    }
    public void OnTriggerEnter(Collider other)
    {
        
    }
}
