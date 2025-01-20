using UnityEngine;

public class DashState : IState
{
    PlayerController player;
    bool moveRight;

    public DashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Dash;

        if (player.IsLockOn)
        {
            if (player.IsLookRight) // 오른쪽을 보고 있을 때, 즉 몬스터가 오른쪽에 있을 때
            {
                if (player.MoveActionValue > 0) // 오른쪽 방향키 입력 시
                {
                    player.Animator.SetTrigger("DoDashForward");
                    moveRight = true;
                }
                else if (player.MoveActionValue < 0) // 왼쪽 방향키 입력 시
                {
                    player.Animator.SetTrigger("DoDashBackward");
                    moveRight = false;
                }
                else // 방향키 입력 없이 대쉬 시
                {
                    player.Animator.SetTrigger("DoDashBackward");
                    moveRight = false;
                }
            }
            else
            {
                if (player.MoveActionValue > 0) // 오른쪽 방향키 입력 시
                {
                    player.Animator.SetTrigger("DoDashBackward");
                    moveRight = true;
                }
                else if (player.MoveActionValue < 0) // 왼쪽 방향키 입력 시
                {
                    player.Animator.SetTrigger("DoDashForward");
                    moveRight = false;
                }
                else // 방향키 입력 없이 대쉬 시
                {
                    player.Animator.SetTrigger("DoDashBackward");
                    moveRight = false;
                }
            }
        }
        else
        {
            if (player.MoveActionValue > 0) // 오른쪽 방향키 입력 시
            {
                player.Animator.SetTrigger("DoDashForward");
                moveRight = true;
            }
            else if (player.MoveActionValue < 0) // 왼쪽 방향키 입력 시
            {
                player.Animator.SetTrigger("DoDashForward");
                moveRight = false;
            }
            else // 방향키 입력 없이 대쉬 시
            {
                if (player.IsLookRight)
                {
                    player.Animator.SetTrigger("DoDashForward");
                    moveRight = true;
                }
                else
                {
                    player.Animator.SetTrigger("DoDashForward");
                    moveRight = false;

                }
            }
        }
    }

    public void Execute()
    {
        if (player.Animator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        if (duration >= 0 && duration <= 0.8f)
        {
            if (moveRight)
            {
                Vector3 moveVector = new Vector3(0, 0, 1);
                player.CharacterController.Move(moveVector * player.DashSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 moveVector = new Vector3(0, 0, -1);
                player.CharacterController.Move(moveVector * player.DashSpeed * Time.deltaTime);
            }
        }
        else if (duration > 0.8f)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {

    }
}