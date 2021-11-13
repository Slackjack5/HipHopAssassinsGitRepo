public class Command
{
  public string Name { get; private set; }
  public string Description { get; private set; }
  public int PatternId { get; private set; }

  public Command(string name, string description, int patternId)
  {
    Name = name;
    Description = description;
    PatternId = patternId;
  }
}
