using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;

public class SenserComponent : BaseComponent
{
    public List<string> eventTypes = new List<string>();
    //public Senser(string name, GameObject game_Obj, int w, int h) : base(name, game_Obj, w, h)
    //{

    //}
   

    public override void OnEnter()
    {
        base.OnEnter();

        foreach (var evt in eventTypes)
        {
            EventModule.AddEvent(evt);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnFreeze()
    {
        base.OnFreeze();
    }

    public override void UnFreeze()
    {
        base.UnFreeze();
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnPause()
    {
        base.OnPause();
    }


}
