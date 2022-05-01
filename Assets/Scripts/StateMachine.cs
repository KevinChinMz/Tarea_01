using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{
    protected State _state;

    public int hp = 5;
    public float patrolChangeTime = 5f;
    public float sightDistance = 3f;
    public float harmDistance = 0.2f;
    public float graceTime = 0.8f;
    private float currentGraceTime = 0;
    private int currentCheckpoint = 0;

    [Space(5)]
    public Transform player;
    public NavMeshAgent agent;
    public Transform[] checkpoints;
    public int totalCheckpoints => checkpoints.Length;
    private float _distanceToPlayer => Vector3.Distance(transform.position, player.position);
    public bool isCloseToPlayer => _distanceToPlayer < sightDistance;
    public bool isClosetToDeath => (hp <= (hp * 0.2f) + 2);

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        SetState(new PatrolState(this));
    }
    public void SetDestination(int index, float speed)
    {
        if (index == currentCheckpoint)
            currentCheckpoint++;
        else
            currentCheckpoint = index;

        index = currentCheckpoint % checkpoints.Length;
        agent.SetDestination(checkpoints[index].position);
        agent.speed = speed;
    }
    private void FixedUpdate()
    {
        if (_distanceToPlayer <= harmDistance && Time.time > currentGraceTime)
        {
            currentGraceTime = Time.time + graceTime;
            TakeDamage();
        }
    }
    public void SetState(State state)
    {
        _state = state;
        StartCoroutine(_state.Enter());
    }
    public void TakeDamage(int damage = 1)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage | HP: {hp}");

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, harmDistance);
    }
}

