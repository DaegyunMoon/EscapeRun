using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class PlayerAgent : Agent
{
    public PlayerLearning player;
    public Rigidbody unitBody;
    public Transform terrainTransform;
    public GameObject target;
    public GameObject obstacle;

    void Start()
    {
    }

    public override void AgentReset()
    {
        this.transform.position = new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f), 
            3.0f, UnityEngine.Random.Range(-8.0f, 8.0f)) + terrainTransform.position;
        target.transform.position = new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f),
            3.0f, UnityEngine.Random.Range(-8.0f, 8.0f)) + terrainTransform.position;
        unitBody.velocity = Vector3.zero;
        obstacle.transform.position = new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), 
            0.5f, UnityEngine.Random.Range(-5.0f, 5.0f)) + terrainTransform.position;
    }
    public override void CollectObservations()
    {
        Vector3 distanceToTarget = target.transform.position - this.transform.position;
        Vector3 relativePosition;

        AddVectorObs(Mathf.Clamp(distanceToTarget.normalized.x, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.normalized.z, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.normalized.y, -1.0f, 1.0f));

        relativePosition = terrainTransform.position - this.transform.position;

        AddVectorObs(Mathf.Clamp(relativePosition.normalized.x, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(relativePosition.normalized.z, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(relativePosition.normalized.y, -1.0f, 1.0f));

        AddVectorObs(Mathf.Clamp(player.moveAmount.normalized.x, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(player.moveAmount.normalized.z, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(unitBody.velocity.normalized.y, -1.0f, 1.0f));

        AddVectorObs(player.GetIsGrounded());
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.001f);
        float v = 1.0f;
        float h = vectorAction[0];
        float j = vectorAction[1];

        player.Move(v, h, j);

        if (player.playerState == PlayerLearning.PlayerState.Death)
        {
            Debug.Log("보상 : 사망");
            AddReward(-1.0f);
            Done();
        }
        Monitor.Log(name, GetCumulativeReward(), transform);
    }

public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("보상 : 타겟을 잡음");
            target.transform.position = new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f), 
                3.0f, UnityEngine.Random.Range(-8.0f, 8.0f)) + terrainTransform.position;
            AddReward(2.0f);
        }
    }
    public void CheckOnLanding(float fallAmount, float heightBefore)
    {
        float heightAfter = this.transform.position.y;
        float heightDiffBefore = Mathf.Abs(heightBefore - target.transform.position.y);
        float heightDiffAfter = Mathf.Abs(heightAfter - target.transform.position.y);
        if (Math.Round(fallAmount, 2) == 1.77 && Math.Round(heightBefore, 2) == Math.Round(heightAfter, 2))
        {
            Debug.Log("보상 : 불필요한 점프");
            SetReward(-1.0f);
            Done();
        }
        else if (Math.Round(heightDiffBefore, 2) > Math.Round(heightDiffAfter, 2))
        {
            Debug.Log("보상 : 타겟과 높이가 가까워짐");
            AddReward(0.5f);
        }
        else if(fallAmount > 6.0f)
        {
            Debug.Log("높은 곳에서 추락");
            AddReward(-1.0f);
        }
    }
}
