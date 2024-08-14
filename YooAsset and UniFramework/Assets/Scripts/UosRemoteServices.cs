using System.IO;
using UnityEngine;
using YooAsset;

public class UosRemoteServices : IRemoteServices
{
    private readonly string _defaultHostServer;

    private readonly string _fallbackHostServer;

    public UosRemoteServices(string defaultHostServer,string fallbackHostServer)
    {
        _defaultHostServer = defaultHostServer;
        _fallbackHostServer = fallbackHostServer;
    }
    
    public string GetRemoteMainURL(string fileName)
    {
        
        string filePath = Path.Combine(_defaultHostServer,fileName);
        
        Debug.Log($"请求下载文件的URL：{filePath}");
        return filePath;
    }

    public string GetRemoteFallbackURL(string fileName)
    {
        string filePath = Path.Combine(_defaultHostServer,fileName);
        
        Debug.Log($"请求下载文件的URL：{filePath}");
        return filePath;
    }
}
