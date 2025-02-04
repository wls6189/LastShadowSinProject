using UnityEngine;

namespace PlayerPart
{
    public class HitShortGroggyState  : IState
    {
        PlayerController player;
        float frame = 32f;

        public HitShortGroggyState(PlayerController player)
        {
            this.player = player;
        }
        public void Enter()
        {
            player.CurrentPlayerState = PlayerState.HitShortGroggy;
            player.PlayerAnimator.SetTrigger("DoHitShortGroggy");
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

            if (duration >= 0f && duration < 15f / frame)
            {
                player.AttackMoving(-6f);
            }

            if (duration >= 28f / frame)
            {
                player.IsGrogging = false;

                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
            }
        }

        public void Exit()
        {
            player.IsGrogging = false;
        }
    }
}
