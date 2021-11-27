using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
  [SerializeField] private float displayDuration;
  [SerializeField] private float distance;
  [SerializeField] private float randomBound;
  [SerializeField] private float hugeSize;
  [SerializeField] private Color hugeColor;
  [SerializeField] private float hugeThreshold;
  [SerializeField] private float largeSize;
  [SerializeField] private Color largeColor;
  [SerializeField] private float largeThreshold;
  [SerializeField] private float mediumSize;
  [SerializeField] private Color mediumColor;
  [SerializeField] private float mediumThreshold;
  [SerializeField] private float smallSize;
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
    float randomOffset = Random.Range(-randomBound, randomBound);
    Vector2 anchoredPosition = rectTransform.anchoredPosition;
    rectTransform.anchoredPosition =
      new Vector2(anchoredPosition.x + randomOffset, anchoredPosition.y + randomOffset);

    textComponent.text = value.ToString();
    textComponent.DOFade(0, displayDuration);

    if (value >= hugeThreshold)
    {
      textComponent.fontSize = hugeSize;
      textComponent.color = hugeColor;
    }
    else if (value >= largeThreshold)
    {
      textComponent.fontSize = largeSize;
      textComponent.color = largeColor;
    }
    else if (value >= mediumThreshold)
    {
      textComponent.fontSize = mediumSize;
      textComponent.color = mediumColor;
    }
    else
    {
      textComponent.fontSize = smallSize;
      textComponent.color = smallColor;
    }

    rectTransform.DOAnchorPosY(distance, displayDuration).OnComplete(() => Destroy(gameObject));
  }
}
