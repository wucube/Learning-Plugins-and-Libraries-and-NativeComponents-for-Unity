using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameLogic:MonoBehaviour
{
    [SerializeField] private Transform _skillCenter;
    [SerializeField] private Transform _avatarCenter;

    private  YooAssetManager _assetManager;
    
    async UniTaskVoid Awake()
    {
        _assetManager = new YooAssetManager("TestPackage");
        
        await _assetManager.InitPackageAsync_OfflinePlay();
        string packageVersion = await _assetManager.RequestPackageVersionAsync();
        await _assetManager.UpdatePackageManifestAsync(packageVersion);
        _assetManager.CheckLocalManifestIntegrity();

        await LoadUIPrefab("Assets/Prefabs/ELF-LADY",_avatarCenter);
        await LoadUIPrefab("Assets/Prefabs/ELF-PRINCE",_avatarCenter);
        await LoadUIPrefab("Assets/Prefabs/ELF-WARRIOR",_avatarCenter);


        Sprite[] sprites = await LoadAllSpritesFromAtlasAsync("Assets/Atlas/UI/Skill/Skill Icons");
        
        for (int i = 0; i < sprites.Length; i++)
        {
            await LoadIconsFromAtlas(sprites, "Assets/Prefabs/Skill_Icon_Image", i, _skillCenter);
        }
        
    }

    void Start()
    {
       
    }
    
    private async UniTask LoadUIPrefab(string filePath,Transform parent)
    {
        GameObject go  = await _assetManager.LoadPrefab(filePath);
        
        int secondSlashIndex = filePath.IndexOf('/', filePath.IndexOf('/') + 1);
        string result = filePath.Substring(secondSlashIndex + 1);

        go.name = result;
        
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.one;
        go.transform.SetParent(parent);
    }
    
    private async UniTask<Sprite[]> LoadAllSpritesFromAtlasAsync(string filePath)
    {
        var atlas = await _assetManager.LoadResource<SpriteAtlas>(filePath);
        Sprite[] sprites = new Sprite [atlas.spriteCount];
        atlas.GetSprites(sprites);
        return sprites;
    }

    private async UniTask LoadIconsFromAtlas(Sprite[] sprites, string prefabPath,int spriteIndex,Transform parent)
    {
        GameObject go  = await _assetManager.LoadPrefab(prefabPath);
        
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.one;
        go.GetComponent<Image>().sprite = sprites[spriteIndex];
        go.name = sprites[spriteIndex].name;
        
        Debug.Log($"第【{spriteIndex+1}】个被加载的精灵图是【{sprites[spriteIndex].name}】");
        go.transform.SetParent(parent);
    }
}
