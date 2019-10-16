using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSkill : MonoBehaviour
{
    //충돌체 검출 반경.
    public float radius = 5.0f;
    public float power = 200.0f;
    public float upwardsModifier = 3.0f;

    private void Start()
    {
        Vector3 explosionPos = transform.position;
        //explosionPos위치를 중심으로 radius 반경에 있는 모든 콜라이더를 얻어옵니다.
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag("PlayerAttack") == true)
            {
                continue;
            }
            Rigidbody rigid = col.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.AddExplosionForce(power, explosionPos, radius, upwardsModifier);
            }
        }
    }
}
