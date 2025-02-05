using UnityEngine;

public class DashState : IState
{
    PlayerController player;
    float frame = 20f;
    bool moveRight;

    public DashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Dash;
        player.IsDoSomething = true;
        player.PlayerStats.CurrentSpiritWave -= 0.5f; // ��ȥ�� �ĵ� �Ҹ�

        if (player.IsLockOn)
        {
            if (player.IsLookRight) // �������� ���� ���� ��, �� ���Ͱ� �����ʿ� ���� ��
            {
                if (player.MoveActionValue > 0) // ������ ����Ű �Է� ��
                {
                    player.PlayerAnimator.SetTrigger("DoDashForward");
                    moveRight = true;
                }
                else if (player.MoveActionValue < 0) // ���� ����Ű �Է� ��
                {
                    player.PlayerAnimator.SetTrigger("DoDashBackward");
                    moveRight = false;
                }
                else // ����Ű �Է� ���� �뽬 ��
                {
                    player.PlayerAnimator.SetTrigger("DoDashBackward");
                    moveRight = false;
                }
            }
            else
            {
                if (player.MoveActionValue > 0) // ������ ����Ű �Է� ��
                {
                    player.PlayerAnimator.SetTrigger("DoDashBackward");
                    moveRight = true;
                }
                else if (player.MoveActionValue < 0) // ���� ����Ű �Է� ��
                {
                    player.PlayerAnimator.SetTrigger("DoDashForward");
                    moveRight = false;
                }
                else // ����Ű �Է� ���� �뽬 ��
                {
                    player.PlayerAnimator.SetTrigger("DoDashBackward");
                    moveRight = true;
                }
            }
        }
        else
        {
            if (player.MoveActionValue > 0) // ������ ����Ű �Է� ��
            {
                player.PlayerAnimator.SetTrigger("DoDashForward");
                moveRight = true;
            }
            else if (player.MoveActionValue < 0) // ���� ����Ű �Է� ��
            {
                player.PlayerAnimator.SetTrigger("DoDashForward");
                moveRight = false;
            }
            else // ����Ű �Է� ���� �뽬 ��
            {
                if (player.IsLookRight)
                {
                    player.PlayerAnimator.SetTrigger("DoDashForward");
                    moveRight = true;
                }
                else
                {
                    player.PlayerAnimator.SetTrigger("DoDashForward");
                    moveRight = false;
                }
            }
        }
    }
    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        if (duration >= 6f / frame && duration <= 14f / frame)
        {
            player.OnDashEffect = true;

            if (moveRight)
            {
                Vector3 moveVector = new Vector3(0, 0, 1);
                player.PlayerCharacterController.Move(moveVector * player.DashSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 moveVector = new Vector3(0, 0, -1);
                player.PlayerCharacterController.Move(moveVector * player.DashSpeed * Time.deltaTime);
            }
        }
        else if (duration >= 17f / frame)
        {
            player.IsDoSomething = false;
            player.OnDashEffect = false;
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsDoSomething = false;
        player.OnDashEffect = false;
    }
}