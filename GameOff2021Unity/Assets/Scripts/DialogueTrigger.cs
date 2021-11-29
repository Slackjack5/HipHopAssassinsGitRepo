using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
  [System.Serializable]
  public class Dialogue
  {
    public string name;
    [TextArea] public string message;
  }

  [SerializeField] private Dialogue[] dialogues;

  public Dialogue[] Dialogues => dialogues;
}
