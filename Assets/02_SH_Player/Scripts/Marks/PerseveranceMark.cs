using UnityEngine;

public class PerseveranceMark : Mark
{
    public PerseveranceMark() : base(
        "�ټ�",
        "���ݷ��� 5 / 15 �����մϴ�."
        ){}

    public override void Effect(PlayerController player, int count) // Awake���� ȣ�� ��
    {
        if (count >= 4)
        {
            player.PlayerStats.AttackPower += 15;
        }
        else if (count >= 2)
        {
            player.PlayerStats.AttackPower += 5;
        }
    }
}
