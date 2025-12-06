using System;
using System.Collections;
using System.Collections.Generic;
using AG.Tool;
using UnityEngine;


public class TP_CameraControl : MonoBehaviour
{
    
    [Header("相机参数配置")] 
    [SerializeField] private float _controlSpeed;
    [SerializeField] private Vector2 _cameraVerticalMaxAngle;//限制相机竖直方向上的最大角度
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private float _cameraOffset;


    private Vector2 _input;
    private Vector3 _cameraRotation;
    [SerializeField]private float _smoothTime;
    [SerializeField] private float _positionSmoothTime;
    private Vector3 _smoothDampVelocity = Vector3.zero;


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraInput();
    }

    private void LateUpdate()
    {
        UpdateCameraRotation();
        UpdateCameraPosition();
    }

    private void CameraInput()
    {
        //当鼠标左右移动时，相机应该绕y轴旋转，当鼠标上下移动时，相机应该绕x轴上下旋转，鼠标向上移动时，旋转方向是顺时针旋转
        //所以当鼠标向上移动时，x轴时增加的，向下旋转，向下移动时，x减小，向上看，方向与视觉上相反，应该取反。
        _input.y += GameInputManager.Instance.CameraLook.x * _controlSpeed;
        _input.x -= GameInputManager.Instance.CameraLook.y * _controlSpeed;
        _input.x = Mathf.Clamp(_input.x,_cameraVerticalMaxAngle.x,_cameraVerticalMaxAngle.y);
    }

    private void UpdateCameraRotation()
    {
        _cameraRotation = Vector3.SmoothDamp(_cameraRotation, new Vector3(_input.x, _input.y, 0),
            ref _smoothDampVelocity, _smoothTime);
        transform.eulerAngles = _cameraRotation;
    }

    private void UpdateCameraPosition()
    {
        Vector3 cameraNewPosition = _lookTarget.position + (-transform.forward * _cameraOffset);
        // transform.position = Vector3.Lerp(transform.position, cameraNewPosition, _positionSmoothTime * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, cameraNewPosition, DevelopmentToos.UnTetheredLerp(_positionSmoothTime));
    }
}
