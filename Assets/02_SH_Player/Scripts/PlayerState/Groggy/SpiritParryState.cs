using UnityEngine;

namespace PlayerPart
{
    public class SpiritParryState : IState
    {
        PlayerController player;
        float frame = 45;

        public SpiritParryState(PlayerController player)
        {
            this.player = player;
        }
        public void Enter()
        {
            player.CurrentPlayerState = PlayerState.SpiritParry;
            player.PlayerAnimator.SetTrigger("DoSpiritParry");
            player.IsGrogging = true;
            //player.PlayerStats.CurrentSpiritWave -= 1; // 영혼의 파동 소모량
        }

        public void Execute()
        {
            if (player.PlayerAnimator.IsInTransition(0))
            {
                return;
            }

            float duration = player.StateInfo.normalizedTime % 1f;

            if (duration >= 9f / frame && duration <= 16f / frame)
            {
                player.IsSpiritParring = true;
                player.AttackMoving(7f);
            }
            else
            {
                player.IsSpiritParring = false;
            }

            if (duration >= 23f / frame)
            {
                player.IsGrogging = false;
            }

            if (duration >= 42f / frame)
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
            player.IsSpiritParring = false;
            player.IsGrogging = false;
        }
    }
}
