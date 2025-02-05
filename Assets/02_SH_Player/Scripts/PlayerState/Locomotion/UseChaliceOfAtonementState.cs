using UnityEngine;


public class UseChaliceOfAtonementState : IState
{
    PlayerController player;
    float frame = 40f;
    int count;

    public UseChaliceOfAtonementState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.UseChaliceOfAtonement;
        player.PlayerAnimator.SetTrigger("DoUseChaliceOfAtonement");
        player.IsDoSomething = true;
        count = player.PlayerChaliceOfAtonement.CurrentChaliceOfAtonementCount;
        AudioManager.instance.Playsfx(AudioManager.Sfx.Heal);
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        if (duration >= 12f / frame && duration <= 14f / frame)
        {
            if (count == player.PlayerChaliceOfAtonement.CurrentChaliceOfAtonementCount)
            {
                player.PlayerChaliceOfAtonement.UseChaliceOfAtonement();
            }
        }

        if (duration >= 23f / frame)
        {
            player.IsDoSomething = false;
        }

        if (duration >= 37f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsDoSomething = false;
    }
}

