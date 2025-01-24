using UnityEngine;

namespace PlayerPart
{
    public class GuardHitState : IState
    {
        PlayerController player;
        float frame = 13f;

        public GuardHitState(PlayerController player)
        {
            this.player = player;
        }
        public void Enter()
        {
            player.CurrentPlayerState = PlayerState.GuardHit;
            player.PlayerAnimator.SetTrigger("DoGuardHit");
            player.IsGrogging = true;
            player.IsGuarding = false;
            player.IsParring = false;
        }

        public void Execute()
        {
            if (player.PlayerAnimator.IsInTransition(0))
            {
                return;
            }

            float duration = player.StateInfo.normalizedTime % 1f;

            if (duration >= 0f && duration <= 6f / frame)
            {
                player.AttackMoving(-6f);
            }

            if (duration >= 10f / frame)
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
