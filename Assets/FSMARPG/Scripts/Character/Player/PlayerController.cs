using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



public enum PlayerStateType
{
    Idle,
    Move,
    MoveStop,
    Evade,
    NormalAttack,
    NormalAttackStop

}
public class PlayerController : CharacterBase
{

    #region Inspector面板显示的内容
    [Header("主摄像机控制参数")]
    [InspectorRange(0,10)] public float rotationSpeed = 5f;
    public NormalAttackActionList_SO normalAttackActionList;

    #endregion


    #region Inspector不显示的内容
    [NonSerialized] public AudioSource anbiAudioSource;
    [NonSerialized] public bool canSwitch = false;
    [NonSerialized] public bool isEvadeBack;
    [NonSerialized] public int attackHitCount;
    [NonSerialized] public int currentAttackIndex;
    [NonSerialized] public int lastAttackIndex;
    #endregion


    #region 弃用或没有的属性
    //[SerializeField] private Transform effectBaseTransform;

    #endregion



    #region 生命周期函数相关
    public override void Init()
    {
        base.Init();
        anbiAudioSource = GetComponent<AudioSource>();
        ChangeState(PlayerStateType.Idle);
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Update()
    {
        Debug.DrawRay(Model.transform.position, Model.transform.forward * 3, Color.blue);
    }


    #endregion



    #region 状态切换
    public void ChangeState(PlayerStateType stateType,bool reCurrent = false)
    {
        switch (stateType)
        {
            case PlayerStateType.Idle:
                stateMachine.ChangeState<PlayerIdle_State>(reCurrent);
                break;
            case PlayerStateType.Move:
                stateMachine.ChangeState<PlayerMove_State>(reCurrent);
                break;
            case PlayerStateType.MoveStop:
                stateMachine.ChangeState<PlayerMoveStop_State>(reCurrent);
                break;
            case PlayerStateType.Evade:
                stateMachine.ChangeState<PlayerEvade_State>(reCurrent);
                break;
            case PlayerStateType.NormalAttack:
                stateMachine.ChangeState<PlayerNormalAttack_State>(reCurrent);
                break;
            case PlayerStateType.NormalAttackStop:
                stateMachine.ChangeState<PlayerNormalAttackStop_State>(reCurrent);
                break;
        }
    }

    #endregion


    #region 弃用的函数
    private void UpdateCharacterRotation()
    {
        //rotationEulerAngles = Mathf.Atan2(GameInputManager.Instance.MovementInput.x,
        //    GameInputManager.Instance.MovementInput.y) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

        //if (GetAnimationTag("Motion"))
        //{
        //    Vector3 input = new Vector3(GameInputManager.Instance.MovementInput.x, 0, GameInputManager.Instance.MovementInput.y);
        //    float y = Camera.main.transform.eulerAngles.y;

        //    //让四元数与向量相乘，表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
        //    //Vector3 targetDiretion = Quaternion.Euler(0, y, 0) * input;
        //    //Model.transform.rotation = Quaternion.Slerp(
        //    //    Model.transform.rotation, Quaternion.LookRotation(targetDiretion),
        //    //    Time.deltaTime * rotationSpeed);
        //    //角度旋转不适用模型控制层非模型层的情况
        //    //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y,
        //    //    rotationEulerAngles, ref meanEulerAnglesValue, rotationTime);
        //    //characterRotationTargetDirection = Quaternion.Euler(0, rotationEulerAngles,0)* transform.forward;
        //}
    }


    #endregion


    #region 攻击相关

        #region 攻击效果

    /// <summary>
    /// 特效生成
    /// </summary>
    /// <param name="action"></param>
    public void SpawnNormalAttackEffect(NormalAttackAction_SO action)
    {
        if (action.EffectPrefab.name == "") return;
        StartCoroutine(SpawnEffcetCornoutine(action.EffectPrefab));
    }

    /// <summary>
    /// 特效生成协程
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    private IEnumerator SpawnEffcetCornoutine(AttackEffect effect)
    {
        yield return new WaitForSecondsRealtime(effect.spawnerTime);

        GameObject item = PoolManager.Instance.GetPoolItem(effect.name, effect.attackEffect, parent:Model.transform);
        if (item != null)
        {
            item.transform.position = SetSpawnPosition(effect, Model.transform);
            //Debug.Log(Model.transform.forward);
            item.transform.localEulerAngles= new Vector3(0,90,0);
            //item.transform.rotation = Quaternion.AngleAxis(effect.rotationAngle,Model.transform.up);
            item.SetActive(true);
            if (!effect.isSonCharacter)
            {
                item.transform.parent = null;
            }
        }

    }

    /// <summary>
    /// 设置生成位置
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="baseTransform"></param>
    /// <returns></returns>
    public Vector3 SetSpawnPosition(AttackEffect effect, Transform baseTransform)
    {
        Vector3 targetPosition = baseTransform.position + 
            baseTransform.forward* effect.position.z+
            baseTransform.up * effect.position.y +
            baseTransform.right * effect.position.x;
        //targetPosition += effect.position;
        //Vector3 targetPosition = effectBaseTransform.position;

        return targetPosition;
    }

        #endregion



    #endregion




    #region 动画状态机相关


        #region 动画事件


        #endregion

    private bool GetAnimationTag(string TagName)
    {
        return Model._Animator.GetCurrentAnimatorStateInfo(0).IsTag(TagName);
    }

    #endregion



}
