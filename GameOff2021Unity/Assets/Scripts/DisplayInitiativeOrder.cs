using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayInitiativeOrder : MonoBehaviour
{
  [SerializeField] private GameObject initiativeCardPrefab;
  [SerializeField] private CombatManager combatManager;

  private void Start()
  {
    ClearDisplay();

    for (int i = 0; i < combatManager.Combatants.Count; i++)
    {
      GameObject initiativeCard = Instantiate(initiativeCardPrefab, transform);

      TextMeshProUGUI[] textComponents = initiativeCard.GetComponentsInChildren<TextMeshProUGUI>();
      TextMeshProUGUI name = textComponents[0];
      TextMeshProUGUI order = textComponents[1];

      name.text = combatManager.Combatants[i].Name;
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
}
