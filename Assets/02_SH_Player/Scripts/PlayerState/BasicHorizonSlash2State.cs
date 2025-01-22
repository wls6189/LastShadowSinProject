using UnityEngine;

public class BasicHorizonSlash2State : IState
{
    PlayerController player;
    float frame = 37;

    public BasicHorizonSlash2State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash2;
        player.Animator.SetTrigger("DoBasicHorizonSlash2");
        player.IsAttacking = true;
        player.CanBasicHorizonSlashCombo = false;
    }

    public void Execute()
    {
        if (player.Animator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 6f / frame && duration <= 11f / frame)
        {
            player.AttackMoving(4f); 
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 7f / frame && duration <= 12f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����)
        if (duration >= 22f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 34f / frame)
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