using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

public class LoadTest : MonoBehaviour
{

    [SerializeField] [LabelText("枪模型的父节点")]
    private Transform gunParentTrans;
    
    private ResourcePackage _package;
    
    // Start is called before the first frame update
    void Start()
    {
        YooAssets.Initialize();
        _package = YooAssets.CreatePackage("Gun");
        YooAssets.SetDefaultPackage(_package);

        StartCoroutine(nameof(InitializeYooAsset));
    }

    private IEnumerator InitializeYooAsset()
    {
        var initParameters = new OfflinePlayModeParameters();
        //initParameters.SandboxRootDirectory = Application.streamingAssetsPath + "/YooAB/Gun/v0.1";
        
        yield return _package.InitializeAsync(initParameters);
    }
    
    

    [Button("加载枪械模")]
    private void LoadGun(string modelName)
    {

        AssetHandle assetHandle =  _package.LoadAssetAsync<GameObject>($"Assets/_Assets/Model/Gun/{modelName}.FBX");
        assetHandle.Completed += Handle_Completed;
    }

    private void Handle_Completed(AssetHandle handle)
    {
        GameObject go =  Instantiate(handle.AssetObject as GameObject);
        go.transform.SetParent(gunParentTrans);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
