
public interface IAIState
{
    public string StateName { get; }

    public AIStateManager AIStateManager { get; }

    void InitState(AIStateManager stateManager);

    void UpdateState();
    

    void EnterState();

    
    void ExitState();
}
