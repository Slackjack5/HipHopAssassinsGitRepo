using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
  [SerializeField] private Hero heroOne;
  [SerializeField] private Hero heroTwo;
  [SerializeField] private Hero heroThree;

  private enum State
  {
    START, HERO_ONE, HERO_TWO, HERO_THREE, EXECUTION
  }

  private State currentState;

  private void Start()
  {
    currentState = State.HERO_ONE;
    DisplayCurrentTurn();
  }

  public void GoNextTurn()
  {
    switch (currentState)
    {
      case State.HERO_ONE:
        currentState = State.HERO_TWO;
        break;
      case State.HERO_TWO:
        currentState = State.HERO_THREE;
        break;
      case State.HERO_THREE:
        currentState = State.EXECUTION;
        break;
      default:
        break;
    }

    DisplayCurrentTurn();
  }

  public void GoPreviousTurn()
  {
    switch (currentState)
    {
      case State.HERO_TWO:
        currentState = State.HERO_ONE;
        break;
      case State.HERO_THREE:
        currentState = State.HERO_TWO;
        break;
      default:
        break;
    }

    DisplayCurrentTurn();
  }

  private void DisplayCurrentTurn()
  {
    TextMeshProUGUI textComponent = GetComponentInChildren<TextMeshProUGUI>();
    switch (currentState)
    {
      case State.HERO_ONE:
        textComponent.text = heroOne.combatantName;
        break;
      case State.HERO_TWO:
        textComponent.text = heroTwo.combatantName;
        break;
      case State.HERO_THREE:
        textComponent.text = heroThree.combatantName;
        break;
      default:
        break;
    }
  }
}
