using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    SelectFirstItem(itemMenu);
  }

  private void SelectFirstItem(GameObject menu)
  {
    EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
  }
}
