
public interface IAIState
{
    public AIStates StateName { get; }

    public AIStateManager AIStateManager { get; }

    void InitState(AIStateManager stateManager);
    void EnterState();

    void UpdateState();
    
    void ExitState();
}
