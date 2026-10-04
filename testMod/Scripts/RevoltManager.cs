using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class RevoltManager
{
    public static float ChefPopularity = 100f;

    //private static readonly ConditionalWeakTable<object, PibFatigue> _healthMap = new();

    private static TextMeshProUGUI Lbl_pourcent;
    private static Image Img_fillBar;

    public static void SetPopularity(float value)
    {
        ChefPopularity = Mathf.Clamp(value, 0, 100f);
        refreshUI();
    }

    public static void UpPopularity(float delta)
    {
        SetPopularity(ChefPopularity + delta);
    }

    public static void refreshUI()
    {
        if(Lbl_pourcent != null){
            Lbl_pourcent.text = ChefPopularity + "%";
        }
        if (Img_fillBar != null)
        {
            Img_fillBar.fillAmount = ChefPopularity/100f;
            Debug.Log("img fill amount : " + Img_fillBar.fillAmount);
        }
        else
        {
            Debug.LogError("IMG_fill Bar is null");
        }
    }

    public static void CreatePopularityBarre()
    {
        Vector2 size = new Vector2(400, 40);
        Vector2 anchor = new Vector2(0.5f, 1f);

        Vector2 anchoredPos = new Vector2(0, -60);
        Vector3 localOffset = new Vector3(0, 1.5f, 0);
        UI_Bar bar = new UI_Bar("Papa Pibble popularity", RenderMode.ScreenSpaceOverlay, size, anchoredPos, Color.cyan, anchor: anchor);

        Lbl_pourcent = bar.GetValueLabel();
        Img_fillBar = bar.GetFilledImage();
}
}
/*
      }//    public static Canvas CreateWorldSpaceCanvas(string name, Transform parent, Vector3 localOffset, float scale = 0.01f)
    }//public static Image CreateFilledBar(Transform parent, Vector2 size, Vector2 anchoredPos, Color color, Vector2? anchor = null)
    //Image CreateBackground(Transform parent, Vector2 size, Vector2 anchoredPos, Color color, Vector2? anchor = null)
*/