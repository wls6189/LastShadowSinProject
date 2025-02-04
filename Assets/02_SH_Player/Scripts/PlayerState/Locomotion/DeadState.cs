using UnityEngine;

public class DeadState : IState
{
    PlayerController player;

    public DeadState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Dead;
        player.PlayerAnimator.SetTrigger("DoDead");
    }
    public void Execute()
    {

    }

    public void Exit()
    {

    }
}