using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UniTaskUsageSample.Base
{
    public class CallbackSample : MonoBehaviour
    {
        public Button CallbackButton;

        public GameObject Target;

        public const float GRAVITY = 9.8f;

        public float FallTime = 0.5f;

        void Start()
        {
            CallbackButton.onClick.AddListener(UniTask.UnityAction(OnClickCallback));
        }
        
        private async UniTaskVoid OnClickCallback()
        {
            float time = Time.time;
            /*手动控制 UniTask 的执行完成*/
            // UniTaskCompletionSource 可以创建出 Token, 对 UniTask 的手动控制
            // 只要执行 UniTaskCompletionSource.TrySetResult, Source 封装的UniTask就执行完成
            // UniTaskCompletionSource 能精准控制 UniTask 中的执行完成时机
            UniTaskCompletionSource source = new UniTaskCompletionSource();
            FallTarget(Target.transform,FallTime,OnTargetHalf,source).Forget();
            await source.Task;//UniTaskCompletionSource 产生的 UniTask 是可以复用的
            Debug.Log($"当前缩放{Target.transform.localScale} 耗时{Time.time - time}秒");
        }

        private async UniTask FallTarget(Transform targetTrans, float fallTime, Action onHalf,
            UniTaskCompletionSource source)
        {
            float startTime = Time.time;

            Vector3 startPosition = targetTrans.position;
            float lastElapsedTime = 0;
            while (Time.time - startTime<=fallTime)
            {
                float elapsedTime = Mathf.Min(Time.time - startTime, fallTime);
                if (lastElapsedTime < fallTime * 0.5f && elapsedTime >= fallTime * 0.5f)
                {
                    onHalf?.Invoke();
                    // UniTaskCompletionSource 是对 UniTask 的手动封装
                    // 调用 TrySetResult 方法时，source 中的 UniTask 直接完成
                    source.TrySetResult();
                    
                    //失败
                    //source.TrySetException(new SystemException());
                    //取消
                    //source.TrySetCanceled(someToken);
                    
                    //泛型类 UniTaskCompletionSource<T> SetResult 是 T 类型，返回 UniTask<T>
                }
                
                lastElapsedTime = elapsedTime;
                float fallY = 0 + 0.5f * GRAVITY * elapsedTime * elapsedTime;
                targetTrans.position = startPosition + Vector3.down * fallY;
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            }
        }
        
        private void OnTargetHalf()
        {
            Target.transform.localScale *= 1.5f;
        }
    }
}


