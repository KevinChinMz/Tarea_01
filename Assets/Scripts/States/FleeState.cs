using System.Collections;
using UnityEngine;


public class FleeState : State
{
    public FleeState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Enter()
    {
        int randomIndex = Random.Range(0, _stateMachine.totalCheckpoints);
        _stateMachine.SetDestination(randomIndex, 20);

        while (true)
        {
            if (_stateMachine.isCloseToPlayer)
            {
                randomIndex = Random.Range(0, _stateMachine.totalCheckpoints);
                _stateMachine.SetDestination(randomIndex, 20);
            }
            yield return new WaitForSeconds(_stateMachine.patrolChangeTime);
        }
    }
}
