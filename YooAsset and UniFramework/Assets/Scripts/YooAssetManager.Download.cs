using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public partial class YooAssetManager
{
    public async UniTask<bool> DownloadAsync()
    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = _package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
    
        //没有需要下载的资源
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("没有需要下载的资源");
            return true;
        }
    
        //需要下载的文件总数和总大小
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;    
    
        //注册回调方法
        // downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
        // downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
        // downloader.OnDownloadOverCallback = OnDownloadOverFunction;
        // downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;
    
        //开启下载
        downloader.BeginDownload();
        await downloader.ToUniTask();
    
        //检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //下载成功
            Debug.Log("资源下载成功");
            return true;
        }
        else
        {
            //下载失败
            Debug.Log("下载失败");
            return false;
        }
    }
    

    /// <summary>
    /// 检查本地资源清单的完整性
    /// </summary>
    /// <returns></returns>
    public bool CheckLocalManifestIntegrity()
    {
        var downloader = _package.CreateResourceDownloader(1, 1, 60);
        if (downloader.TotalDownloadCount > 0)   
        {
            Debug.LogWarning("资源内容本地并不完整，需要更新资源！");
            return false;
        }

        Debug.Log("资源内容在本地完整！");
        return true;
    }
    
    public async UniTask<T> LoadResource<T>(string filePath) where T:Object
    {
        AssetHandle handle = _package.LoadAssetAsync<T>(filePath);
        await handle.ToUniTask();
        return handle.AssetObject as T;
    }

    public async UniTask<GameObject> LoadPrefab(string filePath)
    {
        AssetHandle handle = _package.LoadAssetAsync<GameObject>(filePath);
        await handle.ToUniTask();
        return handle.InstantiateSync();
    }
    
}
