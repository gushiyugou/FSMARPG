using MyAssets.Scripts.Tools;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using XX.Tool.AudioSystem;

public class PlayerModle : ModelBase
{
    private Action<int> atkEndAduio;

    public void AddAtkEndAudio(Action<int> atkEndAduio)
    {
        this.atkEndAduio = atkEndAduio;
    }

    private void AtkEndAudio(int audioIndex)
    {
        atkEndAduio?.Invoke(audioIndex);
    }



    #region 动画事件
    protected override void FootStep()
    {
        if (_Animator.IsInTransition(0) && (_Animator.AnimationAtTag("Motion"))) return;
        AudioManager.Instance.PlayFootStep("FootStep",transform);
    }

    private void CanSwitch()
    {
        (skillOwner as PlayerController).canSwitch = true;
    }



    #endregion
}
