using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class UniTaskWaitTest : MonoBehaviour
{
    public PlayerLoopTiming TestYieldTiming = PlayerLoopTiming.PreUpdate;
    
    public Button TestDelayButton;
    public Button TestDelayFrameButton;
    public Button TestYieldButton;
    public Button TestNextFrameButton;
    public Button TestEndOfFrameButton;
    public Button ClearButton;
    
    public Text ShowLogText;

    private readonly List<PlayerLoopSystem.UpdateFunction> _injectUpdateFuncList = new List<PlayerLoopSystem.UpdateFunction>();
    private UniTaskAsyncSample_Wait _uniTaskWaiter;
    
    private bool _showUpdateLog = false;
    
    void OnEnable()
    {
        //将log输出信息显示到屏幕UI上
        Application.logMessageReceivedThreaded += ShowLog;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        TestDelayButton.onClick.AddListener(OnClickTestDelay);
        TestDelayFrameButton.onClick.AddListener(OnClickTestDelayFrame);
        TestYieldButton.onClick.AddListener(OnClickTestYield);
        TestNextFrameButton.onClick.AddListener(OnClickTestNextFrame);
        TestEndOfFrameButton.onClick.AddListener(OnClickTestEndOfFrame);
        ClearButton.onClick.AddListener(OnClickClear);
        
        _uniTaskWaiter = new UniTaskAsyncSample_Wait();
        
        InjectFunction();
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= ShowLog;
    }
    
    void OnDestroy()
    {
        UnInjectFunction();
    }
    
    private void InjectFunction()
    {
        PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        //得到 Unity 的更新内容（运行时Subsystem）
        var subsystems = playerLoop.subSystemList;
        
        playerLoop.updateDelegate += OnUpdate;
        
        //为每个 子系统 添加操作
        for (int i = 0; i < subsystems.Length; i++)
        {
            int index = i;
            PlayerLoopSystem.UpdateFunction injectFunction = () =>
            {
                //一旦显示更新Log，就会显示 子系统 的执行
                if (!_showUpdateLog) return;
                Debug.Log($"执行子系统 {_showUpdateLog} {subsystems[index]} 当前帧 {Time.frameCount}");
            };
            _injectUpdateFuncList.Add(injectFunction);
            subsystems[i].updateDelegate += injectFunction;
        }

        PlayerLoop.SetPlayerLoop(playerLoop);
    }

    private void UnInjectFunction()
    {
        PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        playerLoop.updateDelegate -= OnUpdate;
        var subsystems = playerLoop.subSystemList;
        for (int i = 0; i < subsystems.Length; i++)
        {
            subsystems[i].updateDelegate -= _injectUpdateFuncList[i];
        }
        
        PlayerLoop.SetPlayerLoop(playerLoop);
        _injectUpdateFuncList.Clear();
    }

    private void OnUpdate()
    {
        Debug.Log($"当前帧{Time.frameCount}");
    }
    
    /// <summary>
    /// 延迟到下一帧(下一帧 Update 函数之后执行)
    /// </summary>
    private async void OnClickTestNextFrame()
    {
        _showUpdateLog = true;
        Debug.Log("执行NextFrame开始");
        await _uniTaskWaiter.WaitNextFrame();
        Debug.Log($"执行NextFrame结束");
        _showUpdateLog = false;
    }

    /// <summary>
    /// 延迟到当前帧结束（当前帧结束之后，下一帧开始之前执行）
    /// </summary>
    private async void OnClickTestEndOfFrame()
    {
        _showUpdateLog = true;
        Debug.Log($"执行WaitEndOfFrame开始");
        await _uniTaskWaiter.WaitEndOfFrame(this);
        Debug.Log($"执行WaitEndOfFrame结束");
        _showUpdateLog = false;
    }
    
    
    
    /// <summary>
    /// 选择延迟执行的时机
    /// <remarks> <see cref="UniTask.Yield"/> 是等待下一个时机执行最快最好的方法 </remarks>
    /// </summary>
    private async void OnClickTestYield()
    {
        _showUpdateLog = true;
        Debug.Log($"执行Yield开始{TestYieldTiming}");
        await _uniTaskWaiter.WaitYield(TestYieldTiming);
        Debug.Log($"执行Yield结束{TestYieldTiming}");
        _showUpdateLog = false;
    }

    /// <summary>
    /// 延迟xx帧
    /// </summary>
    private async void OnClickTestDelayFrame()
    {
        Debug.Log($"执行 DelayFrame 开始，当前帧{Time.frameCount}");
        await UniTask.DelayFrame(5);
        Debug.Log($"执行 DelayFrame 结束，当前帧{Time.frameCount}");
    }

    /// <summary>
    /// 延迟xx秒
    /// </summary>
    private async void OnClickTestDelay()
    {
        Debug.Log($"执行 Delay 开始，当前时间{Time.time}");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        Debug.Log($"执行 Delay 结束，当前时间{Time.time}");
    }
    
    private void OnClickClear()
    {
        ShowLogText.text = "Log";
    }
    
    private void ShowLog(string condition, string stackTrace, LogType type)
    {
        ShowLogText.text = $"{ShowLogText.text}\n{condition}";
    }
}
