using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStepUserInput : UIStep {

    public override void Launch(OnStepEndDelegate _del)
    {
        base.Launch(_del);
    }

    protected override void UpdateStep()
    {
        base.UpdateStep();
        if (Input.GetMouseButtonDown(0))
        {
            Stop();
        }
    }
}
