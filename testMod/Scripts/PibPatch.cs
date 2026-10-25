using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class PibPatch
{
    [HarmonyPatch]
    public class AddHoverGlow
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CS_SapPibbleBehaviour), "Awake");
            yield return AccessTools.Method(typeof(CS_ScrapPibbleBehaviour), "Awake");
        }

        static void Postfix(object __instance)
        {
            var mb = __instance as MonoBehaviour;
            if (mb == null) return;
            if( mb.GetComponent<PibGlowOnHover>() == null)
            {
                Vector2 size = new Vector2(200, 30);
                Vector2 anchoredPos = Vector2.zero;
                Vector3 localOffset = new Vector3(0, 1.5f, 0);
                UI_Bar bar = new UI_Bar("Pibble", RenderMode.WorldSpace, size, anchoredPos, Color.green, scale: 0.01f, parent: mb.gameObject.transform, localOffset: localOffset, nameLabelHight: 40);

                bar.AddButton("UnAssign", new Vector2(200, 26), new Vector2(0, -50), new Color(0.7f, 0.1f, 0.1f, 1f));

                PibGlowOnHover glow = mb.gameObject.AddComponent<PibGlowOnHover>();
                glow.Initialize(bar);
                if (mb.GetComponent<PibRevolt>() == null)
                {
                    mb.gameObject.AddComponent<PibRevolt>();
                    mb.GetComponent<PibRevolt>().Init(bar);
                }


            }
        }

    }

}

