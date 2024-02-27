using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UniTaskUsageSample.Base
{
    public class ThreadSample : MonoBehaviour
    {
        public Button StandardRun;
        public Button YieldRun;

        public Text Text;
    
        // Start is called before the first frame update
        void Start()
        {
            StandardRun.onClick.AddListener(UniTask.UnityAction(OnClickStandardRun));
            YieldRun.onClick.AddListener(UniTask.UnityAction(OnClickYieldRun));
        }

        //使用 UniTask.Yield 切换线程
        private async UniTaskVoid OnClickYieldRun()
        {
            //主线程上获取文件名
            string fileName = Application.dataPath+"/Resources/Test.txt";
            //切换到线程池上运行
            await UniTask.SwitchToThreadPool();
            //标准的C#Task，在额外的线程上读取文本
            string fileContent = await File.ReadAllTextAsync(fileName);
            //切换到主线程，主线程等待到Update执行。
            //运行UniTask.Yield时，执行线程就会回到主线程中
            //UniTask.Yield 包含在Unity的生命周期函数中，并且还能切换线程
            await UniTask.Yield(PlayerLoopTiming.Update);
            Text.text = fileContent;
        }

        // 标准的切换线程方法
        private async UniTaskVoid OnClickStandardRun()
        {
            int result = 0;
            //将 UniTask 运行在线程池上
            await UniTask.RunOnThreadPool(() => result = 1);
            //将 UniTask 返回到主线程上运行
            await UniTask.SwitchToMainThread();
            Text.text = $"计算结束，当前结果是{result}";
        }
    }
}

