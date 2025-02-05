using UnityEngine;

public class DestructiveESM : EternalSpiritMark
{
    float increaseAmount = 0;
    public DestructiveESM() : base(
        "�ı����� ������ ��ȥ����",
        "��� ������ ������ ������ 10�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� �� �������� ���ݷ��� 300%�� ���ظ� �ִ� �ҿ뵹�̸� �߻��մϴ�.",
        10,
        false,
        0f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.FireDestructiveESMProjectile();
        }
    }
}