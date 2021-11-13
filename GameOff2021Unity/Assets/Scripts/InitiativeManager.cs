using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InitiativeManager : MonoBehaviour
{
  [SerializeField] private GameObject initiativeCardPrefab;
  [SerializeField] private List<Combatant> combatants;

  /// <summary>
  /// Combatants sorted in initiative order
  /// </summary>
  public List<Combatant> Combatants { get; private set; }

  private void Start()
  {
    ClearDisplay();

    Combatants = new List<Combatant>(combatants);
    Combatants.Sort(CompareCombatantSpeeds);
    for (int i = 0; i < Combatants.Count; i++)
    {
      GameObject initiativeCard = Instantiate(initiativeCardPrefab, transform);

      TextMeshProUGUI[] textComponents = initiativeCard.GetComponentsInChildren<TextMeshProUGUI>();
      TextMeshProUGUI name = textComponents[0];
      TextMeshProUGUI order = textComponents[1];

      name.text = Combatants[i].Name;
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
