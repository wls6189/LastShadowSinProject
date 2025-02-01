using System;
using UnityEngine;

public class EternalSpiritMark
{
    public string Name { get; }
    public string GainDescription { get; }
    public string AbilityDescription { get; }
    public float Gain { get; }
    public bool IsNaturalRegeneration { get; }

    protected EternalSpiritMark(string name, string gainDescription, string abilityDescription, float gain, bool isNaturalRegeneration)
    {
        Name = name;
        GainDescription = gainDescription;
        AbilityDescription = abilityDescription;
        Gain = gain;
        IsNaturalRegeneration = isNaturalRegeneration;
    }

    public virtual void Ability(PlayerController player) // Update ȣ�� ��
    {

    }

    public override bool Equals(object obj) // �̸��� ������ ���ٰ� �ع�����
    {
        if (obj is EternalSpiritMark other)
        {
            return Name == other.Name;
        }

        return false;
    }
    public override int GetHashCode() // �̸����� �ؽ��ڵ� ��ȯ�ϱ�
    {
        return Name.GetHashCode();
    }
}
