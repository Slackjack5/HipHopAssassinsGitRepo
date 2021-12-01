using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
  [SerializeField] private Sprite stageOne;
  [SerializeField] private Sprite stageTwo;

  private Image currentImage;

  private void Start()
  {
    currentImage = GetComponent<Image>();
    Hide();
  }

  public void Upgrade()
  {
    currentImage.color = Color.white;
    if (currentImage.sprite == null)
    {
      currentImage.sprite = stageOne;
    }
    else if (currentImage.sprite == stageOne)
    {
      currentImage.sprite = stageTwo;
    }
  }

  public void Hide()
  {
    currentImage.sprite = null;
    currentImage.color = Color.clear;
  }
}
