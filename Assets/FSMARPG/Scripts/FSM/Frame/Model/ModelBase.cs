
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModelBase:MonoBehaviour
{
    protected Animator _animator;
    public Animator _Animator { get { return _animator; } }
    protected Action<Vector3, Quaternion> rootMotionAction;
    protected ISkillOwner skillOwner;

    public void OnInit(ISkillOwner skillOwner, List<string> enemyTagList)
    {

        this.skillOwner = skillOwner;
    }

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction += rootMotionAction;
    }

    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }




    #region 根运动
    protected void OnAnimatorMove()
    {
        this.rootMotionAction?.Invoke(_animator.deltaPosition, _animator.deltaRotation);
        //_controller.Move(_animator.deltaPosition);

    }
    #endregion



    #region 动画事件


    protected Action<string> runStopAction;
    protected virtual void FootStep()
    {
       
    }

    

    protected void StartSkillHit(int weaponIndex)
    {
        // 安全检查但不阻止执行
       
    }

    protected void StopSkillHit(int weaponIndex)
    {

        
    }

    protected void SkillCanSwitch()
    {
        
    }


    #endregion
}
