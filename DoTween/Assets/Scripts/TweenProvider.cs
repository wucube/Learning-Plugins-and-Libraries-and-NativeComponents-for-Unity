using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;

public class TweenProvider : MonoBehaviour
{
    [SerializeField] private Transform _transCube;

    [SerializeField] private Vector3 _rate;
    [SerializeField] private Vector3 _target;

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

}
 