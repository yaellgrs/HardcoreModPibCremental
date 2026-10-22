using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class PibRevolt : MonoBehaviour
{
    public float Revolt = 0.0f;
    public UI_Bar bar;

    public void Add(float delta)
    {
        Revolt = Mathf.Clamp(Revolt + delta, 0f, 1f);
        bar?.SetValue(Revolt);

        RevoltManager.NotifyRevoltChange(delta);
    }

    public void Init(UI_Bar bar)
    {
        this.bar = bar;
        bar?.SetValue(Revolt);
    }

    public void OnEnable() { RevoltManager.Register(Revolt); }
    public void OnDisable() { RevoltManager.UnRegister(Revolt); }
}

