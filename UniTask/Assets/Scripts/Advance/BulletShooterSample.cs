﻿using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UniTaskUsageSample.Advance
{
    [Serializable]
    public struct ScoreCollider
    {
        public Collider Collider;
        public float Score;
    }

    public class BulletShooterSample : MonoBehaviour
    {
        public Transform FirePoint;

        [SerializeField] private GameObject bulletTemplate;

        [Header("射速")] [SerializeField] private float flySpeed;

        [Header("自动回收时间")] [SerializeField] private float bulletAutoDestroyTime;

        [Header("分数显示文本")] [SerializeField] private Text currentScoreText;

        [Header("分数配置")] [SerializeField] private ScoreCollider[] scoreColliders;

        [Header("命中效果")] [SerializeField] private GameObject hitEffect;

        private float totalScore = 0;


        // Start is called before the first frame update
        void Start()
        {
            CheckScoreChange().Forget();
        }

        public void Fire()
        {
            (UniTask.UnityAction(OnClickFire)).Invoke();
        }

        async UniTaskVoid CheckScoreChange()
        {
            while (true)
            {
                await UniTask.WaitUntilValueChanged(this, (target) => target.totalScore);
                currentScoreText.text = $"总分:{totalScore}";
            }
        }
        
        //UniTask将事件封装为异步可迭代器，在异步可迭代器中完成各种操作
        /*开火事件的核心
         * 将碰撞事件转化为UniTask的异步可迭代器
         * 在异步可迭代器中检测每次碰撞，执行逻辑处理
         */
        private async UniTaskVoid OnClickFire()
        {
            var bullet = Object.Instantiate(bulletTemplate);
            bullet.transform.position = FirePoint.position;
            bullet.transform.forward = FirePoint.forward;

            // 先飞出去
            var bulletToken = bullet.transform.GetCancellationTokenOnDestroy();
            FlyBullet(bullet.transform, flySpeed).Forget();
            // 等待时间到，或者碰到了任意物体。获取子弹本身的token来当作取消token

            var waitAutoDestroy =
                UniTask.Delay(TimeSpan.FromSeconds(bulletAutoDestroyTime), cancellationToken: bulletToken);

            var source = new UniTaskCompletionSource<Collision>();
            // 注意可以使用where take(1)或FirstAsync来简化操作
            // GetAsyncCollisionEnterTrigger 获取transform上的碰撞事件，并将事件转为异步可迭代器
            bullet.transform.GetAsyncCollisionEnterTrigger().ForEachAsync((collision) =>
            {
                //检测每次的碰撞
                if (collision.collider.CompareTag("Target"))
                {
                    source.TrySetResult(collision);
                }
            }, cancellationToken: bulletToken);
            
            
            /*与下面 result==1 等价的操作
             * var bulletCollisionTask = bullet.transform.GetAsyncCollisionEnterTrigger()
             *      .FirstAsync(collision => collision.collider.CompareTag("Target"), bulletToken);
             * int result = await UniTask.WhenAny(waitAutoDestroy, bulletCollisionTask);
             */
            
            int result = await UniTask.WhenAny(waitAutoDestroy, source.Task);
            if (result == 0)
            {
            }
            else if (result == 1)
            {
                var collision = source.GetResult(0);
                Collider getCollider = collision.collider;
                var go = Object.Instantiate(hitEffect, bullet.transform.position, Quaternion.identity);
                Object.Destroy(go, 4f);
                foreach (ScoreCollider scoreCollider in scoreColliders)
                {
                    if (getCollider == scoreCollider.Collider)
                    {
                        totalScore += scoreCollider.Score;
                    }
                }
            }

            Object.Destroy(bullet);
        }

        private async UniTaskVoid FlyBullet(Transform bulletTransform, float speed)
        {
            float startTime = Time.time;
            Vector3 startPosition = bulletTransform.position;
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, bulletTransform.GetCancellationTokenOnDestroy());
                bulletTransform.position = startPosition + (speed * (Time.time - startTime)) * bulletTransform.forward;
            }
        }
    }
}


