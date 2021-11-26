using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
  //FX
  [SerializeField] private GameObject AttackHit;
  [SerializeField] private GameObject MacroHit;
  public int AttackType = 0;


  public void SpawnAttackHit(Combatant Target, Combatant Caster)
  {
    float randomSize = Random.Range(.5f, 1.5f);
    float randomPosition = Random.Range(-.5f, .5f);
    Debug.Log(Caster is Hero);
    if(Caster is Hero hero && hero.GetSubmittedCommand() is Macro)
    {
      GameObject myEffect = Instantiate(MacroHit, new Vector3(Target.transform.position.x + randomPosition, Target.transform.position.y + randomPosition, 0), Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
      
    }
    else
    {
      GameObject myEffect = Instantiate(AttackHit, new Vector3(Target.transform.position.x + randomPosition, Target.transform.position.y + randomPosition, 0), Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
    }
    
  }
}
