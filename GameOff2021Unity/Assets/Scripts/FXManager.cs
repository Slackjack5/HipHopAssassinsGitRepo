using UnityEngine;
using EZCameraShake;

public class FXManager : MonoBehaviour
{
  //FX
  [SerializeField] private GameObject AttackHit;
  [SerializeField] private GameObject MacroHit;
  [SerializeField] private GameObject MacroPulse;
  [SerializeField] private GameObject BuffOffense;
  [SerializeField] private float cameraShakeMagnitude;
  [SerializeField] private float cameraShakeRoughness;
  [SerializeField] private float cameraShakeFadeIn;
  [SerializeField] private float cameraShakeFadeOut;

  public void SpawnAttackHit(Combatant Target, bool isMacro)
  {
    float randomPosition = Random.Range(-.5f, .5f);

    if (isMacro)
    {
      float randomSize = Random.Range(1f, 1.5f);
      GameObject myEffect = Instantiate(MacroHit,
        new Vector3(Target.transform.position.x + randomPosition, Target.transform.position.y + randomPosition, 0),
        Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
      AkSoundEngine.PostEvent("Play_MacroHit", gameObject);
    }
    else
    {
      float randomSize = Random.Range(.5f, 1.5f);
      GameObject myEffect = Instantiate(AttackHit,
        new Vector3(Target.transform.position.x + randomPosition, Target.transform.position.y + randomPosition, 0),
        Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
      AkSoundEngine.PostEvent("Play_AttackHit", gameObject);
    }

    CameraShaker.Instance.ShakeOnce(cameraShakeMagnitude, cameraShakeRoughness, cameraShakeFadeIn, cameraShakeFadeOut);
  }

  public void SpawnMacroPulse(Combatant Caster)
  {
    GameObject myEffect = Instantiate(MacroPulse,
      new Vector3(Caster.transform.position.x, Caster.transform.position.y, 0), Quaternion.identity);
  }

  public void SpawnBuffOffense(Combatant Target)
  {
    GameObject myEffect = Instantiate(BuffOffense,
      new Vector3(Target.transform.position.x, Target.transform.position.y, 0), Quaternion.identity);
  }

  public void SpawnBuffDefense(Combatant Target)
  {
    GameObject myEffect = Instantiate(MacroPulse,
      new Vector3(Target.transform.position.x, Target.transform.position.y, 0), Quaternion.identity);
  }
}
