using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
  [SerializeField] private GameObject topMenu;
  [SerializeField] private GameObject itemMenu;

  private void Awake()
  {
    OpenTopMenu();
  }

  public void OpenTopMenu()
  {
    topMenu.SetActive(true);
    itemMenu.SetActive(false);

    SelectFirstCommand(topMenu);
  }

  public void OpenItemMenu()
  {
    topMenu.SetActive(false);
    itemMenu.SetActive(true);

    SelectFirstCommand(itemMenu);
  }

  private void SelectFirstCommand(GameObject menu)
  {
    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
  }
}
