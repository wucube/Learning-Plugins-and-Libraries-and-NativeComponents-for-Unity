using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProperty",menuName = "CharacterPropertySO",order = 1)]
public class CharacterPropertySO : ScriptableObject
{
    [LabelText("血量")]
    [SerializeField] private float _hp;
    [LabelText("蓝量")]
    [SerializeField] private float _mp;
    [LabelText("角色名称")]
    [SerializeField] private string _characterName;
    [LabelText("角色描述")]
    [SerializeField] private string _desc;
}
