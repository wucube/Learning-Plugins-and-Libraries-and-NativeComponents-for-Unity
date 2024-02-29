using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TweenProvider : MonoBehaviour
{
    [SerializeField] private Transform _sphereTrans;

    [SerializeField] private Vector3 _curValue = Vector3.one;
    [SerializeField] private Vector3 _tarValue;
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
        DOTween.To(() => transform.position, rate => transform.position = rate, _tarValue, 1);
    }

    [ButtonGroup("GenericWay/通用方法/方法组01")]
    private void ChangeRotation()
    {
        DOTween.To(() => transform.rotation, rate => transform.rotation = rate, _tarValue, 3);
    }

    [ButtonGroup("GenericWay/通用方法/方法组01")]
    private void ChangeScale()
    {
        DOTween.To(() => transform.localScale, rate => transform.localScale = rate, _tarValue, 4);
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
        DOTween.To(() => transform.position, rate => transform.position = rate, _tarValue, 2)
            .SetLoops(3, LoopType.Restart);
    }
    
    [ButtonGroup("SetChain/链式设置/方法组01")]
    private void SetTarget()
    {
        //创建一个在X轴上移动的Tween动画，持续5秒钟，循环执行来回移动，具有一定的延迟和缓动效果
        transform.DOMoveX(20, 5).SetAutoKill(true).SetDelay(3).SetEase(Ease.InOutCirc)  
            .SetId("superTween").SetLoops( -1, LoopType.Yoyo).SetRecyclable()  
            .SetRelative().SetSpeedBased().SetTarget(transform).SetUpdate(UpdateType.Normal, true); 
    }
    
    #endregion
    
    [Button("倒放")]
    private void From()
    {
        transform.DOMoveX(-6, 2).From();
    }
    
    #region 事件响应

    [BoxGroup("EventReactive", ShowLabel = false)]
    [TitleGroup("EventReactive/事件响应")]
    
    [ButtonGroup("EventReactive/事件响应/方法组01")]
    private void OnStart()
    {
        transform.DOMoveX(4, 2).OnStart(()=>Debug.Log("动画补间开始"));
    }

    [ButtonGroup("EventReactive/事件响应/方法组01")]
    private void OnComplete()
    {
        transform.DOMoveX(4, 3).OnComplete(()=>Debug.Log("动画结束"));
    }
    
    #endregion


    #region 常用用法
    [BoxGroup("CommonUsage",ShowLabel = false)]
    [TitleGroup("CommonUsage/常用用法")]
    
    [ButtonGroup("CommonUsage/常用用法/方法组01")]
    private void ImmediatelyComplete()
    {
        //立即完成正在进行的 Tweener 或 Sequence 动画
        //在某些情况下非常有用，例如当你想在某个条件满足时立即停止动画，或者在程序中的特定时间点将动画重置为起始状态。
        //特别是频繁调用动画时，在上一个动画还没有完成的情况下，紧接着就进行下一个动画，会造成了物体大小畸形的问题
        transform.DOComplete();
    }
 
    [ButtonGroup("CommonUsage/常用用法/方法组01")]
    private void Move()
    {
        //使得物体移动，第一个参数类型是Vector3，代表物体要移动到的世界坐标，第二个参数是移动到该位置所需要的时间
        transform.DOMove(new Vector3(5, 9, 1), 6);
    }

    [ButtonGroup("CommonUsage/常用用法/方法组01")]
    private void LocalMoveX()
    {
        //使得物体在x轴进行移动，第一个参数是移动到的x的位置，以自身坐标
        transform.DOLocalMoveX(9, 7);
    }

    [ButtonGroup("CommonUsage/常用用法/方法组02")]
    private void BlendMoveBy()
    {
        //blend对物体的运动进行累加,参数一: 累加到的目标数，参数二：累加所需要的时间
        transform.DOBlendableMoveBy(Vector3.one, 2);
    }
    
    [ButtonGroup("CommonUsage/常用用法/方法组02")]
    private void PunchPosition()
    {
        /*使得物体到达参数一的位置就进行往返运动
         *参数一 能到达的位置
         *参数二 需要的时间
         *参数三 往返的次数
         *参数四 值越大，反方向给的力就越大
         */
        transform.DOPunchPosition(new Vector3(0, 5, 0), 5, 2, 0.5f);
    }

    
    #endregion
    

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
 