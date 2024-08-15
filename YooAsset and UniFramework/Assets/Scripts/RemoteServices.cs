using UnityEngine;
using YooAsset;

/// <summary>
/// 远端资源地址查询服务类
/// </summary>
public class RemoteServices : IRemoteServices
{
    private readonly string _defaultHostServer;

    private readonly string _fallbackHostServer;

    public RemoteServices(string defaultHostServer,string fallbackHostServer)
    {
        _defaultHostServer = defaultHostServer;
        _fallbackHostServer = fallbackHostServer;
    }
    
    public string GetRemoteMainURL(string fileName)
    {
        string filePath = $"{_defaultHostServer}/{fileName}";
        
        Debug.Log($"请求下载文件的URL：{filePath}");
        return filePath;
    }

    public string GetRemoteFallbackURL(string fileName)
    {
        string filePath = $"{_fallbackHostServer}/{fileName}";
        
        Debug.Log($"请求下载文件的URL：{filePath}");
        return filePath;
    }
}
