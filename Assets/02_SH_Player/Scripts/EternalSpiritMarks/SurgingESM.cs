using UnityEngine;

public class SurgingESM : EternalSpiritMark
{
    public SurgingESM() : base(
        "�ڱ�ġ�� ������ ��ȥ����",
        "��� ������ ������ ������ 2�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� 25%, 50%, 100%�� á�� �� ��ȥ���� �ɷ��� ����� �� �ֽ��ϴ�. ��ȥ���� �ɷ� ��� �� �����ִ� �������� ���� ������ �����մϴ�. ��ȥ���η��� ���� �� �����Ҽ��� ���� ���� ������ �����մϴ�.",
        2,
        false,
        0
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce)
            {
                player.PlayerStats.CurrentSpiritMarkForce -= 100;
                player.FireSpiritUnboundProjectile(100);
            }
            else if (player.PlayerStats.CurrentSpiritMarkForce >= 50)
            {
                player.PlayerStats.CurrentSpiritMarkForce -= 50;
                player.FireSpiritUnboundProjectile(50);
            }
            else if (player.PlayerStats.CurrentSpiritMarkForce >= 25)
            {
                player.PlayerStats.CurrentSpiritMarkForce -= 25;
                player.FireSpiritUnboundProjectile(25);
            }
        }
    }
}
