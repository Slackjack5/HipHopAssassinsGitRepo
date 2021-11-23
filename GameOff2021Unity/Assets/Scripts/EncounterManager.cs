using UnityEngine;

public class EncounterManager : MonoBehaviour
{
  [SerializeField] private Encounter[] encounters;
  [SerializeField] private CombatManager combatManager;

  private enum State
  {
    PreEncounter,
    InEncounter
  }

  private State currentState;
  private int currentEncounter;
  private int currentGold;

  private void Awake()
  {
    currentState = State.PreEncounter;
  }

  private void OnGUI()
  {
    if (GUI.Button(new Rect(0, Screen.height - 100, 100, 50), "Start encounter"))
    {
      StartEncounter();
    }

    GUI.Box(new Rect(Screen.width - 100, Screen.height - 100, 100, 50), $"Gold: {currentGold}");
  }

  private void StartEncounter()
  {
    if (currentState == State.InEncounter)
    {
      Debug.LogError("Failed to start encounter. We are already in an encounter!");
      return;
    }

    if (currentEncounter >= encounters.Length)
    {
      Debug.LogError($"Failed to start encounter. Index {currentEncounter} is out of bounds!");
      return;
    }

    combatManager.Begin(Instantiate(encounters[currentEncounter]));
    CombatManager.onStateChange.AddListener(OnCombatStateChange);

    currentState = State.InEncounter;
  }

  private void EndEncounter(bool isWin)
  {
    if (currentState == State.PreEncounter)
    {
      Debug.LogError("Failed to end encounter. We are not in an encounter!");
      return;
    }

    currentState = State.PreEncounter;

    if (isWin)
    {
      currentGold += encounters[currentEncounter].Gold;
      currentEncounter++;
    }
  }

  private void OnCombatStateChange(CombatManager.State state)
  {
    switch (state)
    {
      case CombatManager.State.Lose:
        EndEncounter(false);
        break;
      case CombatManager.State.Win:
        EndEncounter(true);
        break;
    }
  }
}
