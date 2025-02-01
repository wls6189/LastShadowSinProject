using System.Collections.Generic;
using UnityEngine;

public enum TypeOfSpiritMark
{
    Faint,
    Vivid,
    Azure
}

public class SpiritMark
{
    public TypeOfSpiritMark Type { get; }
    public string Name { get; }
    public int Health { get; }
    public float AttackPower { get; }
    public List<Mark> Marks { get; }

    public SpiritMark(TypeOfSpiritMark type)
    {
        Type = type;

        if (Type == TypeOfSpiritMark.Faint)
        {
            Name = "Èå¸´ÇÑ ¿µÈ¥³«ÀÎ";
            Health = Random.Range(0, 10);
            AttackPower = Random.Range(0, 10);
            Marks = MarkAndESMList.ChooseMarks(2);
        }
        else if (Type == TypeOfSpiritMark.Vivid)
        {
            Name = "¶Ñ·ÇÇÑ ¿µÈ¥³«ÀÎ";
            Health = Random.Range(10, 20);
            AttackPower = Random.Range(10, 20);
            Marks = MarkAndESMList.ChooseMarks(3);
        }
        else if (Type == TypeOfSpiritMark.Azure)
        {
            Name = "Â£Çª¸¥ ¿µÈ¥³«ÀÎ";
            Health = Random.Range(20, 30);
            AttackPower = Random.Range(20, 30);
            Marks = MarkAndESMList.ChooseMarks(3);
        }
    }
}
