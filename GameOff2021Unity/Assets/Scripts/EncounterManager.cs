using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
  [SerializeField] private Encounter[] encounters;
  [SerializeField] private CombatManager combatManager;
  [SerializeField] private Shop shop;

  private enum State
  {
    PreEncounter,
    InEncounter
  }

  private State currentState;
  private Encounter currentEncounter;
  private int currentEncounterIndex;
  private int currentGold = 12;

  private static readonly HashSet<Consumable> consumablesOwned = new HashSet<Consumable>();

  public static IEnumerable<Command> ConsumablesOwned => consumablesOwned;

  private void Awake()
  {
    currentState = State.PreEncounter;

    CombatManager.onStateChange.AddListener(OnCombatStateChange);
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

    if (currentEncounterIndex >= encounters.Length)
    {
      Debug.LogError($"Failed to start encounter. Index {currentEncounterIndex} is out of bounds!");
      return;
    }

    // Reset the current encounter.
    if (currentEncounter != null)
    {
      Destroy(currentEncounter.gameObject);
    }

    currentEncounter = Instantiate(encounters[currentEncounterIndex]);

    if (currentEncounter.IsShop)
    {
      shop.Open(currentGold);
      shop.onPurchase.AddListener(OnPurchase);
      shop.onClose.AddListener(gold =>
      {
        currentGold = gold;
        EndEncounter(true);
      });
    }
    else
    {
      combatManager.Begin(currentEncounter);
    }

    currentState = State.InEncounter;
  }

  private void EndEncounter(bool isWin)
  {
    if (currentState == State.PreEncounter)
    {
      Debug.LogError("Failed to end encounter. We are not in an encounter!");
      return;
    }

    if (isWin)
    {
      currentGold += encounters[currentEncounterIndex].Gold;
      currentEncounterIndex++;
    }

    combatManager.Reset();
    Timer.Deactivate();
    currentState = State.PreEncounter;
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

  private void OnPurchase(Consumable consumable)
  {
    if (!consumablesOwned.Contains(consumable))
    {
      consumablesOwned.Add(consumable);
    }

    consumable.IncrementAmountOwned();
  }
}
