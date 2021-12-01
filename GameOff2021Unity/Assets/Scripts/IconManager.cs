using UnityEngine;

public class IconManager : MonoBehaviour
{
  private static BuffIcon attackBuff;
  private static BuffIcon defenseBuff;
  private static BuffIcon macroBuff;

  private void Awake()
  {
    BuffIcon[] icons = GetComponentsInChildren<BuffIcon>();
    attackBuff = icons[0];
    defenseBuff = icons[1];
    macroBuff = icons[2];

    CombatManager.onStateChange.AddListener(OnCombatStateChange);
  }

  private void OnCombatStateChange(CombatManager.State state)
  {
    switch (state)
    {
      case CombatManager.State.Inactive:
      case CombatManager.State.Lose:
      case CombatManager.State.Win:
        attackBuff.Hide();
        defenseBuff.Hide();
        macroBuff.Hide();
        break;
    }
  }

  public static void BuffAttack()
  {
    attackBuff.Upgrade();
  }

  public static void BuffDefense()
  {
    defenseBuff.Upgrade();
  }

  public static void BuffMacro()
  {
    macroBuff.Upgrade();
  }
}
