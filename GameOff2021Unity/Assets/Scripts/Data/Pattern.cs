[System.Serializable]
public class Pattern
{
  [System.Serializable]
  public class Beat
  {
    public float beatNumber;
    public string soundName;
    public bool isCall;
  }

  public int id;
  public Beat[] beats;
  public int barLength;
}
