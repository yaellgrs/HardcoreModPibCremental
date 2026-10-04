using System.Reflection.Emit;
using System.Security.Policy;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bar
{
    private Sprite Sp_fillBarr;
    private Canvas canvas;
    private GameObject Obj_backgorund;
    private GameObject Obj_fillBar;
    private Image Img_fill;

    private TextMeshProUGUI Lbl_name;
    private TextMeshProUGUI Lbl_value;
    private float maxValue;




    public UI_Bar(string name, RenderMode renderMode,  Vector2 size, Vector2 anchoredPos, Color color, float scale = 1f, Transform parent = null, float maxValue = 100f, Vector2? anchor = null, Vector3? localOffset = null, float nameLabelHight = -15f)
    {
        this.maxValue = maxValue;
        //sprite
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        Sp_fillBarr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

        //canva
        GameObject canvasObj = new GameObject();
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = renderMode;
        if(renderMode == RenderMode.ScreenSpaceOverlay)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        else
        {
            canvasObj.GetComponent<RectTransform>().sizeDelta = size;
            if (parent != null)canvasObj.transform.SetParent(parent, false);
            if(localOffset != null) canvasObj.transform.localPosition = (Vector3)localOffset;
            canvasObj.transform.localScale = Vector3.one * scale;
        }

        //background
        Obj_backgorund = new GameObject("background");
        Obj_backgorund.transform.SetParent(canvas.transform, false);

        Image img = Obj_backgorund.AddComponent<Image>();
        img.sprite = Sp_fillBarr;
        img.color = new Color(0, 0, 0, 0.5f);

        SetRect(Obj_backgorund.GetComponent<RectTransform>(), size, anchoredPos, anchor);

        //fill
        Obj_fillBar = new GameObject("Fill");
        Obj_fillBar.transform.SetParent(canvas.transform, false);

        Img_fill = Obj_fillBar.AddComponent<Image>();
        Img_fill.sprite = Sp_fillBarr;
        Img_fill.color = color;
        Img_fill.type = Image.Type.Filled;
        Img_fill.fillMethod = Image.FillMethod.Horizontal;

        SetRect(Obj_fillBar.GetComponent<RectTransform>(), size, anchoredPos, anchor);

        string value = (maxValue + "/" + maxValue);
        Lbl_name = CreateLabel(canvas.transform, name, size, new Vector2(0, nameLabelHight), color: Color.black, anchor: anchor);
        Lbl_value = CreateLabel(canvas.transform, value, size, anchoredPos, color: Color.white, anchor: anchor);
    }



    public static TextMeshProUGUI CreateLabel(Transform parent, string text, Vector2 size, Vector2 anchoredPos, int fontSize = 25, Color? color = null, Vector2? anchor = null)
    {
        GameObject obj = new GameObject("Label");
        obj.transform.SetParent(parent, false);

        TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontWeight = FontWeight.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color ?? Color.black;

        SetRect(obj.GetComponent<RectTransform>(), size, anchoredPos, anchor);
        return label;
    }

    private static void SetRect(RectTransform rect, Vector2 size, Vector2 anchoredPos, Vector2? anchor)
    {
        if (anchor.HasValue)
        {
            rect.anchorMin = anchor.Value;
            rect.anchorMax = anchor.Value;
            rect.pivot = anchor.Value;
        }
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
    }


    public TextMeshProUGUI GetValueLabel() { return Lbl_value; }
    public Image GetFilledImage() { return Img_fill; }
}
