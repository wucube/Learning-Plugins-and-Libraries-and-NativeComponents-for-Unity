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

        //球体点击
        private async UniTaskVoid CheckSphereClick(CancellationToken token)
        {
            //将 Button 的点击转化为 UniTaskAsyncEnumerable(异步可迭代器)
            var asyncEnumerable = SphereButton.OnClickAsAsyncEnumerable();
            await asyncEnumerable.Take(3).ForEachAsync((_, index) =>
            {
                if (token.IsCancellationRequested) return;
                if (index == 0)
                {
                    SphereTweenScale(2, SphereButton.transform.localScale.x, 20, token).Forget();
                }
                else if (index == 1)
                {
                    
                    SphereTweenScale(2, SphereButton.transform.localScale.x, 10, token).Forget();
                }

            }, token);
            GameObject.Destroy(SphereButton.gameObject);
        }

        //
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

        private async UniTaskVoid CheckCooldownClickButton(CancellationToken token)
        {
            var asyncEnumerable = CooldownButton.OnClickAsAsyncEnumerable();
            await asyncEnumerable.ForEachAwaitAsync(async (_) =>
            {
                CooldownText.text = "被点击了，冷却中……";
                await UniTask.Delay(TimeSpan.FromSeconds(CooldownTime), cancellationToken: token);
                CooldownText.text = "冷却好了，可以点了……";
            }, cancellationToken: token);
        }
        
        private async UniTaskVoid CheckDoubleClickButton(Button button, CancellationToken token)
        {
            while (true)
            {
                var clickAsync = button.OnClickAsync(token);
                await clickAsync;
                DoubleClickText.text = $"按钮被第一次点击";
                var secondClickAsync = button.OnClickAsync(token);
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

