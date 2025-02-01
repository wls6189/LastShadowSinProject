using System.Collections;
using UnityEngine;

public class GrabbedState : IState
{
    PlayerController player;

    public GrabbedState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Grabbed;
        player.PlayerAnimator.SetTrigger("DoGrabbed");
        player.IsGrogging = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        player.IsGrogging = false;
    }
}