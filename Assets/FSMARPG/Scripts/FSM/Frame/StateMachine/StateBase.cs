using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ����״̬�Ļ���
/// </summary>
public abstract class StateBase 
{
   /// <summary>
   /// ��ʼ��
   /// ֻ��״̬��һ�δ���ʱ��Ӷ
   /// </summary>
   /// <param name="owner">״̬������</param>
   /// <param name="stateType">״̬�ı�ʶ</param>
    public virtual void Init(IStateMachineOwner owner) { }

    /// <summary>
    /// ����ʼ��
    /// ����ʱ�ͷ�һЩ��Դ
    /// </summary>
    public virtual void UnInit() { }

    /// <summary>
    ///状态进入时调佣一次，通过事件的形式添加到了MonoManager中，在MonoManager中回调执行
    /// </summary>
    public virtual void Enter(){}
    /// <summary>
    ///状态结束时调用一次，通过事件的形式添加到了MonoManager中，在MonoManager中回调执行
    /// </summary>
    public virtual void Exit() { }
    /// <summary>
    ///每帧更新，通过事件的形式添加到了MonoManager中，在MonoManager中回调执行
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// 延迟帧更新，通过事件的形式添加到了MonoManager中，在MonoManager中回调执行
    /// </summary>
    public virtual void LateUpdate() { }

    /// <summary>
    /// 物理帧更新，通过事件的形式添加到了MonoManager中，在MonoManager中回调执行
    /// </summary>
    public virtual void FixedUpdate() { }

}
