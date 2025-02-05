using System.Collections.Generic;
using System.Linq;
using System;

public class MarkAndESMList
{
    ProtectiveESM protectiveESM = new ProtectiveESM();
    SteadfastESM steadfastESM = new SteadfastESM();
    RavenousESM ravenousESM = new RavenousESM();
    EnthusiasticESM enthusiasticESM = new EnthusiasticESM();
    DestructiveESM destructiveESM = new DestructiveESM();
    RadicalESM radicalESM = new RadicalESM();
    RagingESM ragingESM = new RagingESM();
    LiberatedESM liberatedESM = new LiberatedESM();
    SurgingESM surgingESM = new SurgingESM();
    RampagingESM rampagingESM = new RampagingESM();

    public static EternalSpiritMark ChooseESM()
    {
        MarkAndESMList instance = new MarkAndESMList();

        List<EternalSpiritMark> eSMs = new()
        {
            instance.protectiveESM,
            instance.steadfastESM,
            instance.ravenousESM,
            instance.enthusiasticESM,
            instance.destructiveESM,
            instance.radicalESM,
            instance.ragingESM,
            instance.liberatedESM,
            instance.surgingESM,
            instance.rampagingESM
        };

        Random random = new Random();

        return eSMs.OrderBy(x => random.Next()).First();
    }





    PerseveranceMark perseveranceMark = new PerseveranceMark();
    CurseWagesMark curseWagesMark = new CurseWagesMark();
    DeepWoundMark deepWoundMark = new DeepWoundMark();
    FocusMark focusMark = new FocusMark();
    public static List<Mark> ChooseMarks(int count)
    {
        MarkAndESMList instance = new MarkAndESMList();

        List<Mark> chosenMarks = new();

        List<Mark> marks = new()
        {
            instance.perseveranceMark,
            instance.curseWagesMark,
            instance.deepWoundMark,
            instance.focusMark,
        };

        Random random = new Random();
        chosenMarks = marks.OrderBy(x => random.Next()).Take(count).ToList();

        return chosenMarks;
    }
}
