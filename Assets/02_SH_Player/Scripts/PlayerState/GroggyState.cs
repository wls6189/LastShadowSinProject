using UnityEngine;

namespace PlayerPart
{
    public class GroggyState : IState
    {
        PlayerController player;

        public GroggyState(PlayerController player)
        {
            this.player = player;
        }
        public void Enter()
        {
            player.CurrentPlayerState = PlayerState.Groggy;
        }

        public void Execute()
        {
            if (player.Animator.IsInTransition(0))
            {
                return;
            }

            float duration = player.StateInfo.normalizedTime % 1f;

            if (duration > 0.8f)
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

        }
    }
}
