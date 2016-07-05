using UnityEngine;
using System.Collections;

public class BattleNoteSlide : BattleNote {

	// Use this for initialization
	override protected void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	override protected void Update () {
        base.Update();
	}

    override public void Hit(BattleSlot _slot)
    {
        base.Hit(_slot);
    }
}
