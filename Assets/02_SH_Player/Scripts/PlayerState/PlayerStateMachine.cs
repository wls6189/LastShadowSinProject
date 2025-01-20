using UnityEngine;

public class PlayerStateMachine
{
    public IState CurrentState { get; private set; }
    PlayerController player;

    public IdleAndMoveState idleAndMoveState;
    public AirborneState airborneState;
    public DashState dashState;
    public BasicHorizonSlash1State basicHorizonSlash1State;
    public BasicHorizonSlash2State basicHorizonSlash2State;
    public BasicVerticalSlashState basicVerticalSlashState;
    public GuardState guardState;
    public ThrustState thrustState;

    public PlayerStateMachine(PlayerController player)
    {
        this.player = player;
        idleAndMoveState = new(player);
        airborneState = new(player);
        dashState = new(player);
        basicHorizonSlash1State = new(player);
        basicHorizonSlash2State = new(player);
        basicVerticalSlashState = new(player);
        guardState = new(player);
        thrustState = new(player);
    }

    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();
    }

    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
    public void Execute()
    {
        CurrentState.Execute();
    }
}