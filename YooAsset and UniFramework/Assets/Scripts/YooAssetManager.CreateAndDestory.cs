using UnityEngine;
using Cysharp.Threading.Tasks;
using YooAsset;

public partial class YooAssetManager
{
    private ResourcePackage _package;

    public YooAssetManager(string packageName)
    {
        CrateAndSetDefaultPackage(packageName);
    }
    
    /// <summary>
    /// 创建并设置默认的资源包对象
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    private void CrateAndSetDefaultPackage(string packageName)
    {
        // 创建默认的资源包
        YooAssets.Initialize();
        _package = YooAssets.CreatePackage(packageName);
        // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
        YooAssets.SetDefaultPackage(_package);
    }
    
    /// <summary>
    /// 销毁资源包对象
    /// </summary>
    /// <param name="packageName"></param>
    public async UniTask DestroyPackageAsync(string packageName)
    {
        // 先销毁资源包
        var package = YooAssets.GetPackage(packageName);
        DestroyOperation operation = package.DestroyAsync();
        await operation.ToUniTask();
        
        // 然后移除资源包
        if (YooAssets.RemovePackage(packageName))
        {
            Debug.Log("移除成功！");
        }
    }
}
