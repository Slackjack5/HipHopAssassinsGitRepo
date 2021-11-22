public abstract class Command
{
  public string name;
  public string description;
  public int patternId;

  protected Combatant Target { get; private set; }

  public void SetTarget(Combatant combatant)
  {
    Target = combatant;
  }

  public abstract void Execute(Combatant actor);
}
