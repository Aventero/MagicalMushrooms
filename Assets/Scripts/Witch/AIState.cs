
public interface AIState
{
    public string StateName { get; }

    void InitState(AIStateManager stateManager);

    void UpdateState(AIStateManager stateManager);
    

    void EnterState(AIStateManager stateManager);

    
    void ExitState(AIStateManager stateManager);
}
