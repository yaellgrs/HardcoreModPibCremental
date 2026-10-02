using BepInEx;
using HarmonyLib;

[BepInPlugin("nisshoku.pibcremental.hardcore", "Hardcore", "1.0.0")]
public class HardcoreMod : BaseUnityPlugin
{
    public static double CostMultiplier = 10.0;

    public void Awake()
    {
        Logger.LogInfo("Mod chargé !");
        var harmony = new Harmony("nisshoku.pibcremental.hardcore");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(UpgradeData), nameof(UpgradeData.GetScaledCost))]
    public class UpgradeCostPatch
    {
        static void Postfix(UpgradeData __instance, int n, ref double __result)
        {
            double escalation = 1.0 + (n * 0.25);
            __result *= HardcoreMod.CostMultiplier * escalation;
        }
    }

    [HarmonyPatch(typeof(UpgradeData), nameof(UpgradeData.GetScaledEffectValue))]
    public class UpgradeEffectPatch
    {
        static void Postfix(UpgradeData __instance, int n, ref double __result)
        {
            __result *= 0.5;
        }
    }
}