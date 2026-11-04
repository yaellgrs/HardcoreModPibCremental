using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum RevoltStatut
{
    Discontent, Strike, ProlongedStrike, Protest, Uprising
}


public class PibRevolt : MonoBehaviour
{
    public float Revolt = 0.0f;
    public UI_Bar bar;

    public bool Strike = false;

    public RevoltStatut statut;

    public float time = 0f;
    public float strikeTimer = 0f;
    private float _cooldown = 0f;
    public const float STRIKE_TIME = 10f;


    public void Add(float delta)
    {
        float oldRevolt = Revolt;
        Revolt = Mathf.Clamp(Revolt + delta, 0f, 1f);
        bar?.SetValue(Revolt);

        RevoltManager.NotifyRevoltChange(Revolt - oldRevolt);
        UpStatut();
    }

    private void UpStatut()
    {
        if(Revolt < 0.5f)       statut = RevoltStatut.Discontent;
        else if(Revolt < 0.75f) statut = RevoltStatut.Strike;
        else if (Revolt < 0.9f) statut = RevoltStatut.ProlongedStrike;
        else if(Revolt < 1f)    statut = RevoltStatut.Protest;
        else                    statut = RevoltStatut.Uprising;
    }

    public void Init(UI_Bar bar)
    {
        this.bar = bar;
        bar?.SetValue(Revolt);
    }

    public void OnEnable() {
        if (RevoltManager.revoltPool.Count > 0)
        {
            Revolt = RevoltManager.GetRevoltFromPool();
        }
        else
        {
            RevoltManager.Register(Revolt);
        }
        bar?.SetValue(Revolt);

    }
    public void OnDisable() { RevoltManager.UnRegister(Revolt); }



    public void Update()
    {
        time += Time.deltaTime;
        if (time > 1f)
        {
            time = 0f;
            if (UnityEngine.Random.Range(0, 100) <= RevoltManager.GetProbabilityRevolt())
            {
                Add(0.20f);
            }
            if (strikeTimer <= 0 && _cooldown  <= 0f && UnityEngine.Random.Range(0, 100) <= 1)
            {
                strikeTimer = UnityEngine.Random.Range(STRIKE_TIME / 2, STRIKE_TIME);
                Strike = true;
            }
        }
        if (Strike)
        {
            strikeTimer -= Time.deltaTime;
            if (strikeTimer <= 0f)
            {
                Strike = false;
                _cooldown = 10f;  
            }
        }
        else if (_cooldown > 0f)
        {
            _cooldown -= Time.deltaTime;
        }
    }
}

