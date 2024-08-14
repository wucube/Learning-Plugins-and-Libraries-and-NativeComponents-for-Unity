using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public partial class YooAssetManager
{
    
    /// <summary>
    ///  资源系统在编辑器中模拟运行，不需要构建资源包。该模式只在编辑器中有效
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="playMode"></param>
    public async UniTask InitPackageAsync_EditorSimulate()
    {
        //注意：如果是原生文件系统选择EDefaultBuildPipeline.RawFileBuildPipeline
        var buildPipeline = EDefaultBuildPipeline.ScriptableBuildPipeline; 
        var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(buildPipeline, _package.PackageName);
        var editorFileSystem = FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult);
        var initParameters = new EditorSimulateModeParameters();
        initParameters.EditorFileSystemParameters = editorFileSystem;
        var initOperation =  _package.InitializeAsync(initParameters);
        await initOperation.ToUniTask();
        if(initOperation.Status == EOperationStatus.Succeed)
            Debug.Log("资源包初始化成功！");
        else 
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
    }
    
    /// <summary>
    /// 资源系统的单机运行模式
    /// </summary>
    /// <param name="packageName"></param>
    /// <remarks>适合不需要热更新资源的游戏，该模式需要构建资源包</remarks>
    public async UniTask InitPackageAsync_OfflinePlay()
    {
        var buildinFileSystem = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        var initParameters = new OfflinePlayModeParameters();
        initParameters.BuildinFileSystemParameters = buildinFileSystem;
        var initOperation =  _package.InitializeAsync(initParameters);
        await initOperation.ToUniTask();
        if(initOperation.Status == EOperationStatus.Succeed)
            Debug.Log("资源包初始化成功！");
        else 
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        
    }
    
    /// <summary>
    /// 资源系统的联机运行模式
    /// </summary>
    /// <param name="packageName"></param>
    /// <remarks>适合需要热更新资源的游戏，该模式需要构建资源包</remarks>
    public async UniTask InitPackageAsync_HostPlay(string bucketId,string buildPackageVersion)
    {
        string defaultHostServer = GetHostServerURL(bucketId,buildPackageVersion,RemoteMode.HostServer);
        string fallbackHostServer = GetHostServerURL(bucketId,buildPackageVersion,RemoteMode.HostServer);
        IRemoteServices remoteServices = new UosRemoteServices(defaultHostServer, fallbackHostServer);
        var cacheFileSystem = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
        var buildinFileSystem = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();   
        var initParameters = new HostPlayModeParameters
        {
            BuildinFileSystemParameters = buildinFileSystem,
            CacheFileSystemParameters = cacheFileSystem
        };
        
        var initOperation = _package.InitializeAsync(initParameters);
        await initOperation.ToUniTask();
            
        if(initOperation.Status == EOperationStatus.Succeed)
            Debug.Log("资源包初始化成功！");
        else 
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
    }

    /// <summary>
    /// 资源系统专门针对 WebGL 平台的专属模式
    /// </summary>
    /// <param name="packageName"></param>
    /// <remarks> 微信小游戏，抖音小游戏都需要选择该模式。</remarks>
    public async UniTask InitPackageAsync_WebPlay()
    {
        var webFileSystem = FileSystemParameters.CreateDefaultWebFileSystemParameters(false);
        var initParameters = new WebPlayModeParameters
        {
            WebFileSystemParameters = webFileSystem
        };
        
        var initOperation = _package.InitializeAsync(initParameters);
        await initOperation.ToUniTask();
        if(initOperation.Status == EOperationStatus.Succeed)
            Debug.Log("资源包初始化成功！");
        else 
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
    }
    
    public async UniTask InitPackageAsync_WechatMinigame(string bucketId,string buildPackageVersion)
    {
        // string defaultHostServer = GetHostServerURL(bucketId,buildPackageVersion,RemoteMode.WechatMinigame);
        // string fallbackHostServer = GetHostServerURL(bucketId,buildPackageVersion,RemoteMode.WechatMinigame);
        // IRemoteServices remoteServices = new UosRemoteServices(defaultHostServer,fallbackHostServer);
        //
        // var weChatFileSystem = WechatFileSystemCreater.CreateWechatFileSystemParameters(remoteServices);
        // var initParameters = new WebPlayModeParameters
        // {
        //     WebFileSystemParameters = weChatFileSystem
        // };
        //
        // var initOperation = _package.InitializeAsync(initParameters);
        // await initOperation.ToUniTask();
        // if(initOperation.Status == EOperationStatus.Succeed)
        //     Debug.Log("资源包初始化成功！");
        // else 
        //     Debug.LogError($"资源包初始化失败：{initOperation.Error}");
    }
}
