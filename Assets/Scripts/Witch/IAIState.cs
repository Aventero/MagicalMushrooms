
public interface IAIState
{
    public AIStates StateName { get; }

    public AIStateManager AIStateManager { get; }

    void InitState(AIStateManager stateManager);

    void UpdateState();
    

    void EnterState();

    
    void ExitState();
}
