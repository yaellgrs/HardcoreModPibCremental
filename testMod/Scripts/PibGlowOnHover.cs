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

    //freeze
    private Rigidbody2D _rb;
    private CS_WorkerPibBase _worker;
    private bool _frozen;
    private RigidbodyConstraints2D _savedConstraints;


    public void Initialize(UI_Bar bar)
    {
        this.bar = bar;

        if (bar != null) bar.Show(false);
    }

    void Awake()
    {
        _worker = GetComponent<CS_WorkerPibBase>();
        _rb = GetComponent<Rigidbody2D>();

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
            Freeze();
            OnHoverEnter();
        }
        else if (!isOverThis && _isHovered)
        {
            _isHovered = false;
            GetComponent<PibRevolt>()?.Add(0.1f);
            Unfreeze();
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
    private void Freeze()
    {
        if (_worker == null || _worker.IsHeld || _frozen) return;
        _frozen = true;

        if (CentralUpdateManager.Instance != null)
            CentralUpdateManager.Instance.Unregister(_worker); 

        if (_rb != null)
        {
            _savedConstraints = _rb.constraints;
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void Unfreeze()
    {
        if (!_frozen) return;
        _frozen = false;

        if (_worker == null || _worker.IsHeld) return;

        if (_rb != null) _rb.constraints = _savedConstraints;
        if (CentralUpdateManager.Instance != null)
            CentralUpdateManager.Instance.Register(_worker);
    }
}
