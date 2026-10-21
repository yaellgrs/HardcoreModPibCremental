using BepInEx;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Reflection;

[BepInPlugin("nisshoku.pibcremental.pibrevolution", "Hardcore", "1.0.0")]
public class PibRevolution : BaseUnityPlugin
{
    public static double CostMultiplier = 3.0;
    public static double PopularityCostMultiplier = 25.0;


    public void Awake()
    {
        Logger.LogInfo("Mod chargé !");
        var harmony = new Harmony("nisshoku.pibcremental.pibrevolution");

        harmony.PatchAll();
    }

    public static double GetPriceMultiplier(int n)
    {
        return (1.0 + (n * 0.5)) + (PopularityCostMultiplier * ( 1.0f - RevoltManager.ChefPopularity));
    }


    //augmenter le prix
    [HarmonyPatch(typeof(UpgradeData), nameof(UpgradeData.GetScaledCost))]
    public class UpgradeCostPatch
    {
        static void Postfix(UpgradeData __instance, int n, ref double __result)
        {
            if(__instance.GetDisplayName(n) != "Assign Pib")
            {
                __result *= PibRevolution.CostMultiplier * PibRevolution.GetPriceMultiplier(n);
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
            __result *= PibRevolution.CostMultiplier * PibRevolution.GetPriceMultiplier(index);
        }
    }


    [HarmonyPatch(typeof(BuildingManager), "Awake")]
    public class InitPopularityBar
    {
        static void Postfix()
        {
            RevoltManager.CreatePopularityBarre();

        }
    }

    [HarmonyPatch(typeof(CentralUpdateManager), "Update")]
    public class InputManager
    {
        static void Postfix()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                Debug.Log("A");
                RevoltManager.UpPopularity(-0.1f);
            }
            else if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("E");
                RevoltManager.UpPopularity(0.1f);
            }
            else if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                CS_PrestigeManager.Instance.AddPrestigeXpPercent(100);
            }
        }
    }

    [HarmonyPatch(typeof(CS_PapaPibble), "Awake")]
    public class AddHoverGlowPatchPapaPibble
    {
        static void Postfix(CS_PapaPibble __instance)
        {


            Vector2 size = new Vector2(200, 30);
            Vector2 anchoredPos = Vector2.zero;
            Vector3 localOffset = new Vector3(0, 1.5f, 0);
            UI_Bar bar = new UI_Bar("Papa Piblle", RenderMode.WorldSpace, size, anchoredPos, Color.red, scale: 0.01f, parent: __instance.gameObject.transform, localOffset: localOffset, nameLabelHight: 40);
            PibGlowOnHover glow = __instance.gameObject.AddComponent<PibGlowOnHover>();
            glow.Initialize(bar);
        }

    }



}