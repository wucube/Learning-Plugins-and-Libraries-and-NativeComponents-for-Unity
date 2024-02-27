using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UniTaskUsageSample.Base
{
    public class UnityWebRequestTimeoutSample : MonoBehaviour
    {
        public Button SearchResultButton;
        public string SearchWord = "Unity";
    
        public string[] SearchUrls = new string[]
        {
            "https://www.baidu.com/",
            "https://www.bing.com/",
            "https://www.google.com/?hl=zh_cn"
            
        };
    
        public Text[] Texts;
        
        // Start is called before the first frame update
        void Start()
        {
            SearchResultButton.onClick.AddListener(UniTask.UnityAction(OnClickSearch));
        }
        
        private async UniTaskVoid OnClickSearch()
        {
            UniTask<string>[] waitTasks = new UniTask<string>[SearchUrls.Length];
    
            for (int i = 0; i < SearchUrls.Length; i++)
            {
                waitTasks[i] = GetRequest(SearchUrls[i], 2f);
            }
            //所有UniTask全部完成时接收返回的字符串数组，并赋值
            var tasks = await UniTask.WhenAll(waitTasks);
            for (int i = 0; i < tasks.Length; i++)
            {
                Texts[i].text = tasks[i];
            }
        }
    
        private async UniTask<string> GetRequest(string url, float timeout)
        {
            var cancelToken = new CancellationTokenSource();
    
            // token 将在 timeout 结束后发送 取消 信号
            cancelToken.CancelAfterSlim(TimeSpan.FromSeconds(timeout));
    
            // 网络请求发送后，cancelToken 会发送信号并禁止异常抛出，接收 cancelToken 取消的结果（成功or失败）
            var (cancelOrFailed, result) = await UnityWebRequest.Get(url).SendWebRequest()
                .WithCancellation(cancelToken.Token).SuppressCancellationThrow();
            if (!cancelOrFailed)
            {
                return result.downloadHandler.text.Substring(0, 100);
            }
    
            return "超时";
        }
    }
}


