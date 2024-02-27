using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UniTaskUsageSample.Base
{
    public class UniTaskWhenTest : MonoBehaviour
    {
        public Runner FirstRunner;
        public Runner SecondRunner;

        public Button FirstRunButton;
        public Button SecondRunButton;

        public Button WhenAllButton;
        public Button WhenAnyButton;
        public Button ResetButton;

        public float TotalDistance = 15;
        public Text WinnerText;
        public Text CompleteText;

        void Start()
        {
            FirstRunButton.onClick.AddListener(OnClickFirstRun);
            SecondRunButton.onClick.AddListener(OnClickSecondRun);
            
            WhenAllButton.onClick.AddListener(OnClickWhenAll);
            WhenAnyButton.onClick.AddListener(OnClickWhenAny);
            ResetButton.onClick.AddListener(OnClickReset);
        }

        private void OnClickReset()
        {
            FirstRunner.Reset();
            SecondRunner.Reset();
            CompleteText.text = string.Empty;
            WinnerText.text = string.Empty;
        }
        
        private async UniTask RunSomeOne(Runner runner)
        {
            runner.Reset();
            float totalTime = TotalDistance / runner.Speed;
            //流逝的时间
            float timeElapsed = 0;
            
            while (timeElapsed <= totalTime)
            {
                timeElapsed += Time.deltaTime;
                //等待下一帧（Update之后）执行
                await UniTask.NextFrame();
                float runDistance = Mathf.Min(timeElapsed, totalTime) * runner.Speed;
                runner.Target.position = runner.StartPos + Vector3.right * runDistance;
            }
            
            runner.ReachGoal = true;
        }
        
        private async void OnClickFirstRun()
        {
            await RunSomeOne(FirstRunner);
        }

        private async void OnClickSecondRun()
        {
            await RunSomeOne(SecondRunner);
        }

        private async void OnClickWhenAll()
        {
            var firstRunnerReach = UniTask.WaitUntil(() => FirstRunner.ReachGoal);
            // WaitUntilValueChanged 也很好用
            var secondRunnerReach = UniTask.WaitUntil(() => SecondRunner.ReachGoal);
            
            await UniTask.WhenAll(firstRunnerReach, secondRunnerReach);
            
            // 注意，whenAll可以用于平行执行多个资源的读取，非常有用！
            // var (a, b, c) = await UniTask.WhenAll(
            //LoadAsSprite("foo"),
            //LoadAsSprite("bar"),
            //LoadAsSprite("baz"));
            
            CompleteText.text = "双方都抵达了终点，比赛结束";
        }

        private async void OnClickWhenAny()
        {
            var firstRunnerReach = UniTask.WaitUntil(() => FirstRunner.ReachGoal);
            var secondRunnerReach = UniTask.WaitUntil(() => SecondRunner.ReachGoal);

            await UniTask.WhenAny(firstRunnerReach, secondRunnerReach);

            string winner = FirstRunner.ReachGoal ? "绿色小球" : "黄色小球";
            WinnerText.text = $"{winner}率先抵达了终点，获得了胜利";
        }

    }
}


