using UnityEngine;

public class Encounter : MonoBehaviour
{
  [SerializeField] private int gold;
  [SerializeField] private bool isShop;

  public int Gold => gold;
  public bool IsShop => isShop;
}
