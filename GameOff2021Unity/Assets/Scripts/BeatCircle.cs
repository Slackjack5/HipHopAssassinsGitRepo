using UnityEngine;

public class BeatCircle : BeatEntity
{
  private static readonly int hit = Animator.StringToHash("Hit");

  public void Hit()
  {
    gameObject.GetComponent<Animator>().SetBool(hit, true);
  }

  public void Destroy()
  {
    Destroy(gameObject);
  }
}
