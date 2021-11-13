using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InitiativeManager : MonoBehaviour
{
  [SerializeField] private GameObject initiativeCardPrefab;
  [SerializeField] private List<Combatant> combatants;

  private List<Combatant> sortedCombatants;

  /// <summary>
  /// Combatants sorted in initiative order
  /// </summary>
  public List<Combatant> Combatants
  {
    get { return sortedCombatants; }
  }

  private void Start()
  {
    ClearDisplay();

    sortedCombatants = new List<Combatant>(combatants);
    sortedCombatants.Sort(CompareCombatantSpeeds);
    for (int i = 0; i < sortedCombatants.Count; i++)
    {
      GameObject initiativeCard = Instantiate(initiativeCardPrefab, transform);

      TextMeshProUGUI[] textComponents = initiativeCard.GetComponentsInChildren<TextMeshProUGUI>();
      TextMeshProUGUI name = textComponents[0];
      TextMeshProUGUI order = textComponents[1];

      name.text = sortedCombatants[i].Name;
      order.text = (i + 1).ToString();
    }
  }

  private void ClearDisplay()
  {
    foreach (Transform child in transform)
    {
      Destroy(child.gameObject);
    }
  }

  private int CompareCombatantSpeeds(Combatant x, Combatant y)
  {
    return y.Speed.CompareTo(x.Speed);
  }
}
