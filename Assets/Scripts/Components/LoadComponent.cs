using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;

public class LoadComponent : BaseComponent
{
    //private BearComponent bearCmp;

    //public LoadComponent(string name, GameObject game_Obj, int w, int h) : base(name, game_Obj, w, h)
    //{

    //}

    public override void OnEnter()
    {
        base.OnEnter();

        //bearCmp = null;
    }

    public override void OnExit()
    {
        //bearCmp.UnLoadCmp(_name);

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

    /// <summary>
    /// ����������Ŀ�����װ�ض���
    /// </summary>
    /// <param name="bear"></param>
    public void Loaded(BearComponent bear)
    {
        //bearCmp.UnLoadCmp(_name);
        //bearCmp = bear;
    }
}
