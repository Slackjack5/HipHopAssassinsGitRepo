using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
  //FX
  [SerializeField] private GameObject AttackHit;
  [SerializeField] private GameObject MacroHit;
  public int AttackType = 0;


  public void SpawnAttackHit(Combatant Target, bool isMacro)
  {

    float randomPosition = Random.Range(-.5f, .5f);

    if(isMacro)
    {
      float randomSize = Random.Range(1f, 1.5f);
      GameObject myEffect = Instantiate(MacroHit, new Vector3(Target.transform.position.x + randomPosition, Target.transform.position.y + randomPosition, 0), Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
      AkSoundEngine.PostEvent("Play_MacroHit", gameObject);
    }
    else
    {
      float randomSize = Random.Range(.5f, 1.5f);
      GameObject myEffect = Instantiate(AttackHit, new Vector3(Target.transform.position.x + randomPosition, Target.transform.position.y + randomPosition, 0), Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
      AkSoundEngine.PostEvent("Play_AttackHit", gameObject);
    }
    
  }
}
