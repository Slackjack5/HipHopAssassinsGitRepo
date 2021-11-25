[System.Serializable]
public class Consumable : Command
{
  public int id;
  public int cost;

  public override void Execute(Combatant actor)
  {
    throw new System.NotImplementedException();
  }
}
