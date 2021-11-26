public abstract class Command
{
  public string name;
  public string description;
  public int patternId;
  public bool selectMonster;
  public bool needsTarget;

  protected Combatant Target { get; private set; }

  public void SetTarget(Combatant combatant)
  {
    Target = combatant;
  }

  public abstract void Execute(Combatant actor);
}
