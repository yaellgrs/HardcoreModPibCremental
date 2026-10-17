using BepInEx;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[BepInPlugin("nisshoku.pibcremental.pibrevolution", "Hardcore", "1.0.0")]
public class PibRevolution : BaseUnityPlugin
{
    public static double CostMultiplier = 10.0;

    public void Awake()
    {
        Logger.LogInfo("Mod chargé !");
        var harmony = new Harmony("nisshoku.pibcremental.pibrevolution");

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

    [HarmonyPatch(typeof(CS_PapaPibble), "Awake")]
    public class PibRockHeatlBar
    {
        static void Postfix(CS_PapaPibble __instance)
        {
            Vector2 size = new Vector2(200, 30);
            Vector2 anchoredPos = Vector2.zero;
            Vector3 localOffset = new Vector3(0, 1.5f, 0);
            UI_Bar bar = new UI_Bar("Papa Piblle", RenderMode.WorldSpace, size, anchoredPos, Color.red, scale:0.01f, parent: __instance.gameObject.transform, localOffset:localOffset, nameLabelHight:40);

        }//    public static Canvas CreateWorldSpaceCanvas(string name, Transform parent, Vector3 localOffset, float scale = 0.01f)
    }//public static Image CreateFilledBar(Transform parent, Vector2 size, Vector2 anchoredPos, Color color, Vector2? anchor = null)
    //Image CreateBackground(Transform parent, Vector2 size, Vector2 anchoredPos, Color color, Vector2? anchor = null)

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
                RevoltManager.UpPopularity(-10);
            }
            else if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("E");
                RevoltManager.UpPopularity(10);
            }
            if (Keyboard.current.tKey.wasPressedThisFrame) // "T" pour test
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPos);
                mouseWorld.z = 0;
                Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorld);
                Debug.Log("HOVER TEST: " + string.Join(", ", hits.Select(h => h.gameObject.name)));
            }
        }
    }

    [HarmonyPatch(typeof(CS_PapaPibble), "Awake")]
    public class AddHoverGlowPatchPapaPibble
    {
        static void Postfix(CS_PapaPibble __instance)
        {
            __instance.gameObject.AddComponent<PibGlowOnHover>();
        }
    }
    [HarmonyPatch(typeof(CS_SapPibbleBehaviour), "Awake")]
    public class AddHoverGlowPatchSapPibble
    {
        static void Postfix(CS_SapPibbleBehaviour __instance)
        {
            __instance.gameObject.AddComponent<PibGlowOnHover>();
        }
    }
    [HarmonyPatch(typeof(CS_ScrapPibbleBehaviour), "Awake")]
    public class AddHoverGlowPatchScrapPibble { 
        static void Postfix(CS_ScrapPibbleBehaviour __instance)
        {
            __instance.gameObject.AddComponent<PibGlowOnHover>();
        }
    }
    public class PibGlowOnHover : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Material _outlineMaterial;
        private Material _originalMaterial; 
        private Collider2D _myCollider;
        private bool _isHovered;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<SpriteRenderer>();
            }
            _myCollider = GetComponent<Collider2D>();

            if (_renderer == null) return;

            _originalMaterial = _renderer.material; 

            Shader outlineShader = Shader.Find("Shader Graphs/SG_Outline");
            if (outlineShader != null)
            {
                _outlineMaterial = new Material(outlineShader);
                _outlineMaterial.SetTexture("_MainTex", _renderer.sprite.texture);
                _outlineMaterial.SetFloat("_OutlineThickness", 1f);
                _outlineMaterial.SetColor("_OutlineColor", new Color(0, 0, 0, 0));
            }
        }

            void Update()
        {
            if (Mouse.current == null || _myCollider == null) return;

            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPos);
            mouseWorld.z = 0;

            bool isOverThis = _myCollider.OverlapPoint(mouseWorld);

            if (isOverThis && !_isHovered)
            {
                _isHovered = true;
                Debug.Log("[PibGlowOnHover] HOVER ENTER détecté !");
                OnHoverEnter();
            }
            else if (!isOverThis && _isHovered)
            {
                _isHovered = false;
                Debug.Log("[PibGlowOnHover] HOVER EXIT détecté !");
                OnHoverExit();
            }
        }

        private void OnHoverEnter()
        {
            if (_outlineMaterial == null) return;
            _renderer.material = _outlineMaterial;
            _outlineMaterial.SetColor("_OutlineColor", Color.white);
        }

        private void OnHoverExit()
        {
            if (_originalMaterial == null) return;
            _renderer.material = _originalMaterial; 
        }
    }


}