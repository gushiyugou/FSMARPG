using System;
using System.Collections;
using System.Collections.Generic;
using AG.Tool;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    
    [SerializeField,Header("最大最小偏移量")] private Vector2 _CameraOffsetRange;
    [SerializeField,Header("碰撞检测层级")] private LayerMask _cameraColliderLayer;
    [SerializeField,Header("射线长度")] private float _detectionDistance;
    [SerializeField, Header("平滑时间")] private float _colliderSmootTime;



    private Vector3 _origionPosition;
    private float _origionOffsetDistance;
    private Transform _mainCamera;
    

    private void Awake()
    {
        _mainCamera = Camera.main.transform;
    }

    private void Start()
    {
        _origionPosition = transform.localPosition.normalized;
        _origionOffsetDistance = _CameraOffsetRange.y;
    }

    private void LateUpdate()
    {
        UdpateCameraCollider();
    }


    private void UdpateCameraCollider()
    {
        var detectionDirction = transform.TransformPoint(_origionPosition * _detectionDistance);
        if (Physics.Linecast(transform.position, detectionDirction, out RaycastHit hit, 
                _cameraColliderLayer, QueryTriggerInteraction.Ignore))
        {
            _origionOffsetDistance = Mathf.Clamp((hit.distance * 0.8f), _CameraOffsetRange.x, hit.distance);
        }
        else
        {
            _origionOffsetDistance = _CameraOffsetRange.y;
        }

        _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition,
            _origionPosition * (_origionOffsetDistance - 0.1f), DevelopmentToos.UnTetheredLerp(_colliderSmootTime));
    }
    
}
