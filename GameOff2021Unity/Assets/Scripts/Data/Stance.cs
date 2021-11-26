using UnityEngine;

public class Stance : Command
{
  public int id;
  public int power;

  public override void Execute(Combatant actor)
  {
    if (actor is Hero hero)
    {
      switch (id)
      {
        case 1:
          // Defend
          hero.Defend();
          break;
        case 2:
          // Charge
          Timer.Add(power);
          break;
      }
    }
    else
    {
      Debug.LogError($"Failed to execute stance {name} for actor {actor.Name}. Actor is not a Hero!");
    }
  }
}
