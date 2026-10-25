using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PibGlowOnHover : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Material _outlineMaterial;
    private Material _originalMaterial;
    private Collider2D _myCollider;
    public bool isHovered;
    private UI_Bar bar;
    public static CS_SapPibbleBehaviour sapInstance = null;
    public  CS_SapPibbleBehaviour _sap = null;

    public bool _paused = false;

    private Animator _anim;

    public static bool locked = false;


    public void Initialize(UI_Bar bar)
    {
        this.bar = bar;

        if (bar != null) bar.Show(false);
    }

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

        _sap = GetComponent<CS_SapPibbleBehaviour>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Mouse.current == null || _myCollider == null) return;
        if (sapInstance != null && sapInstance != _sap) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPos);
        mouseWorld.z = 0;

        bool isOverThis = _myCollider.OverlapPoint(mouseWorld);

        if (isOverThis && !isHovered && sapInstance == null)
        {
            isHovered = true;
            sapInstance = _sap;
            OnHoverEnter();
        }
        else if (!locked && !isOverThis && isHovered)
        {
            isHovered = false;
            OnHoverExit();
            sapInstance = null;
        }


        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (isOverThis) locked = true;
            if (!isOverThis && locked) locked = false;
        }

    }

    private void Pause()
    {
        if (_paused || sapInstance == null) return;
        var brain = CS_SapPibBrain.Instance;
        if (brain == null) return;

        brain.SetPaused(sapInstance, true);
        if (_anim != null) _anim.speed = 0f;
        _paused = true;
    }

    private void Resume()
    {
        if (!_paused) return;
        _paused = false;

        var brain = CS_SapPibBrain.Instance;
        if (brain != null && sapInstance != null) brain.SetPaused(sapInstance, false);
        if (_anim != null) _anim.speed = 1f;
    }

    private void OnHoverEnter()
    {
        Pause();
        if (_outlineMaterial == null) return;
        _renderer.material = _outlineMaterial;
        _outlineMaterial.SetColor("_OutlineColor", Color.white);
        if (bar != null) bar.Show(true);
    }

    private void OnHoverExit()
    {
        Resume();
        if (_originalMaterial == null) return;
        _renderer.material = _originalMaterial;
        if (bar != null) bar.Show(false);
    }
}

[HarmonyPatch(typeof(CS_SapPibbleBehaviour), "UpdateVisuals")]
public class FreezeSapPibMovement
{
    public static float time = 0f;
    static void Prefix(CS_SapPibbleBehaviour __instance)
    {
        time += Time.deltaTime;
        if (time > 1f)
        {
            time = 0f;
            if(UnityEngine.Random.Range(0, 100) <= 5)
            {
                __instance.GetComponent<PibRevolt>()?.Add(0.01f);
            }
        }

    }

}