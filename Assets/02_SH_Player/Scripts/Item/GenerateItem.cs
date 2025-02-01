using System.Collections.Generic;
using UnityEngine;

public class GenerateItem
{
    public static DropItemData GeneratingItem(int ash, float faint, int maxFaint, float vivid, int maxVivid, float azure, int maxAzure, float eSM) // 매개변수는 각 아이템의 확률을 의미한다.
    {
        float randomValue = Random.value; // 0.0에서 1.0까지의 숫자를 반환
        EternalSpiritMark dropESM = null;
        if (randomValue == 0)
        {
            // 확률이 0일 경우 아무것도 하지 않는다.
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
                // 확률이 0일 경우 아무것도 하지 않는다.
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
                // 확률이 0일 경우 아무것도 하지 않는다.
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
                // 확률이 0일 경우 아무것도 하지 않는다.
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