using UnityEngine;

public class SteadfastESM : EternalSpiritMark
{
    float parryDuration = 0;
    public SteadfastESM() : base(
        "�ε��� ������ ��ȥ����",
        "�� �� 5�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 5�� �� ���⵵ �и��� ȿ���� �����ϴ�.",
        5,
        true
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsSteadfast = true;
            parryDuration = 5f;
        }

        if (parryDuration > 0)
        {
            parryDuration -= Time.deltaTime;
        }

        if (parryDuration < 0)
        {
            player.PlayerStats.IsSteadfast = false;
            parryDuration = 0;
        }
    }
}
