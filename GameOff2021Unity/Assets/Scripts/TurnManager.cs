using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
  [SerializeField] private GameObject heroOneCard;
  [SerializeField] private GameObject heroTwoCard;
  [SerializeField] private GameObject heroThreeCard;

  private enum Turn
  {
    HERO_ONE, HERO_TWO, HERO_THREE, EXECUTION
  }

  private Turn currentTurn;

  private void Start()
  {
    currentTurn = Turn.HERO_ONE;
    DisplayCurrentTurn();
  }

  public void GoNextTurn()
  {
    switch (currentTurn)
    {
      case Turn.HERO_ONE:
        currentTurn = Turn.HERO_TWO;
        break;
      case Turn.HERO_TWO:
        currentTurn = Turn.HERO_THREE;
        break;
      case Turn.HERO_THREE:
        currentTurn = Turn.EXECUTION;
        break;
      default:
        break;
    }

    DisplayCurrentTurn();
  }

  public void GoPreviousTurn()
  {
    switch (currentTurn)
    {
      case Turn.HERO_TWO:
        currentTurn = Turn.HERO_ONE;
        break;
      case Turn.HERO_THREE:
        currentTurn = Turn.HERO_TWO;
        break;
      default:
        break;
    }

    DisplayCurrentTurn();
  }

  private void DisplayCurrentTurn()
  {
    TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
    switch (currentTurn)
    {
      case Turn.HERO_ONE:
        textComponent.text = heroOneCard.GetComponentInChildren<TextMeshProUGUI>().text;
        break;
      case Turn.HERO_TWO:
        textComponent.text = heroTwoCard.GetComponentInChildren<TextMeshProUGUI>().text;
        break;
      case Turn.HERO_THREE:
        textComponent.text = heroThreeCard.GetComponentInChildren<TextMeshProUGUI>().text;
        break;
      default:
        break;
    }
  }
}
