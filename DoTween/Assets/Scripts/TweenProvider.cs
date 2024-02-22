using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TweenProvider : MonoBehaviour
{
    [SerializeField] private Transform _sphereTrans;
    [SerializeField] private Vector3 _rate;
    [SerializeField] private Vector3 _target;
    [SerializeField] private Text _text;
    [SerializeField] private Material _material;

    [SerializeField] private Gradient _gradient;
    
    #region 通用方法 
    
    /* DOTween. 起头的方法为通用方法
     * DOTween.To (getter, setter, to, float duration)
     * 参数一 参数二 输入的为代理，移动，旋转，颜色变化，文字变化都可通用。
     * getter 是初始值，setter 是变化的值，to 是目标，duration 是变化时长。
     */
    
    [BoxGroup("GenericWay",ShowLabel = false)]
    [TitleGroup("GenericWay/通用方法")]
    
    [ButtonGroup("GenericWay/通用方法/方法组01")]
    private void ChangePosition()
    {
        DOTween.To(() => transform.position, _rate => transform.position = _rate, _target, 1);
    }

    [ButtonGroup("GenericWay/通用方法/方法组01")]
    private void ChangeRotation()
    {
        DOTween.To(() => transform.rotation, _rate => transform.rotation = _rate, _target, 3);
    }

    [ButtonGroup("GenericWay/通用方法/方法组01")]
    private void ChangeScale()
    {
        DOTween.To(() => transform.localScale, _rate => transform.localScale = _rate, _target, 4);
    }

    #endregion

    #region 便捷方法

    [BoxGroup("ShortcutWay", ShowLabel = false)]
    [TitleGroup("ShortcutWay/快捷方法")]

    [ButtonGroup("ShortcutWay/快捷方法/方法组01")]
    private void SetPosition()
    {
        //Unity 的各种属性.DOXXX
        transform.DOMove(Vector3.right * 3, 2);
    }
    
    [ButtonGroup("ShortcutWay/快捷方法/方法组01")]
    private void SetRotation()
    {
        transform.DORotate(Vector3.right * 100, 2);
    }
    #endregion

    #region 链式设置
    
    /* 常见链式设置
     * SetAutoKill(结束后删除) SetEase(变化程度) SetInverted(反转)
     * SetLoops(循环) SetRelative(本地坐标) SetUpdate(更新函数) SetTarget(设置目标)
     * SetDelay(等待) SetSpeedBased(时间变速度) SetLookAt(朝向对象)
     */

    [BoxGroup("SetChain", ShowLabel = false)]
    [TitleGroup("SetChain/链式设置")]

    [ButtonGroup("SetChain/链式设置/方法组01")]
    private void SetLoop()
    {
        DOTween.To(() => transform.position, _rate => transform.position = _rate, _target, 2)
            .SetLoops(3, LoopType.Restart);
    }
    
    [ButtonGroup("SetChain/链式设置/方法组01")]
    private void SetTarget()
    {
        transform.DOScale(Vector3.one * 4, 1).SetTarget(_sphereTrans);
    }
    
    #endregion
    
    
    [Button("倒放")]
    private void From()
    {
        transform.DOMoveX(-6, 2).From();
    }

    [Button("动画完成后事件响应")]
    private void ChangeTransWithEvent()
    {
        transform.DOMoveX(4, 2).OnStart(OnEvent);
    }

    private void OnEvent()
    {
        Debug.Log("动画补间完成");
    }


    [Button("立即完成动画")]
    private void ImmediatelyCompleteAnimation()
    {
        transform.DOComplete();
    }

    [Button("移动自身坐标")]
    private void MoveBySelf()
    {
        transform.DOLocalMoveX(9, 5);
    }

    [Button("移动累加")]
    private void MoveBlend()
    {
        transform.DOBlendableMoveBy(Vector3.one , 2);
    }

    [Button("来回移动")]
    private void MovePunch()
    {
        transform.DOPunchPosition(new Vector3(5, 0, 0), 2, -1, 1f);
    }

    [Button("旋转")]
    private void Rotate()
    {
        transform.DORotate(new Vector3(0, 90, 0), 3,RotateMode.LocalAxisAdd);
    }

    [Button("围绕自身坐标轴旋转")]
    private void LocalRotate()
    {
        transform.DOLocalRotate(new Vector3(0, 170, 0), 4, RotateMode.WorldAxisAdd);
    }

    [Button("四元数旋转物体")]
    private void RotateQua()
    {
        transform.DORotateQuaternion(Quaternion.Euler(40, 180, 0), 8);
    }

    [Button("跳跃函数")]
    private void Jump()
    {
        transform.DOJump(new Vector3(0, 5, 0), 1, 3, 3).SetEase(Ease.OutBack);
    }

    [Button("渐变透明度")]
    private void Fade()
    {
        _text.DOFade(0, 2f).SetEase(Ease.OutBack); 
    }

    
    [Button("抖动")]
    private void Shake()
    {
        //_transCube.DOShakePosition(1, new Vector3(0.2f, 0.5f, 0), 20, 0f, false);
        
        //_transCube.DOShakeRotation(0.3f, new Vector3(10, 30, 0), 15, 0, true);

        transform.DOShakeScale(0.5f, 0.3f, 10, 0, true);
    }

    #region 材质的 Tween 动画
    [BoxGroup("Material",ShowLabel = false)]
    [TitleGroup("Material/材质")]
    [ButtonGroup("Material/材质/方法组01")]
    private void ChangeMaterialColor()
    {
        //改变材质的颜色
        _material.DOColor(Color.red, 3);
    }

    [ButtonGroup("Material/材质/方法组01")]
    private void GradientMaterialColor()
    {
        //实现渐变:参数一为渐变条，需要提前声明;参数二为持续时间
        _material.DOGradientColor(_gradient, 5f);
    }
    
    [ButtonGroup("Material/材质/方法组02")]
    private void OffsetMaterialTexture2d()
    {
        //改变贴图位置
        _material.DOOffset(Vector3.zero, 2);
    }

    [ButtonGroup("Material/材质/方法组02")]
    private void BlendMaterialColor()
    {
        //将两个材质的颜色混合在一起
        _material.DOBlendableColor(Color.green, 3);
        _material.DOBlendableColor(Color.yellow, 3);
    }
    
    #endregion
    
}
 