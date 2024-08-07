using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodAnimation : MonoBehaviour
{
    private RectTransform _rectTransform;

    [SerializeField]
    private AnimationCurve _animationCurve;

    private int _length;
    private Keyframe[] _keyframes;

    private Vector3 _bloodTextPos = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _bloodTextPos = _rectTransform.localPosition;

        _keyframes = _animationCurve.keys;
        _length = _animationCurve.length;
        
        Debug.Log(_length+"   " +_keyframes.Length);

        for (int i = 0; i < _keyframes.Length; i++)
        {
            Debug.Log(_keyframes[i].time + "    " +_keyframes[i].value);
        }
    }

    private float time = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _animationCurve = createAnimationCurve();
        }
        GetAnimationCurveValue();
    }

    private void GetAnimationCurveValue()
    {
        if (_keyframes.Length <= 0)
        {
            Debug.LogWarning("Keyframes Length is 0");
            return;
        }

        if (time < _keyframes[_length - 1].time)
        {
            time += Time.deltaTime;
            //根据时间获取动画曲线相应点的 value
            float value = _animationCurve.Evaluate(time);
            Debug.Log("Time :"+time+"   value : "+value );

            Vector3 pos = _rectTransform.localPosition;
            pos.y = _bloodTextPos.y + value;
            
            //设置文字价值的 Y 值
            _rectTransform.localPosition = pos;
        }
        else
        {
            time = 0;
        }
    }

    //代码创建动画曲线
    private AnimationCurve createAnimationCurve()
    {
        Keyframe[] keyframes = new Keyframe[30];
        int i = 0;
        while (i<keyframes.Length)
        {
            // 给每个关键帧赋值 time, value
            keyframes[i] = new Keyframe(i, Mathf.Sin(i));
            i++;
        }
        
        //设置前一个点进入该关键帧的切线（设置斜率）
        keyframes[i].inTangent = 45;

        keyframes[10].outTangent = 90;

        AnimationCurve animationCurve = new AnimationCurve(keyframes);

        animationCurve.postWrapMode = WrapMode.Loop;

        animationCurve.preWrapMode = WrapMode.Once;

        Keyframe keyframe = new Keyframe(31, 2);

        animationCurve.AddKey(keyframe);
        animationCurve.RemoveKey(10);
        
        animationCurve.SmoothTangents(20,3);

        float time = 15.5f;
        animationCurve.Evaluate(this.time);

        return animationCurve;
    }
}
