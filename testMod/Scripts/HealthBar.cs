using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class HealthBar
{
    public static GameObject CreateHealthBar(Transform parent, string name)
    {
        Vector2 size = new Vector2(200, 30);

        //canva
        GameObject canvasObj = new GameObject("HealthBarCanva");
        canvasObj.transform.SetParent(parent, false);
        canvasObj.transform.localPosition = new Vector3(0, 1.5f, 0);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.transform.localScale = Vector3.one * 0.01f;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = size;


        //background
        GameObject backgroundObj = new GameObject("background");
        backgroundObj.transform.SetParent(canvasObj.transform, false);

        Image backgroundImg = backgroundObj.AddComponent<Image>();
        backgroundImg.color= Color.black;
        RectTransform backgroundRect= backgroundObj.GetComponent<RectTransform>();
        backgroundRect.sizeDelta = size;


        //fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(canvasObj.transform, false);

        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = Color.green;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;


        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.sizeDelta = size;


        //label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(canvasObj.transform, false);
        
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = name + " : 100/100";
        label.fontSize = 25;
        label.fontWeight = FontWeight.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.black;

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(400, 30);
        labelRect.anchoredPosition = new Vector2(0, 40);


        return canvasObj;
    }
}