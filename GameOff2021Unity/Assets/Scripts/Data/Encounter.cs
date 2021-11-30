using UnityEngine;

public class Encounter : MonoBehaviour
{
  [SerializeField] private int gold;
  [SerializeField] private bool isShop;
  [SerializeField] private bool isDangerous;

  public int Gold => gold;
  public bool IsShop => isShop;
  public bool IsDangerous => isDangerous;
}
