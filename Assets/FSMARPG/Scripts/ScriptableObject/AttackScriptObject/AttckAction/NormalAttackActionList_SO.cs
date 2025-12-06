
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new NormalActionList",menuName ="Character/Attack/NormalAttackActionList")]
public class NormalAttackActionList_SO:ScriptableObject
{
    [SerializeField,Header("普通技能连招表")]
    private List<NormalAttackAction_SO> normalAttackActionList = new List<NormalAttackAction_SO>();


    public NormalAttackAction_SO GetAction(int index)
    {
        if (normalAttackActionList.Count == 0) return null;
        if(index < 0 || index >= normalAttackActionList.Count) return null;
        return normalAttackActionList[index];
    }

    public int ActionCount()
    {
        return normalAttackActionList.Count;
    }

}
