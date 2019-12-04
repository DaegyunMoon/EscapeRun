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

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void AgentReset()
    {
        this.transform.position = new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f), 
            6.0f, UnityEngine.Random.Range(-8.0f, 8.0f)) + terrainTransform.position;
        target.transform.position = new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f),
            6.0f, UnityEngine.Random.Range(-8.0f, 8.0f)) + terrainTransform.position;
        unitBody.velocity = Vector3.zero;
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
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.001f);
        float v = 1.0f;
        float h = vectorAction[0];
        float s = 0.0f;
        float j = 0.0f;

        player.Move(v, h, s, j);

        if (player.playerState == PlayerLearning.PlayerState.Death)
        {
            Debug.Log("보상 : 사망");
            AddReward(-1.0f);
            Done();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("보상 : 타겟을 잡음");
            target.transform.position = new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f), 
                6.0f, UnityEngine.Random.Range(-8.0f, 8.0f)) + terrainTransform.position;
            AddReward(2.0f);
        }
        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("보상 : 아이템 획득");
            AddReward(0.25f);
            if (player.GetHP() < 50.0f)
            {
                Debug.Log("보상 : 피로도 회복");
                AddReward(0.75f);
            }
        }
    }

    public void CheckSprint(float sprintStart, float sprintEnd)
    {
        float sprintTime = sprintEnd - sprintStart;
        if (sprintTime > 2.0f)
        {
            if (player.GetHP() > 5.0f)
            {
                Debug.Log("보상 : Sprint");
                AddReward(0.001f);
            }
            else
            {
                Debug.Log("보상 : 탈진 위험");
                AddReward(-0.01f);
            }
        }
        else
        {
            Debug.Log("보상 : 불필요한 Sprint");
            AddReward(-1.0f);
        }
    }
}
