using UnityEngine;

public class RadicalESM : EternalSpiritMark
{
    public RadicalESM() : base(
        "�ش����� ������ ��ȥ����",
        "�� �� 15�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� �����ڼ� �� �����ڼ��� ��ȯ�մϴ�." +
        "\r\n���� �ڼ� : �޴� ���ذ� 50% ���������� ����� ���õ� ��� �ൿ�� �Ұ��������ϴ�." +
        "\r\n���� �ڼ� : �и� �� ���ݷ��� 20%��ŭ �������� ������ ���ݰ� ���õ� ��� �ൿ�� �Ұ��������ϴ�." +
        "\r\n���� �ڼ� ��ȯ �� �ֺ� ������ ���ݷ��� 200%�� �������� �ݴϴ�.",
        15,
        true,
        0
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.FireRadicalESMProjectile();

            if (player.IsRadicalESMAttackPosture)
            {
                player.IsRadicalESMAttackPosture = false;
                player.PlayerStats.DamageReducePercentage = 0.5f;
            }
            else
            {
                player.IsRadicalESMAttackPosture = true;
                player.PlayerStats.DamageReducePercentage = 0;

                if (player.CurrentPlayerState == PlayerState.Guard)
                {
                    player.CurrentPlayerState = PlayerState.IdleAndMove;
                }
            }
        }
    }
}
