using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;

public class GoblinControl : MonoBehaviour {
    public enum GoblinState { None, Idle, Patrol, Wait, MoveToTarget, Attack, Damage, Die }

    [Header("기본 속성")]
    public GoblinState goblinState = GoblinState.None;

    public float MoveSpeed = Score.enemySpeed;
    public GameObject TargetPlayer = null;
    public Transform TargetTransform = null;
    public Vector3 TargetPosition = Vector3.zero;

    private Animation myAnimation = null;
    private Transform myTransform = null;

    public AnimationClip IdleAnimClip = null;
    public AnimationClip MoveAnimClip = null;
    public AnimationClip AttackAnimClip = null;
    public AnimationClip DamageAnimClip = null;
    public AnimationClip DieAnimClip = null;

    [Header("전투속성")]
    public double HP = Score.enemyHP;
    public float AttackRange = 0.5f;
    public GameObject DamageEffect = null;
    public GameObject DieEffect = null;
    private Tweener effectTweener = null;
    private SkinnedMeshRenderer skinMeshRenderer = null;

    public MonsterSpawner monsterSpawner;
    public GoblinHealth goblinHealth;

    // Use this for initialization
    void Start() {
        //대기 상태로.
        goblinState = GoblinState.Idle;
        //캐싱.
        myAnimation = GetComponent<Animation>();
        myTransform = GetComponent<Transform>();
        //애니메이션 클립들 기본 세팅.
        myAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[MoveAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[AttackAnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[DamageAnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[DamageAnimClip.name].layer = 10;
        myAnimation[DieAnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[DieAnimClip.name].layer = 10;
        //애니메이션 이벤트 추가.
        AddAnimationEvent(AttackAnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(DamageAnimClip, "OnDamageAnimFinished");
        AddAnimationEvent(DieAnimClip, "OnDieAnimFinished");
        //스킨 메쉬를 캐싱.
        skinMeshRenderer = myTransform.Find("body").GetComponent<SkinnedMeshRenderer>();
        monsterSpawner = GameObject.FindObjectOfType<MonsterSpawner>();
        goblinHealth = GameObject.FindObjectOfType<GoblinHealth>();
    }
    /// <summary>
    /// 고블린의 상태에따라 동작을 제어하는 함수.
    /// </summary>
    void CheckState()
    {
        switch (goblinState)
        {
            case GoblinState.Idle:
                IdleUpdate();//idle -> moveToTarget, Patrol
                break;
            case GoblinState.MoveToTarget:
            case GoblinState.Patrol:
                MoveUpdate(); // move -> wait, attack
                break;
            case GoblinState.Attack:
                AttackUpdate(); //attack -> die,moveToTarget
                break;

        }
    }
    /// <summary>
    /// 대기 상태일때의 동작.
    /// </summary>
	void IdleUpdate()
    {
        //만약 타겟 플레이어가 없다면, 임의 지점을 랜덤하게 선택해서 레이캐스트를 이용하여
        //임의의 지점의 높이값까지 구해서 그 임의의 지점으로 이동시켜주도록 합니다.
        if (TargetPlayer == null)
        {
            TargetPosition = new Vector3(myTransform.position.x + Random.Range(-10.0f, 10.0f),
                                        myTransform.position.y + 1000.0f,
                                        myTransform.position.z + Random.Range(-10.0f, 10.0f));
            Ray ray = new Ray(TargetPosition, Vector3.down);
            RaycastHit raycastHit = new RaycastHit();
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
            {   //임의의 위치의 높이값.
                TargetPosition.y = raycastHit.point.y;
            }
            //상태를 정찰 상태로 변경.
            goblinState = GoblinState.Patrol;
        } else
        {
            //타겟이있다면, 타겟을 향해 이동합니다.
            goblinState = GoblinState.MoveToTarget;
        }
    }

    //이동 상태에서의 동작 .patrol,MoveToTarget
    void MoveUpdate()
    {
        Vector3 diff = Vector3.zero;
        Vector3 lookAtPosition = Vector3.zero;

        switch (goblinState)
        {
            case GoblinState.Patrol:
                if (TargetPosition != Vector3.zero)
                {
                    diff = TargetPosition - myTransform.position;
                    //목표지점까지 거의 왔으면.
                    if (diff.magnitude < AttackRange)
                    {
                        StartCoroutine(WaitUpdate());
                        return;
                    }

                    lookAtPosition = new Vector3(TargetPosition.x,
                                                myTransform.position.y,
                                                TargetPosition.z);
                }
                break;
            case GoblinState.MoveToTarget:
                if (TargetPosition != null)
                {
                    diff = TargetPlayer.transform.position - myTransform.position;
                    if (diff.magnitude < AttackRange)
                    {   //공격 상태로 변경합니다.
                        goblinState = GoblinState.Attack;
                        return;
                    }
                    lookAtPosition = new Vector3(TargetPlayer.transform.position.x,
                                                myTransform.position.y,
                                                TargetPlayer.transform.position.z);
                }
                break;
        }

        Vector3 direction = diff.normalized;
        direction = new Vector3(direction.x, 0.0f, direction.z);
        Vector3 moveAmount = direction * MoveSpeed * Time.deltaTime;

        myTransform.Translate(moveAmount, Space.World);
        myTransform.LookAt(lookAtPosition);
    }

    /// <summary>
    /// 대기 동작.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitUpdate()
    {
        goblinState = GoblinState.Wait;
        float waitTime = Random.Range(1.0f, 3.0f);
        yield return new WaitForSeconds(waitTime);
        goblinState = GoblinState.Idle;
    }
    /// <summary>
    /// 애니메이션을 재생시켜주는 함수.
    /// </summary>
    void AnimationControl()
    {
        switch (goblinState)
        {   
            case GoblinState.Wait:
            case GoblinState.Idle:
                myAnimation.CrossFade(IdleAnimClip.name);
                break;
            case GoblinState.Patrol:
            case GoblinState.MoveToTarget:
                myAnimation.CrossFade(MoveAnimClip.name);
                break;
            case GoblinState.Attack:
                myAnimation.CrossFade(AttackAnimClip.name);
                if (myAnimation[AttackAnimClip.name].normalizedTime <= 0.1f)
                {
                    SoundManager.instance.PlayEachSound(10, false); // EnemySwing 재생
                }
                break;
            case GoblinState.Die:
                myAnimation.CrossFade(DieAnimClip.name);
                if (myAnimation[DieAnimClip.name].normalizedTime <= 0.1f)
                {
                    SoundManager.instance.PlayEachSound(12, false); // EnemyDeathVoice 재생
                }
                break;
        }
    }
    /// <summary>
    /// 인지범위안에 다른 트리거나 플레이어가 들어왔다면 호출됩니다.
    /// </summary>
    /// <param name="target"></param>

    void OnSetTarget(GameObject target)
    {
        TargetPlayer = target;
        TargetTransform = TargetPlayer.transform;
        //타겟을 향해 이동하는 상태로 전환
        goblinState = GoblinState.MoveToTarget;
        SoundManager.instance.PlayEachSound(11, false); // EnemyAttack 재생
    }
    void AttackUpdate()
    {
        float distance = Vector3.Distance(TargetTransform.position, myTransform.position);
        if (distance > AttackRange + 0.5f)
        {
            //타겟과의 거리가 멀어졌다면 타겟으로 이동합니다.
            goblinState = GoblinState.MoveToTarget;
        }
    }
    void AttackComponentControl()
    {
        if(goblinState == GoblinState.Attack && GameControl.instance.MyGameState == GameControl.GameState.Playing)
        {
            this.gameObject.tag = "EnemyAttack";
        }
        else
        {
            this.gameObject.tag = "Untagged";
        }
    }
    /// <summary>
    /// 충돌 검출.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack") == true)
        {
            SoundManager.instance.PlayEachSound(17, false);
            HP -= 12;
            Score.instance.MyScore += Score.damageScore;

            if(FighterControl.SkillMade)
            {
                HP -= 28;
                Score.instance.MyScore += Score.damageScore * 2;
            }

            if (HP > 0)
            {
                //피격 이펙트 생성
                Instantiate(DamageEffect, other.transform.position, Quaternion.identity);
                //피격 이펙트 재생
                myAnimation.CrossFade(DamageAnimClip.name);
                //피격 트위닝 이팩트 재생
                DamageTweenEffect();
                if (Instantiate(DamageEffect, other.transform.position, Quaternion.identity))
                {
                    
                }
            }
            else
            {
                HP = 0;
                goblinState = GoblinState.Die;
            }
            goblinHealth.UpdateHealth(HP, Score.enemyHP);
        }
    }

    void DamageTweenEffect()
    {
        if (effectTweener != null && effectTweener.isComplete == false)
        {
            return;
        }
        Color colorTo = Color.red;
        effectTweener = HOTween.To(skinMeshRenderer.material, 0.2f, new TweenParms()
            .Prop("color", colorTo)
            .Loops(1, LoopType.Yoyo)
            .OnStepComplete(OnDamageTweenFinished));
    }

    void OnDamageTweenFinished()
    {
        skinMeshRenderer.material.color = Color.white;
    }
    // Update is called once per frame
    void Update () {
        if(GameControl.instance.MyGameState == GameControl.GameState.Playing)
        {
            CheckState();
            AnimationControl();
            AttackComponentControl();
            RemoveSpawnedWrong();
        }
	}
    //애니메이션 재생이 끝났을때 호출 될 이벤트 함수들.
    void OnAttackAnimFinished()
    {
    }
    void OnDamageAnimFinished()
    {
    }

    void OnDieAnimFinished()
    {
        //죽음 이펙트 생성.
        Instantiate(DieEffect, myTransform.position, Quaternion.identity);
        //몬스터 삭제.
        Destroy(gameObject);
        if(monsterSpawner.RemoveMonster(gameObject))
        {
            Score.instance.MyScore += Score.killScore;
        }
    }

    void RemoveSpawnedWrong()
    {
        if(myTransform.position.y <= -5f)
        {
            Destroy(gameObject);
            monsterSpawner.RemoveMonster(gameObject);
        }
    }
    /// <summary>
    /// 애니메이션 이벤트를 추가해주는 함수.
    /// </summary>
    /// <param name="Clip"></param>
    /// <param name="funcName"></param>
    void AddAnimationEvent(AnimationClip Clip, string funcName)
    {
        AnimationEvent newEvent = new AnimationEvent();
        newEvent.functionName = funcName;
        newEvent.time = Clip.length - 0.1f;
        Clip.AddEvent(newEvent);
    }
}
