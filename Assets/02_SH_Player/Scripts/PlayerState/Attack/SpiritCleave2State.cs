using UnityEngine;

public class SpiritCleave2State : IState
{
    PlayerController player;
    float frame = 45;

    public SpiritCleave2State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritCleave2;
        player.PlayerAnimator.SetTrigger("DoSpiritCleave2");
        player.IsAttacking = true;
        player.CanSpiritCleave2Combo = false;
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
        if (duration >= 9f / frame && duration <= 13f / frame) // ù �� ���
        {
            player.AttackMoving(5f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է��ϸ� �Ű������� �ӵ��� ������
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 10f / frame && duration <= 13f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����
        if (duration >= 22f / frame)
        {
            player.IsAttacking = false;
            player.CanSpiritCleave3Combo = true;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 41f / frame)
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