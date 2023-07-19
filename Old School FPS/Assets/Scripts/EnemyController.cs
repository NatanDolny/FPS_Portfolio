using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    PlayerStats playerStats;

    public GameObject keycard;
    public GameObject armor;
    public bool dropsKeycard = false;

    GameObject player;
    PlayerController playerController;
    [HideInInspector] public GameObject sprite;
    GameObject head;
    GameObject torso;
    AudioSource shotAudioSource;
    AudioSource painAudioSource;
    
    public AudioClip[] painGrunts;
    public AudioClip[] deathGrunts;

    public GameObject[] covers;

    public int hitPoints = 2;
    private int originalHP;
    public bool isDead = false;
    public int attackPower = 20;
    public int sightRange = 15;
    public int attackRange = 12;
    public bool canAttack = false;
    public bool isAttacking = false;
    public float attackTimer = 0;
    public int attacksPerTen = 10;

    public bool seeksCover = false;
    public bool inCover = false;
    public bool foundCover = false;
    public int coverRange = 8;
    public GameObject chosenCover;

    public bool getsAfraid = true;
    public bool aggroed = false;

    [Range(5, 20)] public float spottingRange = 10;
    public float distanceToPlayer = 0;

    public CurrentState currentState = CurrentState.IDLE;
    public Direction direction = Direction.FRONT;

    public Animator animator;

    public Vector3 destination = Vector3.zero;
    public NavMeshAgent agent;
    public float destinationRange = 1;
    public Transform[] points;
    public int destinationPoint;

    public float stateTimer = 0;
    public float maxTimer = 2;
    public float deathTimer = 0;
    public float maxDeathTimer = 0.75f;

    public bool searching = false;
    public float _distance123;
    public float coverDistance;
    public bool coverChecked = false;
    public bool defaultCover = false;
    public bool forceIdle;

    Vector3 originalScale;

    public enum CurrentState
    {
        IDLE = 0, 
        PATROLLING = 1,
        COVERING = 2,
        ATTACKING = 3,
        AFRAID = 4
    }

    public enum Direction
    {
        FRONT = 0, 
        BACK = 1, 
        LEFT = 2, 
        RIGHT = 3,
    }

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        sprite = transform.Find("Sprite").gameObject;
        head = sprite.transform.Find("Head").gameObject;
        torso = sprite.transform.Find("Head").gameObject;
        animator = transform.GetComponentInChildren<Animator>();
        originalScale = sprite.transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerStats = player.GetComponent<PlayerStats>();
        originalHP = hitPoints;
        covers = GameObject.FindGameObjectsWithTag("Cover");
        shotAudioSource = transform.Find("ShotSource").GetComponent<AudioSource>();
        painAudioSource = transform.Find("PainSource").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!playerController.paused)
        {
            if (hitPoints < originalHP)
            {
                aggroed = true;
            }
            StateLogic();
            DirectionLogic();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerController.paused)
        {
            CheckFear();
            sprite.transform.LookAt(new Vector3(player.transform.position.x, sprite.transform.position.y, player.transform.position.z));
        }
    }

    public void StateLogic()
    {
        CheckSight(sightRange, out canAttack, out float tempDot);
        
        if (inCover && foundCover)
        {
            if (Vector3.Distance(transform.position, chosenCover.transform.Find("CoverPoint").transform.position) > 1f)
            {
                inCover = false;
            }
        }
        if(canAttack)
        {
            if (seeksCover && !inCover)
            {
                currentState = CurrentState.COVERING;
                Vector2 sortVector = new Vector2(Mathf.Infinity, 0);
                foundCover = false;
                if (covers.Length > 0)
                {
                    for (int i = 0; i < covers.Length; i++)
                    {
                        float dis = Vector3.Distance(transform.position, covers[i].transform.position);
                        if (dis < sortVector.x && dis < coverRange)
                        {
                            sortVector = new Vector2(dis, i);
                            foundCover = true;
                            coverDistance = dis;
                        }
                    }
                    if (foundCover)
                    {
                        chosenCover = covers[(int) sortVector.y];
                    }
                    
                }
                if (!foundCover)
                {
                    currentState = CurrentState.ATTACKING;
                }
            }

            if (!seeksCover || inCover)
            {
                currentState = CurrentState.ATTACKING;
            }
        }
        else
        {
            isAttacking = false;
            inCover = false;

            if (!forceIdle)
                currentState = CurrentState.PATROLLING;
            else
                currentState = CurrentState.IDLE;

            
            if (defaultCover)
            {
                currentState = CurrentState.COVERING;
            }
        }
        if (hitPoints <= 1 && getsAfraid)
        {

            currentState = CurrentState.AFRAID;
        }


        if (playerStats.playerDead)
        {
            currentState = CurrentState.IDLE;
        }

        if (hitPoints > 0)
        {
            switch (currentState)
            {
                case CurrentState.IDLE:
                    if (stateTimer < maxTimer)
                    {
                        animator.SetBool("Walking", false);
                        animator.SetBool("Attacking", false);
                        agent.destination = transform.position;
                        stateTimer += Time.deltaTime;
                    }
                    else
                    {
                        stateTimer = 0;
                        currentState = CurrentState.PATROLLING;
                    }
                    break;
                case CurrentState.PATROLLING:

                    animator.SetBool("Walking", true);
                    animator.SetBool("Attacking", false);

                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        GoToNextPoint();
                    }
                    break;
                case CurrentState.COVERING:
                    if (!inCover && seeksCover && !foundCover)
                    {
                        Vector2 sortingVector = new Vector2(Mathf.Infinity, 100);
                    }
                    if (foundCover)
                    {
                        agent.destination = chosenCover.transform.Find("CoverPoint").transform.position;
                        animator.SetBool("Walking", true);
                        if (coverDistance < 1.5f)
                        {
                            inCover = true;
                        }
                    }
                    if (inCover)
                    {
                        animator.SetBool("Walking", false);
                        agent.destination = chosenCover.transform.Find("CoverPoint").transform.position;
                    }
                    break;
                case CurrentState.ATTACKING:
                    agent.destination = transform.position;
                    animator.SetBool("Attacking", true);
                    animator.SetBool("Walking", false);
                    animator.SetInteger("Direction", 1);

                    if (!isAttacking)
                    {
                        isAttacking = true;
                        animator.SetTrigger("Attack");
                    }
                    if (isAttacking)
                    {
                        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

                        attackTimer += Time.deltaTime;
                        if (attackTimer > 10 / attacksPerTen)
                        {
                            if (!CheckSight(sightRange, out canAttack, out float dot))
                            {
                                PatrolReset();
                                return;
                            }
                            attackTimer = 0;
                            if (canAttack && !playerStats.playerDead)
                            {
                                AttackPlayer();
                                animator.SetTrigger("Attack");
                            }
                        }
                    }
                    break;
                case CurrentState.AFRAID:
                    animator.SetBool("Walking", true);
                    animator.SetBool("Attacking", false);
                    Vector2 _sortingVector = new Vector2(Mathf.Infinity, 100);
                    bool _found = false;
                    for (int i = 0; i < points.Length; i++)
                    {
                        float distance = Vector3.Distance(transform.position, points[i].transform.position);
                        if (distance < _sortingVector.x && distance > 5)
                        {
                            _sortingVector = new Vector2(distance, i);
                            _found = true;
                        }
                    }
                    if (_found)
                    {

                        agent.destination = points[(int)_sortingVector.y].transform.position;
                    }
                    break;
            }

        }
        else
        {
            if (!isDead)
            {
                animator.SetTrigger("Dead");
                painAudioSource.clip = deathGrunts[Random.Range(0, deathGrunts.Length)];
                painAudioSource.Play();
                isDead = true;
            }
            else
            {
                deathTimer += Time.deltaTime;
                if (deathTimer > maxDeathTimer)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void DirectionLogic()
    {
        float dotProduct;
        if (CheckSight(sightRange, out canAttack, out dotProduct))
        {
            direction = Direction.FRONT;
            animator.SetInteger("Direction", 1);
        }
        else
        {
            Vector3 origin = head.transform.position;
            Vector3 target = player.transform.Find("SightPoint").transform.position;
            var heading = target - origin;

            if (dotProduct < -0.75f)
            {
                direction = Direction.BACK;
                animator.SetInteger("Direction", 2);
            }
            else
            {
                if (sprite.transform.localEulerAngles.y < 180)
                {
                    animator.SetInteger("Direction", 3);
                    direction = Direction.LEFT;
                }
                else
                {
                    animator.SetInteger("Direction", 4);
                    direction = Direction.RIGHT;
                }
            }
        }
    }

    public bool CheckSight(int range, out bool can_attack, out float dot_product)
    {
        Vector3 origin = head.transform.position;
        Vector3 target = player.transform.Find("SightPoint").transform.position;
        var heading = target - origin;
        var distance = heading.magnitude;
        var _direction = heading / distance; 
        Ray ray = new Ray(origin, _direction);
        RaycastHit hit;
        var dotProduct = Vector3.Dot(transform.forward, heading.normalized);
        dot_product = dotProduct;
        if (dotProduct > 0.25 || aggroed)
        {
            if (Physics.Raycast(ray, out hit, range))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    if (hit.distance < attackRange)
                    {
                        can_attack = true;
                    }
                    else
                    {
                        can_attack = false;
                    }
                    return true;
                }
            }
        }
        if (dotProduct < 0)
            //probably fixed 
        {
            Vector3 scale = sprite.transform.localScale;
            sprite.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        }
        else
        {
            sprite.transform.localScale = originalScale;
        }
        
        can_attack = false;
        return false; 
    }

    private void GoToNextPoint()
    {
        if (points.Length == 0)
        {
            Debug.Log("points empty");
            return;
        }


        agent.destination = points[destinationPoint].position;
        destination = points[destinationPoint].position;
        if (Vector3.Distance(transform.position, points[destinationPoint].transform.position) > 0.5f)
        {
            destinationPoint = destinationPoint % points.Length;
        }
        else
        {
            destinationPoint = (destinationPoint + 1) % points.Length;
        }
    }


    private void PatrolReset()
    {
        animator.SetBool("Attacking", false);
        isAttacking = false;
        Vector2 d_p = new Vector2(Mathf.Infinity,100);
        bool found = false;
        if (points.Length != 0)
        {
            for (int i = 0; i < points.Length; i ++)
            {
                if (Vector3.Distance(transform.position, points[i].position) < d_p.x)
                {
                    d_p = new Vector2(Vector3.Distance(transform.position, points[i].position), i);
                    found = true;
                }
            }
        }
        if (found)
        {
            destinationPoint = (int)d_p.y;
            currentState = CurrentState.PATROLLING;
        }
    }

    private void AttackPlayer()
    {
        shotAudioSource.Play();

        if (playerStats.armor > 0)
        {
            playerStats.armor -= attackPower;
            if (playerStats.armor < 0)
            {
                playerStats.armor = 0;
            }
        }
        else
        {
            playerStats.health = playerStats.health - attackPower;
        }
        
        if (playerStats.health <= 0)
        {
            playerStats.PlayerDead();
            player.GetComponent<PlayerController>().PlayerHit();
        }
        else
        {
            player.GetComponent<PlayerController>().PlayerHit();
        }
    }

    public void RandomDestination(out Vector3 destination)
    {
        Vector3 selfPosition = transform.position; 
        destination = new Vector3(selfPosition.x + Random.Range(destinationRange, -destinationRange), selfPosition.y + 0, selfPosition.z + Random.Range(destinationRange, -destinationRange));
    }    

    public void CheckFear()
    {
        if (getsAfraid && hitPoints <= 1)
        {
            currentState = CurrentState.AFRAID;
        }
    }

    public void EnemyHit(int damageTaken)
    {
        hitPoints = hitPoints - damageTaken;

        if (hitPoints > 0)
        {
            painAudioSource.clip = painGrunts[(int)Random.Range(0, painGrunts.Length)];
            painAudioSource.Play();
        }
        else
        {
            EnemyDied();
        }
    }

    public void EnemyDied()
    {

        if (dropsKeycard)
        {
            GameObject keycardPickUp = Instantiate(keycard, GameObject.FindGameObjectWithTag("GameManager").transform);
            keycardPickUp.transform.position = transform.position; 
        }
        else
        {
            GameObject armorPickUp = Instantiate(armor, GameObject.FindGameObjectWithTag("GameManager").transform);
            armorPickUp.transform.position = transform.position;
        }
    }
}
