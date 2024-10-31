using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    private float smoothing = 0.2f;
    public Vector2 minCameraBoundary;
    public Vector2 maxCameraBoundary;

    private void FixedUpdate() {
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, this.transform.position.z);

        targetPos.x = Mathf.Clamp(targetPos.x, minCameraBoundary.x, maxCameraBoundary.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minCameraBoundary.y, maxCameraBoundary.y);

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }
}
