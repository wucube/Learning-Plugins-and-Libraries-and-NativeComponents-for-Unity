using UnityEngine;
using Sirenix.OdinInspector;
public class CharacterRun : MonoBehaviour
{
    [SerializeField] private float _hp;
    [SerializeField] private float _mp;
    [LabelText("角色名称")] [SerializeField] private string _characterName;
    [LabelText("角色描述")] [SerializeField] private string _desc;
    
    [Button]
    public void Print()
    {
        Debug.Log($"角色的血量:{_hp}");
    }

    [Button("角色血量+1")]
    public void MyUpdate(int value = 1)
    {
        _hp += value;
    }
}
   
