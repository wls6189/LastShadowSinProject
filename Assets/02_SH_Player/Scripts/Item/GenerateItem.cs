using System.Collections.Generic;
using UnityEngine;

public class GenerateItem
{
    public static DropItemData GeneratingItem(int ash, float faint, int maxFaint, float vivid, int maxVivid, float azure, int maxAzure, float eSM) // �Ű������� �� �������� Ȯ���� �ǹ��Ѵ�.
    {
        float randomValue = Random.value; // 0.0���� 1.0������ ���ڸ� ��ȯ
        EternalSpiritMark dropESM = null;
        if (randomValue == 0)
        {
            // Ȯ���� 0�� ��� �ƹ��͵� ���� �ʴ´�.
        }
        else if (eSM >= randomValue)
        {
            dropESM = MarkAndESMList.ChooseESM();
        }

        List<SpiritMark> dropSpiritMarks = new();
        for (int i = 0; i < maxFaint; i++)
        {
            randomValue = Random.value;
            if (randomValue == 0)
            {
                // Ȯ���� 0�� ��� �ƹ��͵� ���� �ʴ´�.
            }
            else if (faint >= randomValue)
            {
                dropSpiritMarks.Add(new SpiritMark(TypeOfSpiritMark.Faint));
            }
        }
        for (int i = 0; i < maxVivid; i++)
        {
            randomValue = Random.value;
            if (randomValue == 0)
            {
                // Ȯ���� 0�� ��� �ƹ��͵� ���� �ʴ´�.
            }
            else if (vivid >= randomValue)
            {
                dropSpiritMarks.Add(new SpiritMark(TypeOfSpiritMark.Vivid));
            }
        }
        for (int i = 0; i < maxAzure; i++)
        {
            randomValue = Random.value;
            if (randomValue == 0)
            {
                // Ȯ���� 0�� ��� �ƹ��͵� ���� �ʴ´�.
            }
            else if (azure >= randomValue)
            {
                dropSpiritMarks.Add(new SpiritMark(TypeOfSpiritMark.Azure));
            }
        }

        return new DropItemData(ash, dropESM, dropSpiritMarks);
    }
}
public struct DropItemData
{
    public int SpiritAsh;
    public EternalSpiritMark EternalSpiritMark;
    public List<SpiritMark> SpiritMarks;

    public DropItemData(int spiritAsh, EternalSpiritMark eternalSpiritMark, List<SpiritMark> spiritMarks)
    {
        SpiritAsh = spiritAsh;
        EternalSpiritMark = eternalSpiritMark;
        SpiritMarks = spiritMarks;
    }
}