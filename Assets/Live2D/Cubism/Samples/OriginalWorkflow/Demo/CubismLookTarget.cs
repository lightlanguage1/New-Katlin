/**
 * 版权所有(c) Live2D Inc. 保留所有权利。
 *
 * 使用此源代码受Live2D开放软件许可证的约束，
 * 可在 https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html 找到。
 */

// 引入必要的命名空间和类
using Live2D.Cubism.Framework.LookAt;
using UnityEngine;

namespace Live2D.Cubism.Samples.OriginalWorkflow.Demo
{
    // CubismLookTarget类继承自MonoBehaviour和ICubismLookTarget接口
    public class CubismLookTarget : MonoBehaviour, ICubismLookTarget
    {
        /// <summary>
        /// 获取鼠标拖拽时的坐标。
        /// </summary>
        /// <returns>鼠标坐标。</returns>
        public Vector3 GetPosition()
        {
            // 如果鼠标左键未被按下，则返回零向量
            if (!Input.GetMouseButton(0))
            {
                return Vector3.zero;
            }

            // 获取鼠标当前位置
            var targetPosition = Input.mousePosition;


           //targetPosition = (Camera.main.ScreenToViewportPoint(targetPosition) * 2) - Vector3.one;

            return targetPosition;
        }

        /// <summary>
        /// 获取目标是否处于活动状态。
        /// </summary>
        /// <returns>如果目标处于活动状态，则返回true；否则返回false。</returns>
        public bool IsActive()
        {
            // 目标始终处于活动状态
            return true;
        }
    }
}
