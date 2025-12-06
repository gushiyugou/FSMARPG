using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNormalAttackStop_State : PlayerStateBase
{
    private string animationName;
    public override void Enter()
    {
        base.Enter();
        if(player.currentAttackIndex == -1)
            animationName = player.normalAttackActionList.GetAction(player.lastAttackIndex).AttackAnimationEndName;
        else
            animationName = player.normalAttackActionList.GetAction(player.currentAttackIndex).AttackAnimationEndName;
        player.PlayAnimation(animationName);
        player.Model.SetRootMotionAction(OnRootMotion);
    }


    public override void Update()
    {
        player.CharacterApplyGravity();
        player.UpdateCharacterGravity();
        if (GameInputManager.Instance.MovementInput != Vector2.zero && 
            player.characterOnGround)
        {
            player.ChangeState(PlayerStateType.Move);
        }

        if (GameInputManager.Instance.IsLAttackPressed())
        {
            player.ChangeState(PlayerStateType.NormalAttack);
        }

        if (GameInputManager.Instance.IsEvadePressed())
        {
            player.isEvadeBack = true;
            player.ChangeState(PlayerStateType.Evade);
        }

        if(!GameInputManager.Instance.AnyMovementKeyHeld && !GameInputManager.Instance.AnyButtonActive && 
            CheckAnimatorStateName(animationName,out float time) && time >0.75f)
        {
            player.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        player.characterController.Move(deltaPosition);
    }
}
