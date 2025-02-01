using PlayerPart;
using UnityEngine;

public class PlayerStateMachine
{
    public IState CurrentState { get; private set; }
    PlayerController player;

    public IdleAndMoveState idleAndMoveState;
    public UseChaliceOfAtonementState useChaliceOfAtonementState;
    public DashState dashState;

    public SpiritParryState spiritParryState;
    public PenetrateState penetrateState;
    public ParryState parryState;
    public GuardHitState guardHitState;
    public HitShortGroggyState hitShortGroggyState;
    public HitLongGroggyState hitLongGroggyState;
    public GrabbedState grabbedState;
    public ClashGuardState clashGuardState;
    public GuardState guardState;

    public BasicHorizonSlash1State basicHorizonSlash1State;
    public BasicHorizonSlash2State basicHorizonSlash2State;
    public BasicVerticalSlashState basicVerticalSlashState;
    public ThrustState thrustState;
    public RetreatSlashState retreatSlashState;
    public SpiritCleave1State spiritCleave1State;
    public SpiritCleave2State spiritCleave2State;
    public SpiritCleave3State spiritCleave3State;
    public SpiritPiercingState spiritPiercingState;
    public SpiritSwordDanceState spiritSwordDanceState;

    public PlayerStateMachine(PlayerController player)
    {
        this.player = player;

        idleAndMoveState = new(player);
        useChaliceOfAtonementState = new(player);
        dashState = new(player);

        spiritParryState = new(player);
        penetrateState = new(player);
        parryState = new(player);
        guardHitState = new(player);
        hitShortGroggyState = new(player);
        hitLongGroggyState = new(player);
        grabbedState = new(player);
        clashGuardState = new(player);
        guardState = new(player);

        basicHorizonSlash1State = new(player);
        basicHorizonSlash2State = new(player);
        basicVerticalSlashState = new(player);
        thrustState = new(player);
        retreatSlashState = new(player);
        spiritCleave1State = new(player);
        spiritCleave2State = new(player);
        spiritCleave3State = new(player);
        spiritPiercingState = new(player);
        spiritSwordDanceState = new(player);
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