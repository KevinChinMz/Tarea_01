using System.Collections;
using UnityEngine;

public class PatrolState : State
{
    public PatrolState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Enter()
    {        
        while (true)
        {
            int randomIndex = Random.Range(0, _stateMachine.totalCheckpoints);
            _stateMachine.SetDestination(randomIndex, 8);

            if (_stateMachine.isCloseToPlayer)
                break;            

            yield return new WaitForSeconds(_stateMachine.patrolChangeTime);
        }

        _stateMachine.SetState(new PursuitState(_stateMachine));
    }
}
