using UnityEngine;

public class SpiritNovaState : IState
{
    PlayerController player;
    float frame = 30;

    public SpiritNovaState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritNova;
        player.PlayerAnimator.SetTrigger("DoSpiritNova");
        player.IsAttacking = true;
        player.CanSpiritNova = false;
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
        if (duration >= 8f / frame && duration <= 15f / frame) // ù �� ���
        {
            player.AttackMoving(20f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է��ϸ� �Ű������� �ӵ��� ������
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 10f / frame && duration <= 14f / frame)
        {
            player.OnDashEffect = true;
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.OnDashEffect = false;
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����
        if (duration >= 20f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 27f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
        player.IsAttackColliderEnabled = false;
        player.OnDashEffect = false;
    }
}