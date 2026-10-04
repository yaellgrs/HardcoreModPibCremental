using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class RevoltManager
{
    public static float ChefPopularity = 100f;

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

    public static GameObject CreatePopularityBarre()
    {
        Vector2 size = new Vector2(400, 40);
        float BarPositionY = -60;

        //canva
        GameObject canvasObj = new GameObject("PopularityBar");
        canvasObj.transform.localPosition = new Vector3(0, -5f, 0);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;



        //background
        GameObject backgroundObj = new GameObject("background");
        backgroundObj.transform.SetParent(canvasObj.transform, false);



        Image backgroundImg = backgroundObj.AddComponent<Image>();
        backgroundImg.color = Color.black;
        //RectTransform backgroundRect = backgroundObj.GetComponent<RectTransform>();
        //backgroundRect.sizeDelta = size;
        RectTransform canvasRect = backgroundObj.GetComponent<RectTransform>();
        canvasRect.anchorMin = new Vector2(0.5f, 1f);
        canvasRect.anchorMax = new Vector2(0.5f, 1f);
        canvasRect.pivot = new Vector2(0.5f, 1f);
        canvasRect.anchoredPosition = new Vector2(0, BarPositionY);
        canvasRect.sizeDelta = size;


        //fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(canvasObj.transform, false);

        RevoltManager.Img_fillBar = fillObj.AddComponent<Image>();
        RevoltManager.Img_fillBar.color = Color.cyan;
        RevoltManager.Img_fillBar.type = Image.Type.Filled;
        RevoltManager.Img_fillBar.fillMethod = Image.FillMethod.Horizontal;

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

        RevoltManager.Img_fillBar.sprite = sprite;

        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0.5f, 1f);
        fillRect.anchorMax = new Vector2(0.5f, 1f);
        fillRect.pivot = new Vector2(0.5f, 1f);
        fillRect.anchoredPosition = new Vector2(0, BarPositionY);
        fillRect.sizeDelta = size;

        GameObject Obj_pourcent = new GameObject("Label");
        Obj_pourcent.transform.SetParent(canvasObj.transform, false);

        RevoltManager.Lbl_pourcent = Obj_pourcent.AddComponent<TextMeshProUGUI>();
        RevoltManager.Lbl_pourcent.text = "100%";
        RevoltManager.Lbl_pourcent.fontSize = 25;
        RevoltManager.Lbl_pourcent.fontWeight = FontWeight.Bold;
        RevoltManager.Lbl_pourcent.alignment = TextAlignmentOptions.Center;
        RevoltManager.Lbl_pourcent.color = Color.white;

        RectTransform Rect_pourcent = Lbl_pourcent.GetComponent<RectTransform>();
        Rect_pourcent.anchorMin = new Vector2(0.5f, 1f);
        Rect_pourcent.anchorMax = new Vector2(0.5f, 1f);
        Rect_pourcent.pivot = new Vector2(0.5f, 1f);
        Rect_pourcent.anchoredPosition = new Vector2(0, BarPositionY);
        Rect_pourcent.sizeDelta = new Vector2(size.x * 1.5f, size.y);


        //label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(canvasObj.transform, false);

        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = "Papa Pibble popularity";
        label.fontSize = 25;
        label.fontWeight = FontWeight.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.black;

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 1f);
        labelRect.anchorMax = new Vector2(0.5f, 1f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.anchoredPosition = new Vector2(0, -15);
        labelRect.sizeDelta = new Vector2(size.x * 1.5f, size.y);


        return canvasObj;
    }
}
