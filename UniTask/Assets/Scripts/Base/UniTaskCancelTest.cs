using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace UniTaskUsageSample.Base
{
    public class UniTaskCancelTest : MonoBehaviour
    {
        public Runner FirstRunner;
        public Runner SecondRunner;

        public Button FirstRunButton;
        public Button SecondRunButton;
        public Button ResetButton;

        public Button FirstCancelButton;
        public Button SecondCancelButton;

        public float TotalDistance = 15;

        private CancellationTokenSource _firstCancelToken;
        private CancellationTokenSource _secondCancelToken;
        private CancellationTokenSource _linkedCancelToken;

        public Text FirstText;
        public Text SecondText;

        void Start()
        {
            FirstRunButton.onClick.AddListener(OnClickFirstRun);
            SecondRunButton.onClick.AddListener(OnClickSecondRun);

            FirstCancelButton.onClick.AddListener(OnClickFirstCancel);
            SecondCancelButton.onClick.AddListener(OnClickSecondCancel);

            ResetButton.onClick.AddListener(OnClickReset);
            _firstCancelToken = new CancellationTokenSource();
            // 注意这里可以直接先行设置多久以后取消
            // _firstCancelToken = new CancellationTokenSource(TimeSpan.FromSeconds(1.5f));
            _secondCancelToken = new CancellationTokenSource();
            _linkedCancelToken =
                CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
        }
        
        void OnDestroy()
        {
            _firstCancelToken.Dispose();
            _secondCancelToken.Dispose();
            _linkedCancelToken.Dispose();
        }
        
        private void OnClickSecondCancel()
        {
            _secondCancelToken.Cancel();
            
            //使 Token 能重复使用
            _secondCancelToken.Dispose();
            _secondCancelToken = new CancellationTokenSource();
            
            //当前 first 或 Second 任意一个取消时，link 也会跟着取消。处理联动时非常有用
            _linkedCancelToken =
                CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
        }

        private void OnClickFirstCancel()
        {
            _firstCancelToken.Cancel();
            _firstCancelToken.Dispose();
            _firstCancelToken = new CancellationTokenSource();
            _linkedCancelToken =
                CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
        }
        
        
        private async void OnClickFirstRun()
        {
            try
            {
                await RunSomeOne(FirstRunner, _firstCancelToken.Token);
            }
            catch (OperationCanceledException e)
            {
                FirstText.text = ("1号跑已经被取消");
            }
        }

        private async void OnClickSecondRun()
        { 
            // SuppressCancellationThrow 忽略对异常的抛出
            // 下行返回值中，值一为是否取消，值二为 await 的返回值
            // 根据返回值决定是否取消的方式性能比上面的异常捕获取消方式性能更好
            // Second 使用的是linkToken，会随着FirstToken的取消而取消
            var (cancelled, _) = await RunSomeOne(SecondRunner, _linkedCancelToken.Token).SuppressCancellationThrow();
            if (cancelled)
            {
                SecondText.text = ("2号跑已经被取消");
            }
        }

        private async UniTask<int> RunSomeOne(Runner runner, CancellationToken cancellationToken)
        {
            runner.Reset();
            float totalTime = TotalDistance / runner.Speed;
            //流逝的时间
            float timeElapsed = 0;
            while (timeElapsed <= totalTime)
            {
                timeElapsed += Time.deltaTime;
                await UniTask.NextFrame(cancellationToken);


                float runDistance = Mathf.Min(timeElapsed, totalTime) * runner.Speed;
                runner.Target.position = runner.StartPos + Vector3.right * runDistance;
            }

            runner.ReachGoal = true;
            return 0;
        }
        
        private void OnClickReset()
        {
            _firstCancelToken.Cancel();
            _firstCancelToken = new CancellationTokenSource();
            _secondCancelToken = new CancellationTokenSource();
            _linkedCancelToken =
                CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
            FirstRunner.Reset();
            SecondRunner.Reset();
            FirstText.text = "";
            SecondText.text = "";
        }
    }
}


