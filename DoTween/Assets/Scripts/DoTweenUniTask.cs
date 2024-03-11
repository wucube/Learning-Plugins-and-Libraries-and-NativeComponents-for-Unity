using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DoTweenUniTask : MonoBehaviour
{
    [SerializeField] private Transform _cube;

    [SerializeField] private Button _cancelButton;

    private CancellationTokenSource _cts = new CancellationTokenSource();
    
    
    // Start is called before the first frame update
    void Start()
    {
        //DoTweenMoveCompleted();
        //DoTweenUniTaskMoveCompleted();
        _cancelButton.onClick.AddListener(OnCancelClick);
        CancelTweenAwait();
    }

    private void DoTweenMoveCompleted()
    {
        _cube.transform.DOMove(new Vector3(5, 0, 5), 2f).OnComplete(() => Debug.Log("移动完成"));
    }

    private async UniTaskVoid DoTweenUniTaskMoveCompleted()
    {
        await _cube.transform.DOMove(new Vector3(5, 0, 5), 2f).ToUniTask();
        Debug.Log("UniTask移动完成");
    }

    private async void CancelTweenAwait()
    {
        await _cube.DOMove(new Vector3(5, 0, 5), 5f).WithCancellation(_cts.Token)
            .ContinueWith(() => Debug.Log("Finish"));
        Debug.Log("移动完成");
    }

    private void OnCancelClick()
    {
        _cts.Cancel();
    }
    
}
