public class Command
{
  public enum Type
  {
    Attack,
    Consumable,
    Macro,
    Defend,
    Charge
  }

  public string Name { get; private set; }
  public string Description { get; private set; }
  public Type CommandType { get; private set; }
  public int PatternId { get; private set; }

  public Command(string name, string description, Type commandType, int patternId)
  {
    Name = name;
    Description = description;
    CommandType = commandType;
    PatternId = patternId;
  }
}
