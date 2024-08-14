using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;


public enum RemoteMode{
    HostServer,
    WechatMinigame
}


public partial class YooAssetManager
{
    /// <summary>
    /// 获取包裹版本
    /// </summary>
    /// <param name="packageName"></param>
    /// <remarks>资源包初始化成功后的操作</remarks>
    public async UniTask<string> RequestPackageVersionAsync()
    {
        var operation = _package.RequestPackageVersionAsync(false);//请求 UOS CDN 中的资源时，关闭在URL末尾添加时间戳。
        await operation.ToUniTask();

        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            string packageVersion = operation.PackageVersion;
            Debug.Log($"Request package Version : {packageVersion}");
            
            return packageVersion;
        }
        
        //更新失败
        Debug.LogError(operation.Error);
        return default;
    }
    
    /// <summary>
    /// 更新资源清单
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="packageVersion"></param>
    /// <remarks> 获取资源版本号后的操作 </remarks>
    public async UniTask UpdatePackageManifestAsync(string packageVersion)
    {
        var operation = _package.UpdatePackageManifestAsync(packageVersion);
        await operation.ToUniTask();

        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
        }
        else
        {
            //更新失败
            Debug.LogError(operation.Error);
        }
    }
    
    private string GetHostServerURL(string bucketId,string buildPackageVersion,RemoteMode remoteMode)
    {
        string hostServerIP = string.Empty;
        
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
