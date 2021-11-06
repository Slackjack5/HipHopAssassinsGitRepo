using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
  [SerializeField] private GameObject topMenu;
  [SerializeField] private GameObject itemMenu;

  private void Start()
  {
    topMenu.SetActive(true);
    itemMenu.SetActive(false);
  }

  public void OpenItemMenu()
  {
    topMenu.SetActive(false);
    itemMenu.SetActive(true);
  }
}
