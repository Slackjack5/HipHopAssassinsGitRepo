using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
  [SerializeField] private Encounter[] encounters;
  [SerializeField] private DialogueTrigger[] dialogueTriggers;
  [SerializeField] private GameObject dialoguePanel;
  [SerializeField] private float textSpeed;
  [SerializeField] private CombatManager combatManager;
  [SerializeField] private Shop shop;
  [SerializeField] private GameObject continueCommand;

  private enum State
  {
    PreEncounter,
    InEncounter
  }

  private Queue<DialogueTrigger.Dialogue> dialogueQueue = new Queue<DialogueTrigger.Dialogue>();
  private DialogueTrigger.Dialogue currentDialogue;
  private TextMeshProUGUI nameText;
  private TextMeshProUGUI messageText;
  private bool isTyping;
  private State currentState;
  private Encounter currentEncounter;
  private int currentEncounterIndex;
  private int currentGold;

  private static readonly HashSet<Consumable> consumablesOwned = new HashSet<Consumable>();

  public static IEnumerable<Command> ConsumablesOwned => consumablesOwned;

  private void Awake()
  {
    currentState = State.PreEncounter;

    var button = continueCommand.GetComponentInChildren<Button>();
    button.onClick.AddListener(DisplayNextDialogue);

    CombatManager.onStateChange.AddListener(OnCombatStateChange);

    shop.onPurchase.AddListener(OnPurchase);
    shop.onClose.AddListener(gold =>
    {
      currentGold = gold;
      EndEncounter(true);
    });

    TextMeshProUGUI[] textComponents = dialoguePanel.GetComponentsInChildren<TextMeshProUGUI>();
    nameText = textComponents[0];
    messageText = textComponents[1];

    StartDialogue();
  }

  private void Update()
  {
    if (currentState == State.PreEncounter)
    {
      continueCommand.SetActive(true);
      EventSystem.current.SetSelectedGameObject(continueCommand.GetComponentInChildren<Button>().gameObject);
    }
    else
    {
      continueCommand.SetActive(false);
    }
  }

  private void StartDialogue()
  {
    if (currentState == State.InEncounter)
    {
      Debug.LogError("Failed to start dialogue. We are in an encounter!");
      return;
    }

    if (currentEncounterIndex >= dialogueTriggers.Length)
    {
      Debug.LogWarning(
        $"Failed to start dialogue. Index {currentEncounterIndex} is out of bounds. Will start the next encounter instead.");
      StartEncounter();
      return;
    }

    dialogueQueue.Clear();
    DialogueTrigger dialogueTrigger = dialogueTriggers[currentEncounterIndex];
    foreach (DialogueTrigger.Dialogue dialogue in dialogueTrigger.Dialogues)
    {
      dialogueQueue.Enqueue(dialogue);
    }

    DisplayNextDialogue();
  }

  private void DisplayNextDialogue()
  {
    if (isTyping)
    {
      // Stop typing the current message before typing a new message.
      StopAllCoroutines();
      isTyping = false;
      messageText.text = currentDialogue.message;
      return;
    }

    if (dialogueQueue.Count == 0)
    {
      StartEncounter();
      return;
    }

    dialoguePanel.SetActive(true);

    currentDialogue = dialogueQueue.Dequeue();
    nameText.text = currentDialogue.name;

    StartCoroutine(TypeMessage(currentDialogue.message));
  }

  private IEnumerator TypeMessage(string message)
  {
    isTyping = true;
    messageText.text = "";
    foreach (char letter in message)
    {
      messageText.text += letter;
      yield return new WaitForSeconds(textSpeed);
    }

    isTyping = false;
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

    dialoguePanel.SetActive(false);

    // Reset the current encounter.
    if (currentEncounter != null)
    {
      Destroy(currentEncounter.gameObject);
    }

    currentEncounter = Instantiate(encounters[currentEncounterIndex]);

    if (currentEncounterIndex == 0)
    {
      foreach (Hero hero in CombatManager.Heroes)
      {
        hero.ResetEverything(true);
      }
    }

    if (currentEncounter.IsShop)
    {
      shop.Open(currentGold);
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
    else
    {
      currentGold = 0;
      currentEncounterIndex = 0;
    }

    combatManager.Reset();
    Timer.Deactivate();
    currentState = State.PreEncounter;
    StartDialogue();
  }

  private void OnCombatStateChange(CombatManager.State state)
  {
    switch (state)
    {
      case CombatManager.State.EndLose:
        EndEncounter(false);
        break;
      case CombatManager.State.EndWin:
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
