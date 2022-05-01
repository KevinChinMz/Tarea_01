using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class PursuitState : State
{
    public PursuitState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Enter()
    {
        NavMeshAgent agent = _stateMachine.agent;
        Transform player = _stateMachine.player;
        agent.speed = 13;

        while (_stateMachine.isCloseToPlayer)
        {
            if (_stateMachine.isClosetToDeath)
            {
                _stateMachine.SetState(new FleeState(_stateMachine));
                yield break;
            }

            agent.SetDestination(player.position);
            yield return new WaitForFixedUpdate();
        }

        yield return null;

        _stateMachine.SetState(new PatrolState(_stateMachine));
    }
}
