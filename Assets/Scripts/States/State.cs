using System.Collections;

public abstract class State
{    protected StateMachine _stateMachine;

    public State(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    public virtual IEnumerator Enter()
    {
        yield break;
    }
}
