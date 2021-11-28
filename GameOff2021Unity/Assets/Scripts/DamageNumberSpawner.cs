using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
  [SerializeField] private GameObject damageNumberPrefab;
  [SerializeField] private float spawnOffsetBound;

  private static RectTransform rectTransform;
  private static GameObject _damageNumberPrefab;
  private static float _spawnOffsetBound;

  private void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
    _damageNumberPrefab = damageNumberPrefab;
    _spawnOffsetBound = spawnOffsetBound;
  }

  public static void Spawn(int value)
  {
    float randomOffset = Random.Range(-_spawnOffsetBound, _spawnOffsetBound);
    Vector2 position = rectTransform.anchoredPosition;
    GameObject obj = Instantiate(_damageNumberPrefab, rectTransform);
    obj.GetComponent<RectTransform>().anchoredPosition =
      new Vector2(position.x + randomOffset, position.y + randomOffset);
    obj.GetComponent<DamageNumber>().value = value;
  }
}
