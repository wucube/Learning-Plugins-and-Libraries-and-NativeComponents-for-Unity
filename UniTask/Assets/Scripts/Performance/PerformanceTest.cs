using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceTest : MonoBehaviour
{
    public Button Btn_CoroutineRun;

    public Button Btn_UniTaskRun;

    public int LoopTimes;

    public Text CoroutineTestText;
    public Text UniTaskTestText;

    void Start()
    {
        Btn_CoroutineRun.onClick.AddListener(OnClickCoroutineTest);
        Btn_UniTaskRun.onClick.AddListener(OnClickUniTaskTest);
    }
    
    //协程测试
    private void OnClickCoroutineTest()
    {
        StartCoroutine(nameof(CoroutineTest));
    }

    private IEnumerator CoroutineTest()
    {
        float elapsedTime = 0;

        for (int i = 0; i < LoopTimes; i++)
        {
            float time = Time.realtimeSinceStartup;
            var coroutine = StartCoroutine(nameof(EmptyCoroutine));
            elapsedTime += (Time.realtimeSinceStartup - time);
            yield return coroutine;
        }

        CoroutineTestText.text = $"协程耗时测试:{LoopTimes}次, 耗时{elapsedTime * 1000:F6}毫秒";// F6 表示小数点后六位
    }

    private IEnumerator EmptyCoroutine()
    {
        yield return null;
    }
    
    /**********************/
    
    private async void OnClickUniTaskTest()
    {
        int times = 0;
        float elapsedTime = 0;
        while (times<LoopTimes)
        {
            times++;
            float time = Time.realtimeSinceStartup;
            var uniTask = EmptyUniTask();
            elapsedTime += (Time.realtimeSinceStartup - time);
            await uniTask;
        }

        UniTaskTestText.text = $"UniTask耗时测试:{LoopTimes}次，耗时{elapsedTime * 1000:F6}毫秒";
    }

    private async UniTask EmptyUniTask()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    
}
