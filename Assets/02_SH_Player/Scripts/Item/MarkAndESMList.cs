using System.Collections.Generic;
using System.Linq;
using System;

public class MarkAndESMList
{
    ProtectiveESM protectiveESM = new ProtectiveESM();
    SteadfastESM SteadfastESM = new SteadfastESM();

    public static EternalSpiritMark ChooseESM()
    {
        MarkAndESMList instance = new MarkAndESMList();

        List<EternalSpiritMark> eSMs = new()
        {
            instance.protectiveESM 
        };

        Random random = new Random();

        return eSMs.OrderBy(x => random.Next()).First();
    }





    PerseveranceMark perseveranceMark = new PerseveranceMark();

    public static List<Mark> ChooseMarks(int count)
    {
        MarkAndESMList instance = new MarkAndESMList();

        List<Mark> chosenMarks = new();

        List<Mark> marks = new()
        {
            instance.perseveranceMark
        };

        Random random = new Random();
        chosenMarks = marks.OrderBy(x => random.Next()).Take(count).ToList();

        return chosenMarks;
    }
}
