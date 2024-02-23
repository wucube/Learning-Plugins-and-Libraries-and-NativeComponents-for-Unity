using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniTaskBaseTest : MonoBehaviour
{
    public Button LoadTextButton;
    public Text TargetText;

    public Button LoadSceneButton;
    public Slider LoadSceneSlider;
    public Text ProgressText;

    public Button WebRequestButton;
    public Image DownloadImage;

    private void Start()
    {
        LoadTextButton.onClick.AddListener(OnClickLoadText);
        LoadSceneButton.onClick.AddListener(OnClickLoadScene);
        WebRequestButton.onClick.AddListener(OnClickWebRequest);
    }
    
    private async void OnClickLoadText()
    {
        var loadOperation = Resources.LoadAsync<TextAsset>("Test");
        var text = await loadOperation;
        TargetText.text = (text as TextAsset).text;
        
        //在非 Unity 托管的脚本中异步加载，与上述代码效果一致
        // UniTaskAsyncSample_Base asyncUniTaskLoader = new UniTaskAsyncSample_Base();
        // TargetText.text = ((TextAsset)await asyncUniTaskLoader.LoadAsync<TextAsset>("Test")).text;
    }
    
    private async void OnClickLoadScene()
    {
        await SceneManager.LoadSceneAsync("TargetScene").ToUniTask(Progress.Create<float>(p =>
        {
            LoadSceneSlider.value = p;
            if (ProgressText != null)
                ProgressText.text = $"读取进度{p * 100:F2}%";
        }));
    }

    /// <summary>
    /// 下载一个序列帧动画
    /// </summary>
    private async void OnClickWebRequest()
    {
        var webRequest =
            UnityWebRequestTexture.GetTexture("https://i0.hdslb.com/bfs/static/jinkela/video/asserts/33-coin-ani.png");
        var result = await webRequest.SendWebRequest();
        var texture = (result.downloadHandler as DownloadHandlerTexture).texture;
        //下载的一张长图片中有二十四张不同小图
        int totalSpriteCount = 24;
        
        int perSpriteWidth = texture.width / totalSpriteCount;
        Sprite[] sprites = new Sprite[totalSpriteCount];
        for (int i = 0; i < totalSpriteCount; i++)
        {

            //将下载的图片分割为24张精灵图
            sprites[i] = Sprite.Create(texture,
                new Rect(new Vector2(perSpriteWidth * i, 0), new Vector2(perSpriteWidth, texture.height)),
                    new Vector2(0.5f, 0.5f));
        }

        float perFrameTime = 0.1f;
        while (true)
        {
            for (int i = 0; i < totalSpriteCount; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(perFrameTime));
                var sprite = sprites[i];
                DownloadImage.sprite = sprite;
            }
        }
    }

    
    
}
