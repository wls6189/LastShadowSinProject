using UnityEngine;

public class SpiritCleave3State : IState
{
    PlayerController player;
    float frame = 50;

    public SpiritCleave3State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritCleave3;
        player.PlayerAnimator.SetTrigger("DoSpiritCleave3");
        player.IsAttacking = true;
        player.CanSpiritCleave3Combo = false;
        player.PlayerStats.CurrentSpiritWave -= 2; // ��ȥ�� �ĵ� �Ҹ�
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 2f / frame && duration <= 12f / frame) // ù �� ���
        {
            player.AttackMoving(5f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է��ϸ� �Ű������� �ӵ��� ������
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 11f / frame && duration <= 14f / frame)
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
        if (duration >= 47f / frame)
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