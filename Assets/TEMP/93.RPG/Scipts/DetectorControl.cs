using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorControl : MonoBehaviour
{
    private Transform myTransform = null;
    public GameObject Target = null;
    private Transform targetTransform = null;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        if (Target != null)
        {
            
            targetTransform = Target.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 타겟의 위치를 가져옵니다.
        myTransform.position = targetTransform.position;
        myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z);

        myTransform.LookAt(targetTransform);
    }
}
