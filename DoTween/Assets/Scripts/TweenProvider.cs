using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine.UI;

public class TweenProvider : MonoBehaviour
{
    [SerializeField] private Transform _transCube;

    [SerializeField] private Vector3 _rate;
    [SerializeField] private Vector3 _target;
    [SerializeField] private Text _text;
    [SerializeField] private Material _material;

    [SerializeField] private Gradient _gradient;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    [Button("使用通用方法改变位置")]
    private void ChangePos()
    {
        DOTween.To(() => _transCube.position, _rate => _transCube.position = _rate, _target, 1);
    }

    [Button("使用通用方法改变旋转")]
    private void ChangeRot()
    {
        DOTween.To(() => _transCube.rotation, _rate => _transCube.rotation = _rate, _target, 3);
    }

    [Button("使用通用方法改变缩放")]
    private void ChangeScale()
    {
        DOTween.To(() => _transCube.localScale, _rate => _transCube.localScale = _rate, _target, 4);
    }


    [Button("使用便捷方法设置位置")]
    private void SetPos()
    {
        _transCube.DOMove(Vector3.right * 3, 2);
    }

    [Button("使用便捷方法设置旋转")]
    private void SetRot()
    {
        _transCube.DORotate(Vector3.right * 100, 2);
    }

    [Button("使用便捷方法设置缩放")]
    private void SetScale()
    {
        _transCube.DOScale(Vector3.one * 5, 2);
    }

    [Button("改变位置后循环多次")]
    private void ChangePosMoreTime()
    {
        DOTween.To(() => _transCube.position, _rate => _transCube.position = _rate, _target, 2)
            .SetLoops(3, LoopType.Restart);
    }

    [Button("改变缩放后循环多次")]
    private void ChangeScaleMoreTime()
    {
        _transCube.DOScale(Vector3.one * 2, 1).SetTarget(_transCube);
    }


    [Button("其他设置")]
    private void ChangeOther()
    {
        _transCube.DOMove(Vector3.right * 4, 3).SetSpeedBased(false);
    }

    [Button("动画完成后事件响应")]
    private void ChangeTransWithEvent()
    {
        _transCube.DOMoveX(4, 2).OnStart(OnEvent);
    }

    private void OnEvent()
    {
        Debug.Log("动画补间完成");
    }


    [Button("立即完成动画")]
    private void ImmediatelyCompleteAnimation()
    {
        _transCube.DOComplete();
    }

    [Button("移动自身坐标")]
    private void MoveBySelf()
    {
        _transCube.DOLocalMoveX(9, 5);
    }

    [Button("移动累加")]
    private void MoveBlend()
    {
        _transCube.DOBlendableMoveBy(Vector3.one , 2);
    }

    [Button("来回移动")]
    private void MovePunch()
    {
        _transCube.DOPunchPosition(new Vector3(5, 0, 0), 2, -1, 1f);
    }

    [Button("旋转")]
    private void Rotate()
    {
        _transCube.DORotate(new Vector3(0, 90, 0), 3,RotateMode.LocalAxisAdd);
    }

    [Button("围绕自身坐标轴旋转")]
    private void LocalRotate()
    {
        _transCube.DOLocalRotate(new Vector3(0, 170, 0), 4, RotateMode.WorldAxisAdd);
    }

    [Button("四元数旋转物体")]
    private void RotateQua()
    {
        _transCube.DORotateQuaternion(Quaternion.Euler(40, 180, 0), 8);
    }

    [Button("跳跃函数")]
    private void Jump()
    {
        _transCube.DOJump(new Vector3(0, 5, 0), 1, 3, 3).SetEase(Ease.OutBack);
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

        _transCube.DOShakeScale(0.5f, 0.3f, 10, 0, true);
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
 