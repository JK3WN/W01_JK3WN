using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    public GameObject player;
    public RotateMap rMap;

    public bool isRight;

    void Update()
    {
        if (rMap.IsRotating) return;

        Vector3 playerPosition = player.transform.position;

        Vector3 rotatedPosition = RotatePointAroundOrigin2D(playerPosition, (isRight ? 1 : -1) * 90);

        // 회전 적용
        transform.position = rotatedPosition;
        transform.rotation = Quaternion.Euler(0, 0, isRight ? -90 : 90);
    }

    Vector3 RotatePointAroundOrigin2D(Vector3 point, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        float newX = point.x * cos + point.y * sin;
        float newY = -point.x * sin + point.y * cos;

        return new Vector3(newX, newY, 0);
    }

    public void DoMapRotate(float t)
    {
        float ratio = Mathf.Clamp01(t / 0.5f);

        float angle = isRight ? 90f - 90f * ratio : 90f * ratio;
        Vector3 playerPosition = player.transform.position;

        transform.position = RotatePointAroundOrigin2D(playerPosition, angle);
    }
}
