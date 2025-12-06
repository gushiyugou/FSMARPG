using UnityEngine;



public class PlayerStateBase : StateBase
{

    protected static bool isAnimationInverted = false;
    protected PlayerController player;
    protected float BaseGravityVelocity = 200;
    //所有派生类都可以获取并赋值，适用于来控制不同状态下的跳跃的距离控制。
    protected static float moveStatePower;
    protected static StateBase previousState;
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        player = owner as PlayerController;
    }

    protected virtual bool CheckAnimatorStateName(string stateName,out float normalizedTime)
    {
        AnimatorStateInfo nextState = player.Model._Animator.GetNextAnimatorStateInfo(0);
        if (nextState.IsName(stateName))
        {
            normalizedTime = nextState.normalizedTime;
            return true;
        }


        AnimatorStateInfo currentState = player.Model._Animator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = currentState.normalizedTime;
        return currentState.IsName(stateName);
    }

    protected virtual float CheckCurrentAnimatorStateTime()
    {
        AnimatorStateInfo currentState = player.Model._Animator.GetCurrentAnimatorStateInfo(0);
        return currentState.normalizedTime;
    }

    //主动运用重力
    protected void UpdataGravity()
    {
        float velocity = 0f;
        if (!player.characterController.isGrounded)
        {
            velocity = player.gravity;
            //_player.characterController.Move(Vector3.down * _player._gravity * Time.deltaTime);
            //Debug.Log(BaseGravityVelocity);
        }
        else
        {
            velocity = 0;
        }

        float displancement = velocity * Time.deltaTime;
        Vector3 verticalDisplancement = new Vector3(0, displancement, 0);
        player.characterController.Move(verticalDisplancement);
    }

    protected void SyncModelToController()
    {
        // 获取控制器位置
        Vector3 controllerPosition = player.transform.position;

        // 同步模型位置到控制器位置
        player.Model.transform.position = controllerPosition;

        //// 如果使用根运动，可能需要禁用或调整
        //_player._PlayerModle._Animator.applyRootMotion = false;
    }

    public static void GetPerviousState(StateBase state)
    {
        previousState = state;
    }

    protected void SetRootAnima(bool setState)
    {
        player.Model._Animator.applyRootMotion = setState;
    }

    protected bool GetRootAnimaState()
    {
        return player.Model._Animator.applyRootMotion;
    }



    #region 获取到鼠标在世界坐标系下的水平方向向量
    protected Vector3 GetMouseWorldDirection()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.Model.transform.position);
        float enter;

        if (groundPlane.Raycast(mouseRay, out enter))
        {
            Vector3 worldPoint = mouseRay.GetPoint(enter);
            Vector3 direction = (worldPoint - player.Model.transform.position).normalized;
            direction.y = 0;
            return direction;
        }

        return player.Model.transform.forward;
    }


    


    #endregion
}
