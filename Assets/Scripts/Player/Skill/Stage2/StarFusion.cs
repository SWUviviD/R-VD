using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StarFusion : SkillBase
{
    [SerializeField] private Transform fusionPos;
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private PlayerAnimation anim;

    public override void OnSkillStart(InputAction.CallbackContext _playerStatus)
    {

        // Play Animation & Sound
        anim.OnFusion();

        Collider[] hits = Physics.OverlapSphere(fusionPos.position, radius, 1 << 23);

        if (hits.Length <= 0) return;

        foreach (Collider hit in hits)
        {
            IFusionable f = GetFusionable(hit);
            if (f == null) continue;

            if (f.Activate(transform) == true)
            {
                // do something
            }
            else
            {
                // do something
            }
        }
    }

    private IFusionable GetFusionable(Collider _hit)
    {
        IFusionable f = null;
        if(_hit.TryGetComponent<IFusionable>(out f))
        {
            return f;
        }

        f = _hit.GetComponentInChildren<IFusionable>();
        if (f != null)
        {
            return f;
        }

        f = _hit.GetComponentInParent<IFusionable>();
        return f;
    }

    public override void OnSkill(InputAction.CallbackContext _playerStatus) { }

    public override void OnSkillStop(InputAction.CallbackContext _playerStatus) { }
}
