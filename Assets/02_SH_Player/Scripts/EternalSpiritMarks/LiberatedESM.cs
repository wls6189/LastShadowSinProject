using UnityEngine;

public class LiberatedESM : EternalSpiritMark
{
    public LiberatedESM() : base(
        "�ع�� ������ ��ȥ����",
        "��� ������ ������ ������ 5�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 6�� �� ��ȥ �ع� ���°� �˴ϴ�. ��ȥ �ع� ���¿����� �⺻ �������(X)�� �����ϸ� ���� �ӵ��� 1.5�� ����մϴ�.",
        5,
        false,
        6f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerAnimator.SetFloat("AttackSpeed", 1.5f);
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerAnimator.SetFloat("AttackSpeed", 1f);
            RemainDuration = 0;
        }
    }
}
