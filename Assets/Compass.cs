using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public Transform playerTransform;
    public Transform goalTransform;
    public RectTransform rectTransform;

    void Update()
    {
        Vector3 direction = goalTransform.position - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }
}
