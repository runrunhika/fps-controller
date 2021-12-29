using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Using to Ai
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    public float walkingSpeed;

        //Playerオブジェクト格納
    GameObject target;
    public float runSpeed;

    public int attackDamage;

    public GameObject ragdoll;

    public AudioSource zomVoice;
    public AudioClip howl, attack;

    //列挙型の作成
        //ゾンビのパターンを作り、パターンごとに異なる処理をさせることが可能
    enum STATE {
        IDLE,WANDER,ATTACK,CHASE,DEAD
    };
        //初期の状態を宣言
    STATE state = STATE.IDLE;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        Howl();
    }

    //Playerを追いかける＝＞攻撃する　というとき　追いかけるという処理を止めるため
    public void TurnOffTrigger()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        animator.SetBool("Death", false);
        animator.SetBool("Attack", false);
    }

    float DistanceToPlayer()
    { 
        if(GameState.GameOver)
        {
            //Deadで目的地を見失う
            return Mathf.Infinity;
        }
                  //PlayerとZombieの距離を取得し返す
        return Vector3.Distance(target.transform.position, transform.position);
    }

    bool CanSeePlayer()
    {
        if(DistanceToPlayer() < 15)
        {
            return true;
        }
        return false;
    }

    bool ForGetPlayer()
    {
        if(DistanceToPlayer() > 20)
        {
            return true;
        }
        return false;
    }

    //攻撃を与える関数
    public void DamagePlayer()
    {
        if (target != null)
        {
            AttackSE();
            target.GetComponent<FPSController>().TakeHit(attackDamage);
        }
    }

    public void ZombieDeath()
    {
        TurnOffTrigger();
        animator.SetBool("Death", true);
        state = STATE.DEAD;
    }

    public void Howl()
    {
        if(!zomVoice.isPlaying)
        {
            zomVoice.clip = howl;
            zomVoice.Play();
        }
    }

    public void AttackSE()
    {
        zomVoice.clip = attack;
        zomVoice.Play();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.IDLE:
                TurnOffTrigger();

                if(CanSeePlayer())
                {
                    state = STATE.CHASE;
                }
                else if(Random.Range(0, 5000) < 5)
                {
                    state = STATE.WANDER;
                }

                if(Random.Range(0, 5000) < 5)
                {
                    Howl();
                }
                
                break;

            case STATE.WANDER:
                //目的地を持っていないとき　目的地を決めてあげる
                if(!agent.hasPath)
                {
                    float newX = transform.position.x + Random.Range(-5, 5);
                    float newZ = transform.position.z + Random.Range(-5, 5);

                    Vector3 NextPos = new Vector3(newX, transform.position.y, newZ);

                    //目的地設定
                    agent.SetDestination(NextPos);
                    //目的地到着で止まる
                    agent.stoppingDistance = 0;

                    TurnOffTrigger();

                    agent.speed = walkingSpeed;
                    animator.SetBool("Walk", true);
                }

                if(Random.Range(0, 500) < 10)
                {
                    state = STATE.IDLE;
                    agent.ResetPath();
                }

                if(CanSeePlayer())
                {
                    state = STATE.CHASE;
                }

                if(Random.Range(0, 5000) < 5)
                {
                    Howl();
                }

                break;

            case STATE.CHASE:
                if(GameState.GameOver)
                {
                    TurnOffTrigger();
                    agent.ResetPath();
                    state = STATE.WANDER;

                    return;
                }
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 2;

                TurnOffTrigger();

                agent.speed = runSpeed;
                animator.SetBool("Run", true);

                //近づき止まると攻撃へ
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    state = STATE.ATTACK;
                }

                if(ForGetPlayer())
                {
                    agent.ResetPath();
                    state = STATE.WANDER;
                }

                break;

            case STATE.ATTACK:
                if(GameState.GameOver)
                {
                    TurnOffTrigger();
                    agent.ResetPath();
                    state = STATE.WANDER;

                    return;
                }
                TurnOffTrigger();
                animator.SetBool("Attack", true);
                
                //Playerのほうを向く
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

                if(DistanceToPlayer() > agent.stoppingDistance + 2)
                {
                    state = STATE.CHASE;

                    Howl();
                }
                break;

            case STATE.DEAD:

                zomVoice.Stop();

                Destroy(agent);

                gameObject.GetComponent<DestroyZombie>().DeadZombie();
                break;
        }
    }
}
