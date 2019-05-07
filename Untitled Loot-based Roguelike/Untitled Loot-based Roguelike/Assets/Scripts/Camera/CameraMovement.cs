using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
    }

    public void moveTo(Vector3 newPosition)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveCamera(newPosition));
        }
    }

    IEnumerator MoveCamera(Vector3 newPosition)
    {
        Vector3 start = transform.position;
        float elapsedTime = 0;
        isMoving = true;
        while (transform.position != newPosition)
        {
            Vector3 newPos = Vector3.Lerp(start, newPosition, elapsedTime / 1);
            transform.position = newPos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isMoving = false;
        GameObject.Find("Player").GetComponent<Player>().isCameraMoving = false;
    }
}
