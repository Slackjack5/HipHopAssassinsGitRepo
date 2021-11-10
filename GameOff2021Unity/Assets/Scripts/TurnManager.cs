using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
  [SerializeField] private Hero heroOne;
  [SerializeField] private Hero heroTwo;
  [SerializeField] private Hero heroThree;

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
        textComponent.text = heroOne.heroName;
        break;
      case Turn.HERO_TWO:
        textComponent.text = heroTwo.heroName;
        break;
      case Turn.HERO_THREE:
        textComponent.text = heroThree.heroName;
        break;
      default:
        break;
    }
  }
}
