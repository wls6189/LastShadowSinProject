using UnityEngine;

public class RavenousESM : EternalSpiritMark
{
    public RavenousESM() : base(
        "���ָ� ������ ��ȥ����",
        "��� ������ ������ ������ 8�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 6�� �� ���� ���� �� ���ݷ¸�ŭ ü���� ȸ���մϴ�.",
        8,
        false,
        6f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsRavenous = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.IsRavenous = false;
            RemainDuration = 0;
        }
    }
}
