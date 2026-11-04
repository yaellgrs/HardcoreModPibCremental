using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public static class RevoltManager
{
    public static float ChefPopularity = 1.0f;

    //private static readonly ConditionalWeakTable<object, PibFatigue> _healthMap = new();

    private static TextMeshProUGUI Lbl_pourcent;
    private static Image Img_fillBar;

    //revolt moyenne
    private static int _count;
    private static float _sum;
    public static int Count => _count;
    public static float Sum => _sum;
    public static float Average => _count == 0 ? 0f : _sum / _count;

    public static List<float> revoltPool;
    public static TextMeshProUGUI Lbl_revoltMoyenne;

    public const string MoyenneRevoltPrefix = "Revolt Moyenne : "; 

    public static float time = 0f;
    public static float timePopularity = 0f;


    //events
    public static event Action<float> OnAverageChanged;

    public static void Init()
    {
        if(revoltPool == null) revoltPool = new List<float>();
        OnAverageChanged -= OnAverage;
        OnAverageChanged += OnAverage;
    }

#region revolt moyenne

    public static void NotifyRevoltChange(float delta)
    {
        _sum += delta;
        OnAverageChanged?.Invoke(Average);
    }

    public static void Register(float revolt)
    {
        _count++;
        _sum += revolt;
        OnAverageChanged?.Invoke(Average);
    }

    public static void UnRegister(float revolt)
    {
        revoltPool.Add(revolt);
    }

    public static float GetRevoltFromPool() {
        float value = revoltPool[0];
        revoltPool.RemoveAt(0);
        return value;
    }

    private static void OnAverage(float avg)
    {
        refreshRevoltMoyenne();
    }

    private static void UpChefPopularity(float delta)
    {
        ChefPopularity = Mathf.Clamp(ChefPopularity + delta, 0f, 1f);
        refreshChefPopularityUI();
    }

    #endregion

    public static void refreshRevoltMoyenne()
    {
        if(Lbl_pourcent != null){
            Lbl_revoltMoyenne.text = MoyenneRevoltPrefix + (Average * 100).ToString("F2") + "%";
        }
        else
        {
            Debug.LogError("IMG_fill Bar is null");
        }
    }
    public static void refreshChefPopularityUI()
    {
        if (Lbl_pourcent != null)
        {
            Lbl_pourcent.text = (ChefPopularity * 100).ToString("F2") + "%";
        }
        if (Img_fillBar != null)
        {
            Img_fillBar.fillAmount = ChefPopularity;
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

    public static void CreateRevoltMoyenne()
    {
        GameObject canvasObj = new GameObject();
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        GameObject obj = new GameObject("background");
        obj.transform.SetParent(canvas.transform, false);

        Lbl_revoltMoyenne = obj.AddComponent<TextMeshProUGUI>();
        Lbl_revoltMoyenne.text = MoyenneRevoltPrefix + "100.00%";
        Lbl_revoltMoyenne.color = Color.black;
        Lbl_revoltMoyenne.fontSize = 22;
        Lbl_revoltMoyenne.fontWeight = FontWeight.Bold;
        Lbl_revoltMoyenne.alignment = TextAlignmentOptions.Center;

        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector2 anchor = new Vector2(0.5f, 1f);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.sizeDelta = new Vector2(500, 30);   
        rect.anchoredPosition = new Vector2(0, -105);

    }



    public static void Update()
    {
        time += Time.deltaTime;
        timePopularity += Time.deltaTime;
        if (time > 1f)
        {
            time = 0f;
            for(int i = 0; i< revoltPool.Count; i++)
            {
                if (UnityEngine.Random.Range(0, 100) <= GetProbabilityRevolt())
                {
                    revoltPool[i] = Mathf.Max(0f, revoltPool[i] - 0.01f);
                }
            }


        }
        if(timePopularity > 10f)
        {
            timePopularity = 0f;
            if (Average >= 0.9f)
            {
                UpChefPopularity(0.005f);
            }
            else if (Average >= 0.75f)
            {
                //rien
            }
            else if (Average >= 0.5f)
            {
                UpChefPopularity(-0.005f);
            }
            else if (Average >= 0.25f)
            {
                UpChefPopularity(0.01f);
            }
            else
            {
                UpChefPopularity(0.015f);
            }
        }
    }


    public static float GetProbabilityRevolt()
    {
        return 10f;
    }
}
/*
      }//    public static Canvas CreateWorldSpaceCanvas(string name, Transform parent, Vector3 localOffset, float scale = 0.01f)
    }//public static Image CreateFilledBar(Transform parent, Vector2 size, Vector2 anchoredPos, Color color, Vector2? anchor = null)
    //Image CreateBackground(Transform parent, Vector2 size, Vector2 anchoredPos, Color color, Vector2? anchor = null)
*/