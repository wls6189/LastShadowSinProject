using UnityEngine;

public class ThrustState : IState
{
    PlayerController player;
    float frame = 35;

    public ThrustState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Thrust;
        player.PlayerAnimator.SetTrigger("DoThrust");
        player.IsAttacking = true;
        player.PlayerStats.Tenacity = TenacityAndGroggyForce.High;
        //player.PlayerStats.CurrentSpiritWave -= 1; // ��ȥ�� �ĵ� �Ҹ�
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 1f / frame && duration <= 5f / frame)
        {
            player.AttackMoving(16f);
        }

        // �и� ���� �� ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 1f / frame && duration <= 5f / frame)
        {
            player.IsAttackingParryColliderEnabled = true;
        }
        else
        {
            player.IsAttackingParryColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����)
        if (duration >= 20f / frame)
        {
            player.PlayerStats.Tenacity = TenacityAndGroggyForce.Medium;
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
        player.IsAttackingParryColliderEnabled = false;
        player.PlayerStats.Tenacity = TenacityAndGroggyForce.Medium;

    }
}