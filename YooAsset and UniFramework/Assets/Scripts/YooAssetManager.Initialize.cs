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
        string defaultHostServer = GetHostServerURL(bucketId, buildPackageVersion, RemoteMode.HostServer);
        string fallbackHostServer = GetHostServerURL(bucketId, buildPackageVersion, RemoteMode.HostServer);
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        
        var initParameters = new HostPlayModeParameters
        {
            //若 StreamingAssets 目录为空，BuildinFileSystemParameters 就为空
            //BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters(),
            CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices)
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

#if UNITY_WEBGL && WEIXINMINIGAME
    public async UniTask InitPackageAsync_WechatMinigame(string bucketId,string buildPackageVersion)
    {
        string defaultHostServer = GetHostServerURL(bucketId,buildPackageVersion,RemoteMode.WechatMinigame);
        string fallbackHostServer = GetHostServerURL(bucketId,buildPackageVersion,RemoteMode.WechatMinigame);
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer,fallbackHostServer);

        var weChatFileSystem = WechatFileSystemCreater.CreateWechatFileSystemParameters(remoteServices);
        var initParameters = new WebPlayModeParameters
        {
            WebFileSystemParameters = weChatFileSystem
        };
        
        var initOperation = _package.InitializeAsync(initParameters);
        await initOperation.ToUniTask();
        if(initOperation.Status == EOperationStatus.Succeed)
            Debug.Log("资源包初始化成功！");
        else 
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
    }
#endif
    
    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL(string bucketId,string buildPackageVersion,RemoteMode remoteMode)
    {
        string hostServerIP = string.Empty;
        
        //针对 UOS CDN 的资源服务器地址
        if (remoteMode == RemoteMode.HostServer)
        {
            hostServerIP = $"https://a.unity.cn/client_api/v1/buckets/{bucketId}/entry_by_path/content/?path=";
        }
        else if(remoteMode == RemoteMode.WechatMinigame)
        {
            hostServerIP = $"https://a.unity.cn/client_api/v1/buckets/{bucketId}/content/";
        }
        string buildVersion = buildPackageVersion;
 
#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}UOS_CDN/Android/{buildVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}UOS_CDN/IPhone/{buildVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}UOS_CDN/WebGL/{buildVersion}";
        else
            return $"{hostServerIP}UOS_CDN/PC/{buildVersion}";
#else
    if (Application.platform == RuntimePlatform.Android)
        return $"{hostServerIP}UOS_CDN/Android/{buildVersion}";
    else if (Application.platform == RuntimePlatform.IPhonePlayer)
        return $"{hostServerIP}UOS_CDN/IPhone/{buildVersion}";
    else if (Application.platform == RuntimePlatform.WebGLPlayer)
        return $"{hostServerIP}UOS_CDN/WebGL/{buildVersion}";
    else
        return $"{hostServerIP}UOS_CDN/PC/{buildVersion}";
#endif
    }
}
