using UnityEngine;

namespace MyAssets.Scripts.Tools
{
    public static class MyTools
    {
        /// <summary>
        /// 不受帧数影响的线性插值
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float UnTetheredLerp(float time = 10f)
        {
            return 1 - Mathf.Exp(-time * Time.deltaTime);
        }

        /// <summary>
        /// 取目标方向(从self到target)
        /// 返回一个标准化的方向向量
        /// </summary>
        /// <param name="target"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 DirectionForTarget(Transform target, Transform self)
        {
            return (self.position - target.position).normalized;
        }

        /// <summary>
        /// 取目标方向(从self到target)
        /// 返回于目标之间的距离
        /// </summary>
        /// <param name="target"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float DistanceFromTarget(Transform target, Transform self)
        {
            return Vector3.Distance(self.position, target.position);
        }

        /// <summary>
        /// 获取增量角度
        /// </summary>
        /// <param name="currentDirection"></param>
        /// <param name="targetDirection"></param>
        /// <returns></returns>
        public static float GetDeltaAngle(Transform currentDirection, Vector3 targetDirection)
        {
            //当前角色的朝向
            var angleCurrent = Mathf.Atan2(currentDirection.forward.x, currentDirection.forward.z) * Mathf.Rad2Deg;
            //目标朝向
            var targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            //获取增量角度
            return Mathf.DeltaAngle(angleCurrent, targetAngle);
        }

        /// <summary>
        /// 计算当前朝向和目标方向之间的夹角 
        /// </summary>
        public static float GetAngleForTargetDirection(Transform target, Transform self)
        {
            return Vector3.Angle((self.position - target.position).normalized, self.forward);
        }


        /// <summary>
        /// 计算当前朝向和目标方向之间的夹角 
        /// </summary>
        /// <param name="currentTransform">当前物体的Transform</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>返回角度值(0-180度)</returns>
        public static float CalculateAngleBetweenDirections(Transform currentTransform, Vector3 targetPosition)
        {
            // 获取当前物体的前向向量 
            Vector3 currentForward = currentTransform.forward;

            // 计算指向目标的方向向量 
            Vector3 targetDirection = (targetPosition - currentTransform.position).normalized;

            // 计算并返回夹角 
            return Vector3.Angle(currentForward, targetDirection);
        }

        /// <summary>
        /// 看着目标方向以Y轴为中心
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="target"></param>
        /// <param name="timer">平滑时间(如果是单机某个按键触发那么值最好设置100以上)</param>
        public static void Look(this Transform transform, Vector3 target, float timer)
        {
            // //计算当前物体的前向向量
            // Vector3 currentForward = transform.forward;
            //
            // //计算指向目标的方向向量
            // Vector3 targetDirection = (target - transform.position).normalized;
            //
            // //计算并返回夹角
            // float angle = Vector3.SignedAngle(currentForward, targetDirection, Vector3.up);
            //
            // //平滑旋转
            // transform.Rotate(Vector3.up, angle * Time.deltaTime * timer);
            //
            //
            Vector3 direction = target - transform.position; 
            direction.y = 0;
            
            if (direction.sqrMagnitude  < 0.001f) return;
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation  = Quaternion.Slerp(
                transform.rotation,  
                targetRotation,
                timer * Time.deltaTime 
            );
        }
        
        public static void Look1(this Transform transform, Vector3 target, float time)
        {
            if (time <= 0)
            {
                // 立即转向 
                Vector3 direction = (target - transform.position).normalized; 
                direction.y = 0; // 锁定Y轴 
                if (direction != Vector3.zero) 
                {
                    transform.rotation  = Quaternion.LookRotation(direction);
                }
                return;
            }
 
            // 计算目标方向（忽略Y轴差异）
            Vector3 targetDirection = (target - transform.position).normalized; 
            targetDirection.y = 0;
 
            // 当前前向方向（忽略Y轴差异）
            Vector3 currentForward = transform.forward; 
            currentForward.y = 0;
 
            // 计算插值比例（基于时间）
            float t = Mathf.Clamp01(Time.deltaTime  / time);
 
            // 使用Quaternion.Slerp平滑旋转 
            if (targetDirection != Vector3.zero  && currentForward != Vector3.zero) 
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion currentRotation = Quaternion.LookRotation(currentForward);
                transform.rotation  = Quaternion.Slerp(currentRotation, targetRotation, t);
            }
        }
        
        
        
        /// <summary>
        /// 检查Animator当前播放的动画是否带有指定标签 
        /// </summary>
        /// <param name="animator">目标Animator组件</param>
        /// <param name="tag">要检查的标签名称</param>
        /// <param name="layerIndex">动画层索引（默认为0）</param>
        /// <returns>如果当前动画状态带有指定标签则返回true</returns>
        public static bool AnimationAtTag(this Animator animator, string tag, int layerIndex = 0)
        {
            if (animator == null)
            {
                Debug.LogError("Animator为空！");
                return false;
            }
 
            if (!animator.isActiveAndEnabled) 
            {
                Debug.LogWarning("Animator未激活或已禁用");
                return false;
            }
 
            if (layerIndex < 0 || layerIndex >= animator.layerCount) 
            {
                Debug.LogError($"无效的层级索引：{layerIndex}");
                return false;
            }
 
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            return stateInfo.IsTag(tag);
        }
        
        
    }
}