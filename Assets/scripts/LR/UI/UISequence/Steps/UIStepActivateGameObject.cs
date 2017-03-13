using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStepActivateGameObject : UIStep {

    [SerializeField] List<GameObject> m_targets;

    [SerializeField] bool m_activate = true;

    public override void Launch(OnStepEndDelegate _del)
    {
        base.Launch(_del);
        foreach(var go in m_targets)
        {
            go.SetActive(m_activate);
        }
        Stop();
    }
}
