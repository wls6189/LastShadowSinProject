using UnityEngine;

public class BasicVerticalSlashState : IState
{
    PlayerController player;
    float frame = 40;

    public BasicVerticalSlashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicVerticalSlash;
        player.Animator.SetTrigger("DoBasicVerticalSlash");
        player.IsAttacking = true;
    }

    public void Execute()
    {
        if (player.Animator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 8f / frame && duration <= 13f / frame) // ù �� ���
        {
            player.AttackMoving(3f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է�.
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 9f / frame && duration <= 13f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����
        if (duration >= 26f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 37f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
        player.IsAttackColliderEnabled = false;
    }
}