using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerScript : MonoBehaviour
{
    public int direction;
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            Player player = coll.gameObject.GetComponent<Player>();
            if (player.isChangingRoom)
            {
                return;
            }
            player.isChangingRoom = true;
            int xChange = 0;
            int yChange = 0;
            if (direction == 0)
            {
                // Left
                xChange = -1;
                yChange = 0;
            } else if (direction == 1)
            {
                // Up
                xChange = 0;
                yChange = 1;
            } else if (direction == 2)
            {
                // Right
                xChange = 1;
                yChange = 0;
            } else if (direction == 3)
            {
                // Down
                xChange = 0;
                yChange = -1;
            } else
            {
                // Invalid
                player.isChangingRoom = false;
                return;
            }
            Room newRoom = player.changeRoom(xChange, yChange);
            GameObject newRoomObj = newRoom.getRoom();
            newRoomObj.transform.Find("Door Blocks").gameObject.SetActive(false);
            Vector3 newPosition = newRoom.cameraPosition;
            newPosition.y += 0.5f;
            newPosition.z = -10f;
            transform.parent.Find("Door Blocks").gameObject.SetActive(true);
            player.isCameraMoving = true;
            Camera.main.GetComponent<CameraMovement>().moveTo(newPosition);
        }
    }
}
