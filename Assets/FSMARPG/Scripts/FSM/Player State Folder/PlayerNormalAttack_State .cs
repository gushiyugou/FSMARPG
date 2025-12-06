using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;


public class PlayerNormalAttack_State : PlayerStateBase
{
    
    private int currentAttackIndex = 0;
    private int lastAttackIndex = -1;
    private NormalAttackActionList_SO attackLIst;
    private NormalAttackAction_SO currentAction;
    private float rotationSpeed = 5f;
    


    public override void Enter()
    {
        currentAttackIndex = 0;
        lastAttackIndex = -1;
        attackLIst = player.normalAttackActionList;
        player.Model.SetRootMotionAction(OnRootMotion);
        PlayAttack(currentAttackIndex);

    }

    private bool PlayAttack(int index)
    {
        currentAction = attackLIst.GetAction(index);
        if (currentAction == null) return false;
        player.anbiAudioSource.clip = currentAction.AttackSound;
        player.anbiAudioSource.Play();
        player.PlayAnimation(currentAction.AnimationName, fixedTransitionDuration:0);
        //TODO:技能特效和音效
        player.SpawnNormalAttackEffect(attackLIst.GetAction(currentAttackIndex));
        player.canSwitch = false;
        lastAttackIndex = currentAttackIndex;
        currentAttackIndex++;
        if(currentAttackIndex >= attackLIst.ActionCount())
        {
            lastAttackIndex = attackLIst.ActionCount() - 1;
            currentAttackIndex = 0;
        }
        return true;
    }

    public override void Update() 
    {
       
        if (GameInputManager.Instance.IsLAttackPressed() && player.canSwitch)
        {
            //TODO:技能变招逻辑
            //if (GameInputManager.Instance.IsLAttackHeld())
            //{
            //    currentAttackIndex = attackLIst.ActionCount() -1;
            //}
            PlayAttack(currentAttackIndex);
        }
        //TODO:技能变招切换
        ModelRotaion();
        if (!GameInputManager.Instance.AnyAttackPressed &&
            CheckCurrentAnimatorStateTime()>=0.9f)
        {
            //if(currentAttackIndex >= attackLIst.ActionCount())
            //    player.currentAttackIndex = currentAttackIndex - 1;
            //else
            player.lastAttackIndex = lastAttackIndex;
            player.currentAttackIndex = currentAttackIndex - 1;
            player.ChangeState(PlayerStateType.NormalAttackStop);
        }
    }

    private void ModelRotaion()
    {
        //TODO: 当存在敌人时，应该自动转向敌人，玩家无法控制移动
        if(player.canSwitch && GameInputManager.Instance.IsMoving) return;
        Vector3 input = new Vector3(GameInputManager.Instance.MovementInput.x, 0
            , GameInputManager.Instance.MovementInput.y);
        if (input == Vector3.zero) return;
        float y = Camera.main.transform.eulerAngles.y;
        Vector3 targetDirection = Quaternion.Euler(0, y, 0) * input;
        float angle = Vector3.Angle(player.transform.forward, input);

        player.Model.transform.rotation = Quaternion.Slerp(
            player.Model.transform.rotation, Quaternion.LookRotation(targetDirection),
            Time.deltaTime * rotationSpeed);
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
