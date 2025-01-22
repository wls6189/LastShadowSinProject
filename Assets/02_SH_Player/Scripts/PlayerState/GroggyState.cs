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
            player.IsGuarding = false;
            player.IsParring = false;
        }

        public void Execute()
        {
            if (player.Animator.IsInTransition(0))
            {
                return;
            }

            float duration = player.StateInfo.normalizedTime % 1f;

            if (player.StateInfo.IsName("Parry"))
            {
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
            else if (player.StateInfo.IsName("GuardHit"))
            {
                if (duration > 0.9f)
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
            else if (player.StateInfo.IsName("HitShortGroggy"))
            {

            }
            else if (player.StateInfo.IsName("HitLongGroggy"))
            {

            }
        }

        public void Exit()
        {

        }
    }
}
