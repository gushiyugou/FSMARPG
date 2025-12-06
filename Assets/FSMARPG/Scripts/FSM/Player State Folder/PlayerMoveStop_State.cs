using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMoveStop_State:PlayerStateBase
{

    private float time;
    public override void Enter()
    {
        if (GameInputManager.Instance.IsEvadePressed())
        {
            player.isEvadeBack = true;
            player.ChangeState(PlayerStateType.Evade);
        }
        player.Model.SetRootMotionAction(OnRootMotion);
        time = 0.15f;
        player.PlayAnimation("Stop",true,0.2f);
        //player.Model._Animator.Play("Stop");
    }

    public override void Update()
    {

        if (GameInputManager.Instance.IsEvadePressed())
        {
            player.isEvadeBack = true;
            player.ChangeState(PlayerStateType.Evade);
        }

        if (GameInputManager.Instance.IsLAttackPressed())
        {
            player.ChangeState(PlayerStateType.NormalAttack);
        }


        if (time > 0) time -= Time.deltaTime;

        

        if (GameInputManager.Instance.MovementStopped && time >0f)
        {
            player.ChangeState(PlayerStateType.MoveStop);
        }
        else if(GetCurrentStateInfo().IsName("Stop") && GetCurrentStateInfo().normalizedTime > 0.9f)
        {
            player.ChangeState(PlayerStateType.Idle);
        }

        if (GameInputManager.Instance.IsMoving && time <= 0)
        {
            player.ChangeState(PlayerStateType.Move);
        }
        
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

        player.characterController.Move(deltaPosition);
        //player.characterController.Move(deltaPosition);
    }
}
