using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectionCoin : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private Transform _coinParent;
    /// <summary>
    /// 金币生成的位置
    /// </summary>
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private Transform _endPosition;
    [SerializeField] private TextMeshProUGUI _coinText;
    
    [SerializeField] private float duration;
    
    [SerializeField] private int _coinAmount;
    [SerializeField] private int _minX;
    [SerializeField] private int _maxX;
    [SerializeField] private int _minY;
    [SerializeField] private int _maxY;

    private readonly List<GameObject> _coins = new List<GameObject>();
    private Tween _coinReactionTween;
    private int coin;
    
    [Button("随机位置生成金币")]
    private  async void CollectCoins()
    {
        //重置
        SetCoin(0);
        for (int i = 0; i < _coins.Count; i++)
            Destroy(_coins[i]);
        _coins.Clear();

        List<UniTask> spawnCoinTasks = new List<UniTask>();
        //在指定范围生成金币
        for (int i = 0; i < _coinAmount; i++)
        {
            GameObject coinInstance = Instantiate(_coinPrefab, _coinParent);
            float xPos = _spawnLocation.position.x + Random.Range(_minX, _maxX);
            float yPos = _spawnLocation.position.y + Random.Range(_minY, _maxY);
            
            coinInstance.transform.position = new Vector3(xPos, yPos);
            _coins.Add(coinInstance);
            
            UniTask punchPosTask = coinInstance.transform.DOPunchPosition(new Vector3(0, 20, 0), Random.Range(0, 1f)).SetEase(Ease.InOutElastic)
                .ToUniTask();
            spawnCoinTasks.Add(punchPosTask);
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.02f));
        }

        await UniTask.WhenAll(spawnCoinTasks);
        
        //所有金币移动至金币菜单栏
        await MoveCoinsTask();
    }
    
    private async UniTask MoveCoinsTask()
    {
        List<UniTask> moveCoinTasks = new List<UniTask>();

        for (int i = _coins.Count-1; i >=0; i--)
        {
            //MoveConTask中改变了对集合元素的数量
            moveCoinTasks.Add(MoveConTask(_coins[i]));
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
    }

    private async UniTask MoveConTask(GameObject coinInstance)
    {
        await coinInstance.transform.DOMove(_endPosition.position, duration).SetEase(Ease.InBack).ToUniTask();

        GameObject temp = coinInstance;
        _coins.Remove(coinInstance);
        Destroy(temp);
        
        await ReactToCollectionCoin();
        SetCoin(coin + 1);
    }

    /// <summary>
    /// 金币收集的响应
    /// </summary>
    private async UniTask ReactToCollectionCoin()
    {
        if (_coinReactionTween == null)
        {
            _coinReactionTween = _endPosition.DOPunchScale(Vector3.one * 0.5f, 0.1f).SetEase(Ease.InOutElastic);
            await _coinReactionTween.ToUniTask();
            _coinReactionTween = null;
        }
    }
    
    private void SetCoin(int value)
    {
        coin = value;
        _coinText.text = coin.ToString();
    }
}
