using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerEvade_State : PlayerStateBase
{
    private enum EvadeChildState
    {
        EvadeBack,
        EvadeFront,
    }

    private string animationName;
    private int evadeCount;
    private EvadeChildState currentEvadeState;
    private EvadeChildState CurrentEvadeState
    {
        get => currentEvadeState;
        set
        {
            currentEvadeState = value;
            switch (currentEvadeState)
            {
                case EvadeChildState.EvadeBack:
                    player.PlayAnimation("EvadeBack");
                    break;
                case EvadeChildState.EvadeFront:
                    player.PlayAnimation("EvadeFront");
                    break;
            }
        }
    }

    public override void Enter()
    {
        animationName = player.isEvadeBack ? "EvadeBack" : "EvadeFront";
        CurrentEvadeState = player.isEvadeBack ? EvadeChildState.EvadeBack : EvadeChildState.EvadeFront;
        player.Model.SetRootMotionAction(OnRootMotion);
        player.isEvadeBack = false;
    }


    public override void Update() 
    {

        #region 逻辑切换

        switch (CurrentEvadeState)
        {
            case EvadeChildState.EvadeBack:
                if(CheckAnimatorStateName("EvadeBack", out float timeBack))
                {
                    if(timeBack >= 0.25f)
                    {
                        if (GameInputManager.Instance.IsEvadePressed())
                            CurrentEvadeState = EvadeChildState.EvadeBack;
                        if (GameInputManager.Instance.AnyMovementKeyHeld)
                            player.ChangeState(PlayerStateType.Move);
                    }
                    if (timeBack >= 0.35f)
                    {
                        
                        if (!GameInputManager.Instance.AnyAttackPressed && !GameInputManager.Instance.AnyButtonActive
                             && timeBack > 0.75f)
                        {
                            player.ChangeState(PlayerStateType.Idle);
                        }
                    }
                }
                
                break;
            case EvadeChildState.EvadeFront:
                if(CheckAnimatorStateName("EvadeFront",out float timeFront))
                {
                    if (GameInputManager.Instance.AnyMovementKeyHeld && timeFront > 0.5f)
                    {
                        player.ChangeState(PlayerStateType.Move);
                    }

                    


                    if(!GameInputManager.Instance.AnyAttackHeld && !GameInputManager.Instance.AnyButtonActive)
                    {
                        player.ChangeState(PlayerStateType.Idle);
                    }

                }
                
                break;
        }

        #endregion

        #region 无敌帧效果
        //是否可以切换到攻击状态，根据情况来判断
        if (GameInputManager.Instance.AnyAttackHeld)
        {

        }

        #endregion


    }







    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
    }

    private void OnRootMotion(Vector3 deltaPosition,Quaternion deltaRotation)
    {

        player.characterController.Move(deltaPosition);
    }

}
