using UnityEngine;

namespace PlayerPart
{
    public class HitLongGroggyState : IState
    {
        PlayerController player;
        float frame = 55f;

        public HitLongGroggyState(PlayerController player)
        {
            this.player = player;
        }
        public void Enter()
        {
            player.CurrentPlayerState = PlayerState.HitLongGroggy;
            player.PlayerAnimator.SetTrigger("DoHitLongGroggy");
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

            float duration = player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

            if (duration >= 0f && duration <= 15f / frame)
            {
                player.AttackMoving(-7f);
            }

            if (duration >= 40f / frame)
            {
                player.IsGrogging = false;
            }

            if (duration >= 49f / frame)
            {
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
