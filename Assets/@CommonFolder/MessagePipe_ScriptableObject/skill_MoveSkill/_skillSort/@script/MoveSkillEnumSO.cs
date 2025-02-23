
using UnityEngine;

[CreateAssetMenu(menuName = "CVariableSO/SkillEnumSO/Move")]
public class MoveSkillEnumSO : ScriptableObject
{
    public int moveNum { get; private set; }

    public void SetMoveNum(int i)
    {
        moveNum = i;
    }
}
