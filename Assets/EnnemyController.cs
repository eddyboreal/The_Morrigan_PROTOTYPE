using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemyController : MonoBehaviour
{
    [Header("Init")]
    [Space(10)]
    public GameObject activeModel;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rigid;
    [HideInInspector]
    public NavMeshAgent agent;

    public Transform[] waypoints;
    public int actualWaypointPos;
    public Transform attackSphereDetectionCenter;
    public float sphereRadius;

    public GameObject player;
    
    [Space(10)]
    [Header("Stats")]
    public float Life = 200;
    public float damages = 27;

    [Space(10)]
    [Header("States")]
    public bool isWandering;
    public bool isFollowing;
    public bool isAttacking;
    public bool isDead;


    // Start is called before the first frame update
    void Start()
    {
        SetupAnimator();
    }
    void SetupAnimator()
    {
        if (activeModel == null)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.Log("No model found");
            }
            else
            {
                activeModel = anim.gameObject;
            }
        }
        if (anim == null)
        {
            anim = activeModel.GetComponent<Animator>();
        }

        agent = GetComponent<NavMeshAgent>();
        if(isWandering)
        agent.destination = waypoints[0].position;

        //anim.applyRootMotion = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowing && !isDead)
        {
            Collider[] hitColliders = Physics.OverlapSphere(attackSphereDetectionCenter.position, sphereRadius);
            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.tag == "Player")
                {
                    anim.CrossFade("Attack", 0.2f);
                    agent.isStopped = true;
                    isFollowing = false;
                    break;
                } 
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackSphereDetectionCenter.position, sphereRadius);
    }

    private void FixedUpdate()
    {
        if (isFollowing && !isDead)
        {
            if (agent.isStopped)
                agent.isStopped = false;
            isWandering = false;
            agent.destination = player.transform.position;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (isWandering && other.gameObject.tag == "waypoint")
            {
                SelectNewNode();
            }

            if (isWandering && !isFollowing && other.gameObject.tag == "Player")
            {
                isFollowing = true;
                anim.SetBool("playerDetected", true);
                player = other.gameObject;
                agent.destination = player.transform.position;
                agent.speed = 6;
            }
            else if(!isWandering && !isFollowing && other.gameObject.tag == "Player")
            {
                anim.SetBool("playerDetected", true);
                player = other.gameObject;
                agent.speed = 6;
            }
        }
    }

    public void SelectNewNode()
    {
        int nextNodePos = 0;
        if(actualWaypointPos != waypoints.Length - 1)
        {
            nextNodePos = actualWaypointPos+1;
        }
        actualWaypointPos = nextNodePos;
        agent.destination = waypoints[nextNodePos].position;
    }

    public void getHit(int damages)
    {
        if ((Life - damages) <= 0)
        {
            anim.CrossFade("Death", 0.1f);
            isDead = true;
            agent.isStopped = true;
            Life = 0;
        }
        else
        {
            Life -= damages;
        }
    }

    public void CheckHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(attackSphereDetectionCenter.position, sphereRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Player")
            {
                hitCollider.GetComponent<SA.StateManager>().getHit(damages);
                break;
            }
        }
    }
}
