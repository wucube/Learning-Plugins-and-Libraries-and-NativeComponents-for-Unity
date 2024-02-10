using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;

public class TweenProvider : MonoBehaviour
{
    [SerializeField] private Transform _transCube;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    [Button("使用通用方法改变位置")]
    private void ChangePos(Vector3 changeRate,Vector3 targetValue)
    {
        DOTween.To(() => _transCube.position, changeRate=> _transCube.position = changeRate, targetValue, 1);
    }

    [Button("使用通用方法改变旋转")]
    private void ChangeRot(Vector3 changeRate,Vector3 targetValue)
    {
        DOTween.To(() => _transCube.rotation, changeRate => _transCube.rotation = changeRate, targetValue, 3);
    }

    [Button("使用通用方法改变缩放")]
    private void ChangeScale(Vector3 changeRate,Vector3 targetValue)
    {
        DOTween.To(() => _transCube.localScale, changeRate => _transCube.localScale = changeRate, targetValue, 4);
    }
}
