using UnityEngine;

namespace PlayerPart
{
    public class PenetrateState : IState
    {
        PlayerController player;
        float frame = 25;

        public PenetrateState(PlayerController player)
        {
            this.player = player;
        }
        public void Enter()
        {
            player.CurrentPlayerState = PlayerState.Penetrate;
            player.PlayerAnimator.SetTrigger("DoPenetrate");
            player.IsGrogging = true;
            player.CanPenetrate = false;
        }

        public void Execute()
        {
            if (player.PlayerAnimator.IsInTransition(0))
            {
                return;
            }

            float duration = player.StateInfo.normalizedTime % 1f;

            if (duration >= 3f / frame && duration <= 6f / frame)
            {
                player.AttackMoving(5f);
            }

            if (duration > 22f / frame)
            {
                player.IsGrogging = false;

                if (player.guardAction.IsPressed())
                {
                    player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardState);
                }
                else
                {
                    player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
                }
            }

        }

        public void Exit()
        {
            player.IsGrogging = false;
        }
    }
}
