using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ForgetSample : MonoBehaviour
{
    public Button StarButton;

    public GameObject FirstTarget;
    public GameObject SecondTarget;

    public const float GRAVITY = 9.8f;

    public float FirstFallTime = 2f;
    public float SecondFallTime = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        StarButton.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        //同步方法中调用异步方法，不用等待的时候就调用 Forget 方法。UniTask 执行 Forget 的时候毋需等待直接执行
        /*Fire And Forget——UniTask针对在同步方法中调用异步方法的解决方案
         * 发出事件——Fire Event
         * 之后不管这个事件——Forget
         * 事件会自动执行该做的行为
         */
        FallTarget(FirstTarget.transform, FirstFallTime).Forget();
        FallTarget(SecondTarget.transform, SecondFallTime).Forget();
    }
    
    /// <summary>
    /// 对指定的 Transform 做自由落体运动
    /// </summary>
    /// <param name="targetTrans"></param>
    /// <param name="fallTime"></param>
    private async UniTaskVoid FallTarget(Transform targetTrans, float fallTime)
    {
        float startTime = Time.time;

        Vector3 startPosition = targetTrans.position;
        while (Time.time-startTime<=fallTime)
        {
            //流逝的时间
            float elapsedTime = Mathf.Min(Time.time - startTime, fallTime);
            float fallY = 0 + 0.5f * GRAVITY * elapsedTime * elapsedTime;
            targetTrans.position = startPosition + Vector3.down * fallY;
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
    }
}