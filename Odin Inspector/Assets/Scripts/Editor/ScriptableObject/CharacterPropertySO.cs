using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProperty",menuName = "CharacterPropertySO",order = 1)]
public class CharacterPropertySO : ScriptableObject
{
    /// <summary>
    /// 职业类型
    /// </summary>
    public enum ProfessionType
    {
        [LabelText("战士")]
        Warrior,
        
        [LabelText("法师")]
        Mage
    }
    
    [LabelText("血量")]
    [SerializeField] private float _hp;
    [LabelText("蓝量")][HideIf("@_professionType == ProfessionType.Warrior")]
    [SerializeField] private float _mp;
    [LabelText("角色名称")]
    [SerializeField] private string _characterName;
    [LabelText("角色描述")]
    [SerializeField] private string _desc;
    
    [LabelText("职业")]
    [SerializeField] private ProfessionType _professionType;

    [Button]
    public void Print()
    {
        Debug.Log($"角色的血量:{_hp}");
    }

    [Button("角色血量+1")]
    public void MyUpdate(int value)
    {
        _hp += value;
    }
}
