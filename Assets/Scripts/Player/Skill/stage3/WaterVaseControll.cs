using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class WaterVaseControll : SkillBase
{
    [SerializeField] private Renderer[] Point;
    [SerializeField] private Material noWater;
    [SerializeField] private Material yesWater;

    [SerializeField] private Transform fusionPos;
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private PlayerAnimation anim;
    public bool remainingUsage;


    private void Start()
    {
        remainingUsage = false;
        SetPointColor();
    }

    private void SetPointColor()
    {
        foreach (var p in Point)
        {
            p.material = remainingUsage ? yesWater : noWater;
        }
    }


    public void watermove()
    {
        if (remainingUsage)
        {
            remainingUsage = false;
            SetPointColor();
        }
        else
        {
            remainingUsage = true;
            SetPointColor();
        }
    }

    public override void OnSkillStart(InputAction.CallbackContext _playerStatus)
    {

        // Play Animation & Sound
        anim.OnWater();

        Collider[] hits = Physics.OverlapSphere(fusionPos.position, radius, 1 << 24);

        if (hits.Length <= 0) return;

        IWaterable f = GetWaterable(hits[0]);
        if (f == null)
            return;

        f.OnWater(transform);

        //foreach (Collider hit in hits)
        //{
        //    IWaterable f = GetWaterable(hit);
        //    if (f == null) continue;

        //    f.OnWater(transform);
        //}
    }

    private IWaterable GetWaterable(Collider _hit)
    {
        IWaterable f = null;
        if (_hit.TryGetComponent<IWaterable>(out f))
        {
            return f;
        }

        f = _hit.GetComponentInChildren<IWaterable>();
        if (f != null)
        {
            return f;
        }

        f = _hit.GetComponentInParent<IWaterable>();
        return f;
    }

    public override void OnSkill(InputAction.CallbackContext _playerStatus)
    {
        
    }

    public override void OnSkillStop(InputAction.CallbackContext _playerStatus)
    {
        
    }
}
