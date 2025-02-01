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

    public virtual void Ability(PlayerController player) // Update 호출 요
    {

    }

    public override bool Equals(object obj) // 이름이 같으면 같다고 해버리기
    {
        if (obj is EternalSpiritMark other)
        {
            return Name == other.Name;
        }

        return false;
    }
    public override int GetHashCode() // 이름으로 해쉬코드 반환하기
    {
        return Name.GetHashCode();
    }
}
