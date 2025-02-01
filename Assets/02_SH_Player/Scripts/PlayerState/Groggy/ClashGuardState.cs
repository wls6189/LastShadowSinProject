using System.Collections;
using UnityEngine;

public class ClashGuardState : IState
{
    PlayerController player;

    public ClashGuardState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.ClashGuard;
        player.PlayerAnimator.SetTrigger("DoClashGuard");
        player.IsGrogging = true;
    }

    public void Execute()
    {
        if (player.specialAttackAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentHealth += 3f;
        }

        player.PlayerStats.CurrentHealth -= 10f * Time.deltaTime;
    }

    public void Exit()
    {
        player.IsGrogging = false;
    }
}