using UnityEngine;

public class BasicHorizonSlash1State : IState
{
    PlayerController player;
    float frame = 35;

    public BasicHorizonSlash1State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash1;
        player.PlayerAnimator.SetTrigger("DoBasicHorizonSlash1");
        player.IsAttacking = true;
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 6f / frame && duration <= 10f / frame) // ù �� ���
        {
            player.AttackMoving(5f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է��ϸ� �Ű������� �ӵ��� ������
        }
        else if (duration >= 18f / frame && duration <= 32f / frame) // ���ƿ��� �� ���
        {
            player.AttackMoving(2f);
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 7f / frame && duration <= 10f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����) + (�޺� ���� ��� ����)
        if (duration >= 20f / frame)
        {
            player.IsAttacking = false;
            player.CanBasicHorizonSlash2Combo = true;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 32f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        // ������ ����� ���� �ʱ�ȭ
        player.IsAttacking = false;
        player.IsAttackColliderEnabled = false;
    }
}