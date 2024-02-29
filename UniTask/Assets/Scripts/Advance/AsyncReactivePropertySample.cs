using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UniTaskUsageSample.Advance
{
    public class AsyncReactivePropertySample : MonoBehaviour
    {
        /*使用 AsyncReactiveProperty<T> 记录数值变化时，可将 AsyncReactiveProperty<T> 转为 异步可迭代器
         * 异步可迭代器能进行各种操作，能对每次异步迭代的时间点做各种处理
         * 同时能跟踪数据的变化，对变化整体做一个异步处理
         */
        
        private AsyncReactiveProperty<int> curHp;
        public int maxHp = 100;
        public float totalChangeTime = 10f;
        public Text CurHealText;
        public Text StateText;
        public Text ChangeText;
        
        public Slider HpSlider;
        public Image HpBarImage;

        public Button HealButton;
        public Button HurtButton;
        
        private int maxHeal = 10;
        private int maxHurt = 10;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _linkedTokenSource;
        
        private void Start()
        {
            // 设置AsyncReactiveProperty
            curHp = new AsyncReactiveProperty<int>(maxHp);
            HpSlider.maxValue = maxHp;
            HpSlider.value = maxHp;
            
            curHp.Subscribe(OnHpChange);
            CheckHpChange(curHp).Forget();
            CheckFirstLowHp(curHp).Forget();
            
            //将 AsyncReactiveProperty 绑定到 UI组件上，相当于 MVVM 的数据绑定
            curHp.BindTo(CurHealText);
            
            HealButton.onClick.AddListener(OnClickHeal);
            HurtButton.onClick.AddListener(OnClickHurt);
            
            _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 检测每次血量的变化
        /// </summary>
        /// <param name="hp"></param>
        private async UniTaskVoid CheckHpChange(AsyncReactiveProperty<int> hp)
        {
            int hpValue = hp.Value;
            //将hp的变化转为异步可迭代器，WithoutCurrent 每次迭代时发射出委托，但不记初始值
            await hp.WithoutCurrent().ForEachAsync((_, index) =>
            {
                ChangeText.text = $"血量发生变化 第{index}次 变化{hp.Value - hpValue}";
                hpValue = hp.Value;
            }, this.GetCancellationTokenOnDestroy());
        }


        private void OnClickHeal()
        {
            ChangeHp(Random.Range(0, maxHeal));
        }
        
        private void OnClickHurt()
        {
            ChangeHp(-Random.Range(0, maxHurt));
        }

        private void ChangeHp(int deltaHp)
        {
            curHp.Value = Mathf.Clamp(curHp.Value + deltaHp, 0, maxHp);
        }

        /// <summary>
        /// 查询第一次血量低于界限时
        /// </summary>
        /// <param name="hp"></param>
        private async UniTaskVoid CheckFirstLowHp(AsyncReactiveProperty<int> hp)
        {
            // hp 的初次变化，相当于 Linq 中的 First，满足传入的条件，await 完成，继续执行后面的逻辑
            await hp.FirstAsync((value) => value < maxHp * 0.4f, this.GetCancellationTokenOnDestroy());
            StateText.text = "首次血量低于界限，请注意!";
        }

        private async UniTaskVoid OnHpChange(int hp)
        {
            //hp变化时,重置 cancellation 
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, this.GetCancellationTokenOnDestroy());
            await SyncSlider(hp, _linkedTokenSource.Token);
        }

        /// <summary>
        /// 同步血条
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask SyncSlider(int hp, CancellationToken token)
        {
            var sliderValue = HpSlider.value;
            //一次血量变化时UI更新完成所需要的时间
            float needTime = Mathf.Abs((sliderValue - hp) / maxHp * totalChangeTime);
            float useTime = 0;
            while (useTime < needTime)
            {
                useTime += Time.deltaTime;
                bool result = await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();
                if (result) return;
                
                //血量变化中时，每帧的血量值
                var newValue = (sliderValue + (hp - sliderValue) * (useTime / needTime));
                SetNewValue(newValue);
            }
        }
        
        private void SetNewValue(float newValue)
        {
            if (!HpSlider) return;
            HpSlider.value = newValue;
            HpBarImage.color = HpSlider.value / maxHp < 0.4f ? Color.red : Color.white;
        }
    }
}

