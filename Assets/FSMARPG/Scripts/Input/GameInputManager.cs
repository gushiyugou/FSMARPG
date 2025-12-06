using System;
using System.Collections;
using System.Collections.Generic;
using AG.Tool.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : SingletonMono<GameInputManager>
{
    private GameInputAction _gameInputAction;

    // 按钮状态类
    [System.Serializable]
    public class ButtonState
    {
        public bool Pressed { get; set; } //当前帧按下（当前帧有效，当前帧结束后变false）
        public bool Held { get; set; }    // 按住（按住期间都有效）
        public bool Released { get; set; }// 当前帧抬起（当前帧有效，当前帧结束后变false）

        public bool Active => Pressed || Held; 

        public override string ToString()
        {
            return $"Pressed: {Pressed}, Held: {Held}, Released: {Released}";
        }
    }

    //移动状态类
    [SerializeField]
    public class MovementState
    {
        public Vector2 CurrentInput { get;set; }
        public Vector2 PreviousInput { get;set; }
        public bool StartMoveing { get;set; }
        public bool Moveing { get;set; }
        public bool StopMoveing { get;set; }
        public float Magnitude =>CurrentInput.magnitude;


        public void Update(Vector2 newInput)
        {
            PreviousInput = CurrentInput;
            CurrentInput = newInput;

            float threshold = 0.1f;
            bool wasMoving = PreviousInput.magnitude > threshold;
            bool isMoving = CurrentInput.magnitude > threshold;

            StartMoveing = !wasMoving && isMoving;
            Moveing = wasMoving && isMoving;
            StopMoveing = wasMoving && !isMoving;
        }
    }

    #region 输入动作包装器
    private class InputActionWrapper
    {
        private InputAction _action;
        private ButtonState _state = new ButtonState();
        public InputActionWrapper(InputAction action)
        {
            _action = action;
        }

        public ButtonState GetState()
        {
            // 更新状态
            bool wasHeld = _state.Held;
            _state.Held = _action.ReadValue<float>() > 0.1f;
            // 按下：当前帧按住但上一帧没按住
            _state.Pressed = _state.Held && !wasHeld;
            // 抬起：当前帧没按住但上一帧按住
            _state.Released = !_state.Held && wasHeld;
            return _state;
        }

        // 便捷方法
        public bool IsPressed() => GetState().Pressed;
        public bool IsHeld() => GetState().Held;
        public bool IsReleased() => GetState().Released;
    }

    // 输入动作包装器实例
    private MovementState _movementState = new MovementState();
    private InputActionWrapper _runWrapper;
    private InputActionWrapper _lAttackWrapper;
    private InputActionWrapper _rAttackWrapper;
    private InputActionWrapper _climbWrapper;
    private InputActionWrapper _grabWrapper;
    private InputActionWrapper _takeOutWrapper;
    private InputActionWrapper _evadeWrapper;

    #endregion



    #region 向后兼容的triggered属性
    public bool Run => _gameInputAction.GameInput.Run.triggered;
    public bool LAttack => _gameInputAction.GameInput.LAttack.triggered;
    public bool RAttack => _gameInputAction.GameInput.RAttack.triggered;
    public bool Climb => _gameInputAction.GameInput.Climb.triggered;
    public bool Grab => _gameInputAction.GameInput.Grab.triggered;
    public bool TakeOut => _gameInputAction.GameInput.TakeOut.triggered;
    public bool Evade => _gameInputAction.GameInput.Evade.triggered;

    #endregion


    #region 移动状态属性
    public Vector2 MovementInput => _gameInputAction.GameInput.Movement.ReadValue<Vector2>();
    public Vector2 CameraLook => _gameInputAction.GameInput.CameraLook.ReadValue<Vector2>();
    public MovementState Movement => _movementState;
    public bool MovementStarted => _movementState.StartMoveing;
    public bool IsMoving => _movementState.Moveing;
    public bool MovementStopped =>_movementState.StopMoveing;

    #endregion


    #region 生命周期函数相关
    protected override void Awake()
    {
        base.Awake();
        _gameInputAction ??= new GameInputAction();

        // 初始化包装器
        _runWrapper = new InputActionWrapper(_gameInputAction.GameInput.Run);
        _lAttackWrapper = new InputActionWrapper(_gameInputAction.GameInput.LAttack);
        _rAttackWrapper = new InputActionWrapper(_gameInputAction.GameInput.RAttack);
        _climbWrapper = new InputActionWrapper(_gameInputAction.GameInput.Climb);
        _grabWrapper = new InputActionWrapper(_gameInputAction.GameInput.Grab);
        _takeOutWrapper = new InputActionWrapper(_gameInputAction.GameInput.TakeOut);
        _evadeWrapper = new InputActionWrapper(_gameInputAction.GameInput.Evade);
    }

    private void Update()
    {
        _movementState.Update(MovementInput);
    }

    private void OnEnable()
    {
        _gameInputAction.Enable();
    }

    private void OnDisable()
    {
        _gameInputAction.Disable();
    }

    #endregion 




    // === 统一的状态获取接口 ===

    #region 获取完整的按钮状态对象
    public ButtonState GetRunState() => _runWrapper.GetState();
    public ButtonState GetLAttackState() => _lAttackWrapper.GetState();
    public ButtonState GetRAttackState() => _rAttackWrapper.GetState();
    public ButtonState GetClimbState() => _climbWrapper.GetState();
    public ButtonState GetGrabState() => _grabWrapper.GetState();
    public ButtonState GetTakeOutState() => _takeOutWrapper.GetState();
    public ButtonState GetEvadeState() => _evadeWrapper.GetState();

    #endregion

    #region 获取按钮按下状态的API
    public bool IsRunPressed() => _runWrapper.IsPressed();
    public bool IsLAttackPressed() => _lAttackWrapper.IsPressed();
    public bool IsRAttackPressed() => _rAttackWrapper.IsPressed();
    public bool IsClimbPressed() => _climbWrapper.IsPressed();
    public bool IsGrabPressed() => _grabWrapper.IsPressed();
    public bool IsTakeOutPressed() => _takeOutWrapper.IsPressed();
    public bool IsEvadePressed() => _evadeWrapper.IsPressed();

    #endregion


    #region 判断移动输入状态
    public bool WasMovementKeyReleased() => MovementStopped;
    public bool WasMovemntKeyPressed()=>MovementStarted;
    public bool IsMovementHeld() => IsMoving;


    public bool MovedForward => MovementInput.y > 0.1f;
    public bool MovedBacked => MovementInput.y < -0.1f;
    public bool MovedRight => MovementInput.x > 0.1f;
    public bool MovedLeft => MovementInput.x < -0.1f;


    public bool StopMoveForward => MovementInput.y <= 0.1f && _movementState.PreviousInput.y > 0.1f;
    public bool StopMoveBack => MovementInput.y >= -0.1f && _movementState.PreviousInput.y < 0.1f;
    public bool StopMoveRight => MovementInput.x <= 0.1f && _movementState.PreviousInput.x > 0.1f;
    public bool StopMoveLeft => MovementInput.x >= -0.1f && _movementState.PreviousInput.x < 0.1f;

    public bool WasDiagonalMovementReleased()
    {
        return MovementStopped &&
            Mathf.Abs(_movementState.PreviousInput.x) > 0.1f &&
            Mathf.Abs(_movementState.PreviousInput.y) > 0.1f;
    }

    #endregion

    #region 获取按钮按住状态的API
    public bool IsRunHeld() => _runWrapper.IsHeld();
    public bool IsLAttackHeld() => _lAttackWrapper.IsHeld();
    public bool IsRAttackHeld() => _rAttackWrapper.IsHeld();
    public bool IsClimbHeld() => _climbWrapper.IsHeld();
    public bool IsGrabHeld() => _grabWrapper.IsHeld();
    public bool IsTakeOutHeld() => _takeOutWrapper.IsHeld();
    public bool IsEvadeHeld() => _evadeWrapper.IsHeld();

    #endregion


    #region 获取按钮释放释放状态的API
    public bool IsRunReleased() => _runWrapper.IsReleased();
    public bool IsLAttackReleased() => _lAttackWrapper.IsReleased();
    public bool IsRAttackReleased() => _rAttackWrapper.IsReleased();
    public bool IsClimbReleased() => _climbWrapper.IsReleased();
    public bool IsGrabReleased() => _grabWrapper.IsReleased();
    public bool IsTakeOutReleased() => _takeOutWrapper.IsReleased();
    public bool IsEvadeReleased() => _evadeWrapper.IsReleased();

    #endregion


    #region 获取移动方向
    public Vector2 GetCurrentMovementDirection()
    {
        return MovementInput.normalized;
    }


    public Vector2 GetPreviousMovementDirection()
    {
        return _movementState.PreviousInput.normalized;
    }

    #endregion

    #region 获取状态名字
    public ButtonState GetButtonState(string actionName)
    {
        return actionName.ToLower() switch
        {
            "run" => GetRunState(),
            "lattack" => GetLAttackState(),
            "rattack" => GetRAttackState(),
            "climb" => GetClimbState(),
            "grab" => GetGrabState(),
            "takeout" => GetTakeOutState(),
            "evade" => GetEvadeState(),
            _ => throw new ArgumentException($"未知的动作名称: {actionName}")
        };
    }

    // 通用方法：通过InputAction获取状态
    public ButtonState GetButtonState(InputAction action)
    {
        if (action == _gameInputAction.GameInput.Run) return GetRunState();
        if (action == _gameInputAction.GameInput.LAttack) return GetLAttackState();
        if (action == _gameInputAction.GameInput.RAttack) return GetRAttackState();
        if (action == _gameInputAction.GameInput.Climb) return GetClimbState();
        if (action == _gameInputAction.GameInput.Grab) return GetGrabState();
        if (action == _gameInputAction.GameInput.TakeOut) return GetTakeOutState();
        if (action == _gameInputAction.GameInput.Evade) return GetEvadeState();

        throw new ArgumentException("未知的InputAction");
    }

    #endregion

    #region 泛型状态判断相关
    public bool AnyAttackPressed => IsLAttackPressed() || IsRAttackPressed();
    public bool AnyAttackHeld => IsLAttackHeld() || IsRAttackHeld();
    public bool AnyMovementKeyHeld => MovementInput.magnitude > 0.1f;
    public bool AnyButtonActive => IsRunHeld() || IsLAttackHeld() || IsRAttackHeld() ||
                                 IsClimbHeld() || IsGrabHeld() || IsTakeOutHeld() || 
                                IsEvadeHeld();

    #endregion
}