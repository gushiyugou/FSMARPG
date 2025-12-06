using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdle_State : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        base.Enter();
        player.PlayAnimation("Idle");
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
