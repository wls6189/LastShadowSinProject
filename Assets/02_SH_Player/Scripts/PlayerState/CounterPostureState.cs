using UnityEngine;

public class CounterPostureState : IState
{
    PlayerController player;

    public CounterPostureState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.CounterPosture;
        player.Animator.SetTrigger("DoCounterPosture");
        player.IsParrySucceed = false;
        player.IsCounterPosture = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        player.IsCounterPosture = false;
    }
}