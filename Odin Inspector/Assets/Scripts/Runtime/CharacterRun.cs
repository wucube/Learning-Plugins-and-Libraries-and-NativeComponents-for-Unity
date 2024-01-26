using System;
using UnityEngine;
using Sirenix.OdinInspector;


public class CharacterRun:MonoBehaviour
{
    [LabelText("血量")]
    [SerializeField] private float _hp;
    [LabelText("蓝量")]
    [SerializeField] private float _mp;
    [LabelText("角色名称")]
    [SerializeField] private string _characterName;
    [LabelText("角色描述")]
    [SerializeField] private string _desc;

    [Button]
    public void Print()
    {
        Debug.Log($"角色的血量:{_hp}");
    }

    [Button("角色血量+1")]
    public void Update(int value = 1)
    {
        _hp += value;
    }
}
