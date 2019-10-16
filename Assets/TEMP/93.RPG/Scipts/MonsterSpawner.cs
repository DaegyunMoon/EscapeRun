using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    //생성할 몬스터 소스
    public GameObject SpawnMonter = null;
    public List<GameObject> MonsterList = new List<GameObject>();

    private void Update()
    {
        if(GameControl.instance.MyGameState == GameControl.GameState.Playing && TimeManager.instance.MyPlayTime % 3 == 0)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        if (MonsterList.Count >= Score.enemySpawnLimit)
        {
            return;
        }

        Vector3 spawnPos = new Vector3(Random.Range(-40.0f, 40.0f), 2.0f, Random.Range(-40.0f, 40.0f));
        Ray ray = new Ray(spawnPos, Vector3.down);
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
        {
            spawnPos.y = raycastHit.point.y;
        }
        GameObject newMonster = Instantiate(SpawnMonter, spawnPos, Quaternion.identity);
        MonsterList.Add(newMonster);
    }

    public bool RemoveMonster(GameObject gameObject)
    {
        return MonsterList.Remove(gameObject);
    }
}
