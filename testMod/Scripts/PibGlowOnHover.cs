using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool _isHovered;
    private UI_Bar bar;

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
            OnHoverEnter();
        }
        else if (!isOverThis && _isHovered)
        {
            _isHovered = false;
            OnHoverExit();
        }
    }

    private void OnHoverEnter()
    {
        if (_outlineMaterial == null) return;
        _renderer.material = _outlineMaterial;
        _outlineMaterial.SetColor("_OutlineColor", Color.white);
        if (bar != null) bar.Show(true);
    }

    private void OnHoverExit()
    {
        if (_originalMaterial == null) return;
        _renderer.material = _originalMaterial;
        if (bar != null) bar.Show(false);
    }
}
