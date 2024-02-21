using System.Globalization;
using UnityEngine;

[CreateAssetMenu(menuName = "ActiveAbilityLevelSetup", fileName = "New ActiveAbilityLevelSetup")]
public class ActiveAbilityLevelSetupSO : ScriptableObject
{
    [field:SerializeField] public int MaxLevel { get; private set; }

    [field: SerializeField] private int[] expNeeded;

    public int GetCurrentLevel(int exp)
    {
        for (int i = 0; i < expNeeded.Length; i++)
        {
            if (exp < expNeeded[i]) return i;

            exp -= expNeeded[i];
        }

        return MaxLevel;
    }
    
    public float GetCurrentProgressInPercent(int exp)
    {
        for (int i = 0; i < expNeeded.Length; i++)
        {
            if (exp < expNeeded[i]) return exp / (float)expNeeded[i];

            exp -= expNeeded[i];
        }

        return 1;
    }
}
