using UnityEngine;

[System.Serializable]
public class Consumable : Command
{
  public int id;
  public int cost;

  private int amountOwned;

  public int AmountOwned => amountOwned;

  public override void Execute(Combatant actor)
  {
    if (amountOwned == 0)
    {
      Debug.LogError($"Failed to use consumable {name}. Amount owned is 0!");
      return;
    }

    throw new System.NotImplementedException();
  }
}
