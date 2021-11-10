using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InitiativeManager : MonoBehaviour
{
  [SerializeField] private GameObject initiativeCardPrefab;
  [SerializeField] private List<Combatant> combatants;

  private void Start()
  {
    ClearDisplay();

    combatants.Sort(CompareCombatantSpeeds);
    for (int i = 0; i < combatants.Count; i++)
    {
      GameObject initiativeCard = Instantiate(initiativeCardPrefab, transform);

      TextMeshProUGUI[] textComponents = initiativeCard.GetComponentsInChildren<TextMeshProUGUI>();
      TextMeshProUGUI name = textComponents[0];
      TextMeshProUGUI order = textComponents[1];

      name.text = combatants[i].combatantName;
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
    return y.speed.CompareTo(x.speed);
  }
}
