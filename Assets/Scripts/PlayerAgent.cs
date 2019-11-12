using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class PlayerAgent : Agent
{
    public PlayerControl playerControl;
    public Rigidbody unitBody;
    public Transform terrainTransform;
    public Transform targetTransform;
    private TargetSpawner targetSpawner;

    private int thresholdTime;
    private float beforeDistanceToTarget;

    // Start is called before the first frame update
    void Start()
    {
        targetSpawner = FindObjectOfType<TargetSpawner>();
    }

    public override void AgentReset()
    {
        this.transform.position = new Vector3(42.5f, 5.0f, 32.5f);
        unitBody.velocity = Vector3.zero;
    }
    public override void CollectObservations()
    {
        float temp = 500.0f;
        Vector3 closestDistance = Vector3.zero;
        Vector3 relativeDistance = Vector3.zero;
        foreach(GameObject target in targetSpawner.targetList)
        {
            Vector3 distanceToTarget = target.transform.position - this.transform.position;
            if(temp > distanceToTarget.magnitude)
            {
                temp = distanceToTarget.magnitude;
                closestDistance = distanceToTarget;
                targetTransform = target.transform;
            }
        }
        AddVectorObs(Mathf.Clamp(closestDistance.x / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(closestDistance.z / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(closestDistance.y / 5.0f, -1.0f, 1.0f));

        relativeDistance = terrainTransform.position - this.transform.position;

        AddVectorObs(Mathf.Clamp(relativeDistance.x / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(relativeDistance.z / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(relativeDistance.y / 5.0f, -1.0f, 1.0f));

        AddVectorObs(Mathf.Clamp(playerControl.moveAmount.x / 10.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(playerControl.moveAmount.z / 10.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(unitBody.velocity.y / 10.0f, -1.0f, 1.0f));
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.01f);
        float v = vectorAction[0];
        float h = vectorAction[1];
        float s = vectorAction[2];
        float j = vectorAction[3];

        playerControl.Move(v, h, s, j);

        if(playerControl.hpbar.value <= 0.0f && playerControl.hpbar.value > -99.0f)
        {
            AddReward(-1.0f);
        }

        if (playerControl.playerState == PlayerControl.PlayerState.Death)
        {
            AddReward(-1.0f);
            Done();
        }

        if((int)Timer.time >= thresholdTime)
        {
            float nowDistanceToTarget = (targetTransform.position - this.transform.position).magnitude;
            thresholdTime += 2;
            if(beforeDistanceToTarget > nowDistanceToTarget)
            {
                AddReward(0.5f);
                Debug.Log("타겟과 가까워짐");
            }
            else
            {
                AddReward(-0.5f);
                Debug.Log("타겟과 멀어짐");
                if(s > 0.5f && playerControl.playerState != PlayerControl.PlayerState.Exhaust)
                {
                    AddReward(-0.5f);
                }
            }
            beforeDistanceToTarget = (targetTransform.position - this.transform.position).magnitude;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("Finish");
            AddReward(1.0f);
            targetSpawner.RemoveTarget(collision.gameObject) ;
        }
    }

    public void CheckOnLanding(float fallAmount, float heightBefore)
    {
        float heightAfter = this.transform.position.y;
        float heightDiffBefore = Mathf.Abs(heightBefore - targetTransform.position.y);
        float heightDiffAfter = Mathf.Abs(heightAfter - targetTransform.position.y);
        if (Math.Round(fallAmount, 2) == 1.77 && Math.Round(heightBefore, 1) == Math.Round(heightAfter, 1))
        {
            AddReward(-0.5f);
        }
        if (Math.Round(heightDiffBefore, 2) > Math.Round(heightDiffAfter, 2))
        {
            AddReward(0.5f);
        }
    }
}
