using UnityEngine;

public class RetreatSlashState : IState
{
    PlayerController player;
    float frame = 35;

    public RetreatSlashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.RetreatSlash;
        player.PlayerAnimator.SetTrigger("DoRetreatSlash");
        player.IsAttacking = true;
        player.PlayerStats.CurrentSpiritWave -= 1; // ��ȥ�� �ĵ� �Ҹ�
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� �̵� ����
        if (duration >= 9f / frame && duration <= 14f / frame)
        {
            player.AttackMoving(-16f);
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 6f / frame && duration <= 11f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����)
        if (duration >= 20f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 32f / frame)
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