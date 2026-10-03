using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

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

    public static double GetPriceMultiplier(int n)
    {
        return 1.0 + (n * 0.5);
    }


    //augmenter le prix
    [HarmonyPatch(typeof(UpgradeData), nameof(UpgradeData.GetScaledCost))]
    public class UpgradeCostPatch
    {
        static void Postfix(UpgradeData __instance, int n, ref double __result)
        {
            if(__instance.GetDisplayName(n) != "Assign Pib")
            {
                __result *= HardcoreMod.CostMultiplier * HardcoreMod.GetPriceMultiplier(n);
            }

        }
    }

    //réduire les effets d'amélioreration
    [HarmonyPatch(typeof(UpgradeData), nameof(UpgradeData.GetScaledEffectValue))]
    public class UpgradeEffectPatch
    {
        static void Postfix(UpgradeData __instance, int n, ref double __result)
        {
            __result *= 0.5;
        }
    }

    //retirer le click
    [HarmonyPatch(typeof(CS_ClickRateManager), nameof(CS_ClickRateManager.TryConsumeClick))]
    public class DisableClickPatch
    {
        static bool Prefix(ref bool __result)
        {
            __result = false; 
            return false;     
        }
    }

    //premier pib gratuit
    //les autres aux prix augmenté
    [HarmonyPatch(typeof(PibRockExtension), "GetSapCostAt")]
    public class FixPibbleSapCost
    {
        static void Postfix(int index, ref double __result)
        {
            //if (index == 0) return 0.0;
            __result *= HardcoreMod.CostMultiplier * HardcoreMod.GetPriceMultiplier(index);
        }
    }

    [HarmonyPatch(typeof(CS_PapaPibble), "Awake")]
    public class PibRockHeatlBar
    {
        static void Postfix(CS_PapaPibble __instance)
        {
            HealthBar.CreateHealthBar(__instance.transform, "PapaPib");

        }
    }
}