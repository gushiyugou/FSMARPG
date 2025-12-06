

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class CharacterBase : MonoBehaviour, IStateMachineOwner, ISkillOwner,IHurt
{
    #region 控制器基础组件
    public  float gravity = -9.8f;
    [Header("基础组件")]
    [SerializeField] protected ModelBase model;
    public ModelBase Model { get => model; }
    public Transform ModelTransform => Model.transform;
    [SerializeField] protected CharacterController _characterController;
    public CharacterController characterController { get => _characterController; }
    protected StateMachine stateMachine;

    [SerializeField] protected AudioSource audioSource;
    #endregion

    [Header("敌人Tag列表")] public List<string> enemyTagList;
    
    
    #region ****************地面检测相关参数****************\
    protected Vector3 movementDirection; 
    [Header("地面检测相关参数")]
    public bool characterOnGround;
    [SerializeField]protected float detectionGroundPositionOffset;
    [SerializeField]protected float detectionGroundRange;
    [SerializeField]protected LayerMask whatIsGround;
        
    #endregion

    #region ****************重力应用相关参数****************
    protected float characterVerticalVelocity;
    protected float characterVerticalMaxVelocity = 54f;
    protected float fallOutDeltaTime;
    protected float fallOutTime = 0.05f;
    protected Vector3 characterVelocityDerection;
    protected bool isEnabelGravity;

    #endregion
    
    
    public virtual void Init()
    {
        Model.OnInit(this, enemyTagList);
        stateMachine = new StateMachine();
        stateMachine.Init(this);
        _characterController = GetComponent<CharacterController>();
        CanSwitchSkill = true;
        audioSource = GetComponent<AudioSource>();
    }
    
    #region ****************生命周期函数、引擎回调函数相关****************
    protected virtual void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start()
    {
        fallOutDeltaTime = fallOutTime;
        isEnabelGravity = true;
    }

    protected virtual void OnEnable()
    {
            
    }

    protected virtual void OnDisable()
    {
            
            
    }

    protected virtual void Update()
    {

        CharacterApplyGravity();
        UpdateCharacterGravity();

    }

    protected virtual void LateUpdate()
    {
            
    }

    #endregion




    #region 技能相关
    /// <summary>
    /// 技能配置数组
    /// </summary>
    [Header("技能配置")]
    protected int currentHitIndex = 0;
    public int currentHitWeapIndex;
    public bool CanSwitchSkill { get; protected set; }
    public ISkillOwner HitSource { get; protected set; }
    
    //技能后摇的部分
    public virtual void SkillCanSwitch()
    {
        CanSwitchSkill = true;
    }
    public virtual void OnHit(IHurt target, Vector3 hitPosition)
    {

        

    }

    /// <summary>
    /// 卡肉效果
    /// </summary>
    /// <param name="force"></param>
    public  void DoFreezeFrameTime(float time)
    {
        StartCoroutine(StartFreezeFrameTime(time));
    }
    public IEnumerator StartFreezeFrameTime(float time)
    {
        model._Animator.speed = 0;
        yield return new WaitForSeconds(time);
        model._Animator.speed = 1;
    }



    /// <summary>
    /// 游戏停止
    /// </summary>
    /// <param name="force"></param>
    public void DoFreezeGame(float time)
    {
        StartCoroutine(StartFreezeGameTime(time));
    }
    public IEnumerator StartFreezeGameTime(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }



    






    



   



    public void OnSkillOver()
    {
        CanSwitchSkill = true;
    }

    public void OnFootStep()
    {
        throw new System.NotImplementedException();
    }
    #endregion
    
    #region 通用功能
     //角色基础属性
        #region ****************地面检测函数****************

        public bool GroundDetection()
        {
            Vector3 detectionPosition = new Vector3(transform.position.x,
                transform.position.y - detectionGroundPositionOffset,transform.position.z);
            return Physics.CheckSphere(detectionPosition, detectionGroundRange, whatIsGround, QueryTriggerInteraction.Ignore);
        }

        #endregion


        #region  ****************重力应用函数****************

        public void CharacterApplyGravity()
        {  
            characterOnGround = GroundDetection();
            if (characterOnGround)
            {
                fallOutDeltaTime = fallOutTime;
                if (characterVerticalVelocity < 0)
                    characterVerticalVelocity = -2f;
                else
                {
                    characterVerticalVelocity += gravity * Time.deltaTime;
                }
            }
            else
            {
                if (fallOutDeltaTime > 0)
                    fallOutDeltaTime -= Time.deltaTime;
                else
                {
                    //可以执行一些动画播放的效果
                }

                if (characterVerticalVelocity < characterVerticalMaxVelocity && isEnabelGravity)
                    characterVerticalVelocity += gravity * Time.deltaTime;
                else
                    characterVerticalVelocity = characterVerticalMaxVelocity;
            }
        }

        public void UpdateCharacterGravity()
        {
            if(!isEnabelGravity) return;   
            characterVelocityDerection.Set(0,characterVerticalVelocity,0);
            _characterController.Move(characterVelocityDerection*Time.deltaTime);
    }
        
        //坡道检测
        protected Vector3 SlopResetDirction(Vector3 moveDerection)
        {
            if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), Vector3.down, out var hitInfo,
                    _characterController.height * 0.85f))
            {
                if (Vector3.Dot(Vector3.up, hitInfo.normal) != 0)
                {
                    return moveDerection = Vector3.ProjectOnPlane(moveDerection,hitInfo.normal);
                }
            }
            return moveDerection;
        }

        #endregion

        #region  ****************角色移动方向计算****************

        protected void CharacterMovementDirction(Vector3 dirction)
        {
            movementDirection = SlopResetDirction(dirction);
            //_characterController.Move(movementDirection);
        }

        #endregion

       #region ****************注册事件相关****************

       private void ChangeCharacterVerticalVelocity(float targetVelocity)
       {
           characterVerticalVelocity = targetVelocity;
       }

       protected void EnableCharacterGravity(bool enable)
       {
           
           isEnabelGravity = enable;
           characterVerticalVelocity = enable ? -9.8f : 0;
       }
       #endregion


        #region ****************可视化相关****************

        private void OnDrawGizmos()
        {
            Vector3 detectionPosition = new Vector3(transform.position.x,
                transform.position.y - detectionGroundPositionOffset,transform.position.z);
            Gizmos.color = characterOnGround? Color.red : Color.green;
            Gizmos.DrawWireSphere(detectionPosition, detectionGroundRange);
        }

        #endregion
    

    #endregion

    #region 动画播放相关
    
    private string currentAnimationName;
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName">动画名字</param>
    /// /// <param name="reState">当播放的动画名字相同时是否选择刷新播放</param>
    /// <param name="fixedTransitionDuration">过渡时间</param>
    public void PlayAnimation(string animationName, bool reState = true, float fixedTransitionDuration = 0.25f)
    {
        var currentState = Model._Animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName(animationName) && !reState)
        {

            return;
        }
        currentAnimationName = animationName;
        Model._Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }



    /// <summary>
    /// 检查当前是否正在播放指定动画
    /// </summary>
    public bool IsPlayingAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = Model._Animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }

    /// <summary>
    /// 强制动画过渡
    /// </summary>
    public void ForceAnimationTransition(string targetAnimation, float transitionDuration = 0.1f)
    {
        Model._Animator.CrossFade(targetAnimation, transitionDuration);
    }

    #endregion
    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null)
            audioSource.PlayOneShot(audioClip);
    }

    
    
    
}
