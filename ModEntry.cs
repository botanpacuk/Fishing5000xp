using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace FishXP5000;

internal sealed class ModEntry : Mod
{
    private const int FishingSkillId = 1;
    private const int CrabPotExperience = 5;
    private const int FishingBookExperience = 250;
    private const int ExperiencePerCatch = 5000;

    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);
        var method = AccessTools.Method(typeof(Farmer), "gainExperience", new[] { typeof(int), typeof(int) });

        if (method is null)
        {
            this.Monitor.Log("Could not find Farmer.gainExperience(int, int). The mod will not change fishing XP.", LogLevel.Error);
            return;
        }

        harmony.Patch(method, prefix: new HarmonyMethod(typeof(ModEntry), nameof(BeforeGainExperience)));
        this.Monitor.Log("Fish XP 5000 loaded. Fishing catches award 5000 XP.", LogLevel.Info);
    }

    // Vanilla awards 5 XP for crab pots and 250 XP for fishing skill books.
    // Leave those fixed rewards alone; replace ordinary fishing-catch XP.
    private static void BeforeGainExperience(object[] __args)
    {
        if (__args.Length < 2)
            return;

        int skillId = Convert.ToInt32(__args[0]);
        int amount = Convert.ToInt32(__args[1]);

        if (skillId == FishingSkillId && amount != CrabPotExperience && amount != FishingBookExperience)
            __args[1] = ExperiencePerCatch;
    }
}
