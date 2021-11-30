using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInitiativeOrder : MonoBehaviour
{
  [SerializeField] private GameObject initiativeCardPrefab;
  [SerializeField] private Sprite heroOneCard;
  [SerializeField] private Sprite heroTwoCard;
  [SerializeField] private Sprite heroThreeCard;
  [SerializeField] private Sprite monsterCard;

  private void Start()
  {
    ClearDisplay();

    CombatManager.onStateChange.AddListener(OnCombatStateChange);
  }

  private void OnCombatStateChange(CombatManager.State state)
  {
    switch (state)
    {
      case CombatManager.State.Start:
        ClearDisplay();

        for (var i = 0; i < CombatManager.Combatants.Count; i++)
        {
          Combatant combatant = CombatManager.Combatants[i];
          GameObject initiativeCard = Instantiate(initiativeCardPrefab, transform);

          // Background image should be the second image component.
          Image image = initiativeCard.GetComponentsInChildren<Image>()[1];
          if (combatant is Hero hero)
          {
            switch (hero.HeroId)
            {
              case 1:
                image.sprite = heroOneCard;
                break;
              case 2:
                image.sprite = heroTwoCard;
                break;
              case 3:
                image.sprite = heroThreeCard;
                break;
              default:
                Debug.LogError("Failed to set sprite of initiative card for hero " + hero.HeroId);
                break;
            }
          }
          else
          {
            image.sprite = monsterCard;
          }

          TextMeshProUGUI[] textComponents = initiativeCard.GetComponentsInChildren<TextMeshProUGUI>();
          TextMeshProUGUI combatantName = textComponents[0];
          TextMeshProUGUI order = textComponents[1];

          combatantName.text = combatant.Name;
          order.text = (i + 1).ToString();
        }

        break;
      case CombatManager.State.Inactive:
      case CombatManager.State.Lose:
      case CombatManager.State.Win:
        ClearDisplay();
        break;
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
