using AG.Tool;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;


public class PlayerMove_State : PlayerStateBase
{
    private enum MoveChildState
    {
        WalkStart,
        Walk,
        RunStart,
        Run,
        TurnBack
    }

    private AnimatorStateInfo currentAnimationStateInfo;
    private bool isRun;
    float rotationSpeed;
    private bool isTurningBack;
    private bool isChange;

    private MoveChildState currentChildState;

    private MoveChildState CurrentChildState
    {
        get => currentChildState;
        set
        {
            currentChildState = value;
            switch (currentChildState)
            {
                case MoveChildState.WalkStart:
                    player.PlayAnimation("WalkStart");
                    break;
                case MoveChildState.Walk:
                    player.PlayAnimation("Walk",true,0.13f);
                    break;
                case MoveChildState.RunStart:
                    player.PlayAnimation("RunStart");
                    break;
                case MoveChildState.Run:
                    player.PlayAnimation("Run",true,0.13f);
                    break;
                case MoveChildState.TurnBack:
                    player.PlayAnimation("TurnBack", fixedTransitionDuration:0.1f);
                    isTurningBack = true; // 重置转身标记
                    isChange = false;
                    break;
            }
        }
    }
    public override void Enter()
    {
        player.Model._Animator.applyRootMotion = false;
        player.Model.SetRootMotionAction(OnRootMotion);
        isRun = GetRunState();
        isTurningBack = false; // 重置转身标记
        isChange = true;
        CurrentChildState = isRun ? MoveChildState.RunStart : MoveChildState.WalkStart;
        currentAnimationStateInfo = GetCurrentStateInfo();
    }


    public override void Update()
    {
        currentAnimationStateInfo = GetCurrentStateInfo();

        #region 转身检测
        if (GameInputManager.Instance.IsMoving &&
            !currentChildState.Equals(MoveChildState.TurnBack) &&
            !isTurningBack)
        {
            if (ShouldTurnBack())
            {
                CurrentChildState = MoveChildState.TurnBack;
                return; 
            }
        }
        #endregion


        #region 子状态状态切换
        if (!GameInputManager.Instance.MovementStopped)
        {
            if (GameInputManager.Instance.IsEvadePressed())
            {
                player.isEvadeBack = false;
                player.ChangeState(PlayerStateType.Evade);
            }

            switch (currentChildState)
            {
                case MoveChildState.WalkStart:
                    if (!GetRunState())
                    {
                        if (currentAnimationStateInfo.IsName("WalkStart") && currentAnimationStateInfo.normalizedTime > 0.56f)
                            CurrentChildState = MoveChildState.Walk;
                    }
                    else
                    {
                        if (currentAnimationStateInfo.IsName("WalkStart") && currentAnimationStateInfo.normalizedTime > 0.56f)
                            CurrentChildState = MoveChildState.Run;
                    }
                    break;
                case MoveChildState.Walk:
                    if (GetRunState())
                    {
                        if (currentAnimationStateInfo.IsName("Walk") && currentAnimationStateInfo.normalizedTime >= 0.79f)
                            CurrentChildState = MoveChildState.Run;
                    }
                    break;
                case MoveChildState.RunStart:
                    if (GetRunState())
                    {
                        if (currentAnimationStateInfo.IsName("RunStart") && currentAnimationStateInfo.normalizedTime > 0.58f)
                            CurrentChildState = MoveChildState.Run;
                    }
                    break;
                case MoveChildState.Run:
                    if (!GetRunState() && isChange)
                    {
                        if (currentAnimationStateInfo.IsName("Run") && currentAnimationStateInfo.normalizedTime >= 0.7f)
                            CurrentChildState = MoveChildState.Walk;
                    }
                    break;
                case MoveChildState.TurnBack:
                     if (currentAnimationStateInfo.IsName("TurnBack") && currentAnimationStateInfo.normalizedTime >= 0.6f)
                         CurrentChildState = MoveChildState.Run;
                    break;

            }
            HandleRotation();
        }
        else
        {
            if (GameInputManager.Instance.IsEvadePressed())
            {
                player.isEvadeBack = true;
                player.ChangeState(PlayerStateType.Evade);
            }


            switch (currentChildState)
            {
                case MoveChildState.WalkStart:
                case MoveChildState.Walk:
                case MoveChildState.RunStart:
                case MoveChildState.Run:
                case MoveChildState.TurnBack:
                    player.ChangeState(PlayerStateType.MoveStop, true);
                    break;
            }
        }
        #endregion


        #region 攻击切换
        if (GameInputManager.Instance.IsLAttackPressed())
        {
            player.ChangeState(PlayerStateType.NormalAttack);
        }

        #endregion


    }

    public override void LateUpdate()
    {
        
    }

    private void OnTurnBackCheck()
    {

    } 

    private void HandleRotation()
    {
        //Vector3 input = new Vector3(GameInputManager.Instance.MovementInput.x,
        //    0, GameInputManager.Instance.MovementInput.y);
        //float y = Camera.main.transform.eulerAngles.y;

        ////让四元数与向量相乘，表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
        //Vector3 targetDiretion = Quaternion.Euler(0, y, 0) * input;
        //player.Model.transform.rotation = Quaternion.Slerp(
        //    player.Model.transform.rotation, Quaternion.LookRotation(targetDiretion),
        //    Time.deltaTime * player.rotationSpeed);



        //1.获取输入向量：
        //2.得到相机eulerAngles的y值
        //3.获取目标向量：通过四元素转换得到
        //4.将得到的向量引用到旋转上
        Vector3 input = new Vector3(GameInputManager.Instance.MovementInput.x, 0
            , GameInputManager.Instance.MovementInput.y);
        float y = Camera.main.transform.eulerAngles.y;
        Vector3 targetDirection = Quaternion.Euler(0, y, 0) * input;
        float angle = Vector3.Angle(player.transform.forward, input);

        player.Model.transform.rotation = Quaternion.Slerp(
            player.Model.transform.rotation,Quaternion.LookRotation(targetDirection),
            Time.deltaTime*player.rotationSpeed);
    }


    private bool GetRunState()
    {
        return GameInputManager.Instance.GetRunState().Pressed|| GameInputManager.Instance.GetRunState().Held;
    }

    private bool ShouldTurnBack()
    {
        Vector3 input = new Vector3(GameInputManager.Instance.MovementInput.x, 0, GameInputManager.Instance.MovementInput.y);
        float cameraY = Camera.main.transform.eulerAngles.y;
        Vector3 targetDirection = Quaternion.Euler(0, cameraY, 0) * input;

        float signedAngle = Vector3.SignedAngle(player.Model.transform.forward, targetDirection, Vector3.up);
        return Mathf.Abs(signedAngle) > 135f;
    }

    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
    }


    private AnimatorStateInfo GetCurrentStateInfo()
    {
        return player.Model._Animator.GetCurrentAnimatorStateInfo(0);
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        //player.transform.rotation *= deltaRotation;
        player.characterController.Move(deltaPosition);

        // 强制同步模型位置到控制器
        //player.Model.transform.localPosition = Vector3.zero;
        //player.characterController.Move(deltaPosition);
    }
}
