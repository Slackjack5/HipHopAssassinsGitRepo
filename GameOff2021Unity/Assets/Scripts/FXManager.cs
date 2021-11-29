using UnityEngine;
using EZCameraShake;
using Random = UnityEngine.Random;

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
  [SerializeField] private GameObject PerfectHit;
  [SerializeField] private GameObject GoodHit;
  [SerializeField] private GameObject Stars;
  [SerializeField] private GameObject HurtFX;

  private static float _cameraShakeMagnitude;
  private static float _cameraShakeRoughness;
  private static float _cameraShakeFadeIn;
  private static float _cameraShakeFadeOut;
  private static GameObject _attackHit;
  private static GameObject _macroHit;
  private static GameObject _macroPulse;
  private static GameObject _buffOffense;
  private static GameObject _perfectHit;
  private static GameObject _goodHit;
  private static GameObject _stars;
  private static GameObject _hurtFX;
  private static GameObject fxManager;

  private void Awake()
  {
    _cameraShakeMagnitude = cameraShakeMagnitude;
    _cameraShakeRoughness = cameraShakeRoughness;
    _cameraShakeFadeIn = cameraShakeFadeIn;
    _cameraShakeFadeOut = cameraShakeFadeOut;
    _attackHit = AttackHit;
    _macroHit = MacroHit;
    _macroPulse = MacroPulse;
    _buffOffense = BuffOffense;
    _perfectHit = PerfectHit;
    _goodHit = GoodHit;
    _stars = Stars;
    _hurtFX = HurtFX;

    fxManager = gameObject;
  }

  public static void SpawnAttackHit(Combatant Target, bool isMacro)
  {
    float randomPosition = Random.Range(-.5f, .5f);

    if (isMacro)
    {
      float randomSize = Random.Range(1f, 1.5f);
      Vector3 position = Target.transform.position;
      GameObject myEffect = Instantiate(_macroHit,
        new Vector3(position.x + randomPosition, position.y + randomPosition, 0),
        Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
      AkSoundEngine.PostEvent("Play_MacroHit", fxManager);
    }
    else
    {
      float randomSize = Random.Range(.5f, 1.5f);
      Vector3 position = Target.transform.position;
      GameObject myEffect = Instantiate(_attackHit,
        new Vector3(position.x + randomPosition, position.y + randomPosition, 0),
        Quaternion.identity);
      myEffect.transform.localScale = new Vector3(randomSize, randomSize, 0);
      AkSoundEngine.PostEvent("Play_AttackHit", fxManager);
    }

    CameraShaker.Instance.ShakeOnce(_cameraShakeMagnitude, _cameraShakeRoughness, _cameraShakeFadeIn,
      _cameraShakeFadeOut);
  }

  public static void SpawnMacroPulse(Combatant Caster)
  {
    Vector3 position = Caster.transform.position;
    Instantiate(_macroPulse,
      new Vector3(position.x, position.y, 0), Quaternion.identity);
  }

  public static void SpawnBuffOffense(Combatant Target)
  {
    Vector3 position = Target.transform.position;
    Instantiate(_buffOffense,
      new Vector3(position.x, position.y, 0), Quaternion.identity);
  }

  public static void SpawnBuffDefense(Combatant Target)
  {
    Vector3 position = Target.transform.position;
    Instantiate(_macroPulse,
      new Vector3(position.x, position.y, 0), Quaternion.identity);
  }

  public static void SpawnPerfectHit()
  {
    Instantiate(_perfectHit, Vector3.zero, Quaternion.identity);
    Instantiate(_stars, Vector3.zero, Quaternion.identity);
  }

  public static void SpawnGreatHit()
  {
    Instantiate(_goodHit, Vector3.zero, Quaternion.identity);
  }

  public static void SpawnHurtFX()
  {
    Instantiate(_hurtFX, Vector3.zero, Quaternion.identity);
  }
}
