using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
  [SerializeField] private float displayDuration;
  [SerializeField] private float distance;

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

    rectTransform.DOAnchorPosY(distance, displayDuration).OnComplete(() => Destroy(gameObject));
  }
}
