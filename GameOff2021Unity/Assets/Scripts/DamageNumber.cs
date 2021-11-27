using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
  [SerializeField] private float displayDuration;
  [SerializeField] private float distance;
  [SerializeField] private Color hugeColor;
  [SerializeField] private Color largeColor;
  [SerializeField] private Color mediumColor;
  [SerializeField] private Color smallColor;

  [HideInInspector] public int value;

  private RectTransform rectTransform;
  private TextMeshProUGUI textComponent;

  private void Start()
  {
    rectTransform = GetComponent<RectTransform>();
    textComponent = GetComponent<TextMeshProUGUI>();

    Display();
  }

  private void Display()
  {
    textComponent.text = value.ToString();
    textComponent.DOFade(0, displayDuration);

    if (value >= 16)
    {
      textComponent.fontSize = 64;
      textComponent.color = hugeColor;
    }
    else if (value >= 8)
    {
      textComponent.fontSize = 36;
      textComponent.color = largeColor;
    }
    else if (value >= 4)
    {
      textComponent.fontSize = 24;
      textComponent.color = mediumColor;
    }
    else
    {
      textComponent.fontSize = 14;
      textComponent.color = smallColor;
    }

    rectTransform.DOAnchorPosY(distance, displayDuration).OnComplete(() => Destroy(gameObject));
  }
}
