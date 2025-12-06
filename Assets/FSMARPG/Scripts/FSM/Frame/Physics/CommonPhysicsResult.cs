using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CommonPhysicsResult : MonoBehaviour
{
    //地面检测
    private bool _isGrounded;
    [Header("地面检测参数")]
    [SerializeField] private float _checkRange = 0.14f;
    [SerializeField] private float _checkOffset = -0.08f;
    [SerializeField] private LayerMask _checkLayer;


    //重力影响
    [Header("重力影响")]
    private readonly float _gravity = -9.8f;
    private float _verticalVelocity;
    private float _fallOutTiem = 0.15f;
    private float _fallOutDeltaTime;
    private float _verticalMaxVelocity = 54.0f;
    private Vector3 _characterVerticalDrection;

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _fallOutDeltaTime = Time.deltaTime;
    }


    private void Update()
    {
        CharacterGravity();
        UpdateCharactorGravity();
    }



    #region 地面检测
    private bool GroundCheck()
    {
        Vector3 checkCenterPos = new Vector3(
            transform.position.x, transform.position.y - _checkOffset, transform.position.z);

        return Physics.CheckSphere(checkCenterPos, _checkRange,_checkLayer,QueryTriggerInteraction.Ignore);
    }
    #endregion

    private Vector3 SlopCheck(Vector3 moveDirection)
    {
        if(Physics.Raycast(transform.position + (transform.up*0.5f),
            Vector3.down,out RaycastHit hit,_controller.height*0.85f, _checkLayer))
        {
            if(Vector3.Dot(hit.normal,Vector3.up) != 0)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                return moveDirection;
            }
            
        }
        return moveDirection;
    }


    //重力应用
    private void CharacterGravity()
    {
        _isGrounded = GroundCheck();

        if (_isGrounded)
        {
            _fallOutDeltaTime = _fallOutTiem;
            _verticalVelocity = -2.0f;
            
        }
        else
        {
            if(_fallOutDeltaTime > 0)
            {
                _fallOutDeltaTime -= Time.deltaTime;
            }
            else
            {
                //说明下落时间大于0.15f，则可以进行动画播放等操作
            }

            if(_verticalVelocity < _verticalMaxVelocity)
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }
        }
    }

    private void UpdateCharactorGravity()
    {
       _characterVerticalDrection.Set(0, _verticalVelocity, 0);
        _controller.Move(_characterVerticalDrection*Time.deltaTime);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.red : Color.green;
        Vector3 checkPosition = new Vector3(transform.position.x,
            transform.position.y -
            _checkOffset, transform.position.z);
        Gizmos.DrawWireSphere(checkPosition, _checkRange);
    }
}
