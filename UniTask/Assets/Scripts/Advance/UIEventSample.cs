using System;
using System.Threading;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UniTaskUsageSample.Advance
{
    public class UIEventSample : MonoBehaviour
    {
        public float CheckDoubleClickTime = 0.5f;
        
        public Button DoubleClickButton;
        public Text DoubleClickText;
        
        public Button CooldownButton;
        public Text CooldownText;
        
        public Button SphereButton;
        
        public float CooldownTime = 3f;
        
        // Start is called before the first frame update
        void Start()
        {
            //使用 Fire and Forget 编程方式，不去做异步等待
            CheckDoubleClickButton(DoubleClickButton, this.GetCancellationTokenOnDestroy()).Forget();
            CheckSphereClick(SphereButton.GetCancellationTokenOnDestroy()).Forget();
            CheckCooldownClickButton(this.GetCancellationTokenOnDestroy()).Forget();
        }

        /// <summary>
        /// 点击球体的响应事件
        /// </summary>
        /// <remarks>
        /// <para> 第一次点击球体放大，第二次点击球体缩小，第三次点击球体销毁 </para>
        /// <para> <see long="摄像机"/>组件上挂载了<see cref="PhysicsRaycaster"/>组件，球体挂载碰撞体和UI组件后，便可响应UI事件 </para>
        /// </remarks>
        private async UniTaskVoid CheckSphereClick(CancellationToken token)
        {
            //将 Button 的点击转化为 UniTaskAsyncEnumerable(异步迭代器)
            var asyncEnumerable = SphereButton.OnClickAsAsyncEnumerable();
            // ForEachAsync 异步迭代，每次迭代的时间点可能不同。以异步迭代的方式将对应时间信号放出来
            // UniTaskAsyncEnumerable 是 reactive 的，可对它进行 Take,Select,Where 等操作，将其转换为新的迭代器
            // Take(3) 表示取整个迭代的前三次，first 和 last 可以取第一次和最后一次
            // ForEachAsync 中传入的委托是同步的
            await asyncEnumerable.Take(3).ForEachAsync((_, index) =>
            {
                if (token.IsCancellationRequested) return;
                
                // index 为迭代次数
                if (index == 0)
                {
                    SphereTweenScale(2, SphereButton.transform.localScale.x, 20, token).Forget();
                }
                else if (index == 1)
                {
                    SphereTweenScale(2, SphereButton.transform.localScale.x, 10, token).Forget();
                }

            }, token);
            
            //异步迭代器迭代完成后， await 才完成。完成后执行销毁按钮逻辑
            GameObject.Destroy(SphereButton.gameObject);
        }
        
        private async UniTaskVoid SphereTweenScale(float totalTime, float from, float to, CancellationToken token)
        {
            var trans = SphereButton.transform;
            float time = 0;
            while (time < totalTime)
            {
                time += Time.deltaTime;
                trans.localScale = (from + (time / totalTime) * (to - from)) * Vector3.one;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        /// <summary>
        /// 按钮冷却
        /// </summary>
        /// <param name="token"></param>
        private async UniTaskVoid CheckCooldownClickButton(CancellationToken token)
        {
            var asyncEnumerable = CooldownButton.OnClickAsAsyncEnumerable();
            
            // ForEachAwaitAsync 中传入的委托是异步的（C# 8.0及以上的操作）
            await asyncEnumerable.ForEachAwaitAsync(async (_) =>
            {
                CooldownText.text = "被点击了，冷却中……";
                
                // Delay 过程中，ForEachAwaitAsync 不会再响应，从而推迟了下一次的点击事件的响应时间
                // 这里是阻塞的，无法在冷却当中接受其他操作。
                // 要想要冷却期间接收其他操作，就要进行事件排队 asyncEnumerable.Queue().ForEachAwaitAsync 此时冷却效果没有，但await后面的代码还会被执行（await后面的代码还会被继续执行存疑）
                await UniTask.Delay(TimeSpan.FromSeconds(CooldownTime), cancellationToken: token);
                CooldownText.text = "冷却好了，可以点了……";
            }, cancellationToken: token);
        }
        
        /// <summary>
        /// 按钮双击检测
        /// </summary>
        /// <param name="button"></param>
        /// <param name="token"></param>
        private async UniTaskVoid CheckDoubleClickButton(Button button, CancellationToken token)
        {
            while (true)
            {
                //将 Button 的点击转换为异步UniTask，UI元素事件都能转换为 异步UniTask
                var clickAsync = button.OnClickAsync(token);
                //等待一次
                await clickAsync;
                DoubleClickText.text = $"按钮被第一次点击";
                var secondClickAsync = button.OnClickAsync(token);
                
                //判断第二次点击先完成，还是指定的延迟时间先结束。将 第二次点击 与 指定时间的延迟 合并为一个 WhenAny
                int resultIndex = await UniTask.WhenAny(secondClickAsync, UniTask.Delay(TimeSpan.FromSeconds(CheckDoubleClickTime), cancellationToken: token));
                if (resultIndex == 0)
                {
                    DoubleClickText.text = $"按钮被双击了";
                }
                else
                {
                    DoubleClickText.text = $"超时，按钮算单次点击";
                }
            }
        }
    }

}

