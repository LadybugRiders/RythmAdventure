using UnityEngine;
using System.Collections;

public class BattleAttackEffect : BattleActionEffect {

    override public void Launch(Vector3 _origin, Vector3 _destination)
    {
        base.Launch(_origin, _destination);
        //Utils.SetAlpha(m_effectSprite, 1f);
        gameObject.SetActive(true);
        Utils.Set2DPosition(transform.parent, _destination);
        m_animator.SetTrigger("attack");
    }
}
