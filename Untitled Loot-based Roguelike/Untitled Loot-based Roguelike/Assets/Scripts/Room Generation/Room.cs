using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 gridPos;

    public int type;

    public bool doorTop, doorBot, doorLeft, doorRight;

    public Vector3 cameraPosition;

    public bool isStartingRoom;

    public bool isChestRoom;

    public bool isSafeRoom;

    public bool isEndingRoom;

    private GameObject roomObj;

    public Room(Vector2 _gridPos, int _type)
    {
        gridPos = _gridPos;
        type = _type;
        isStartingRoom = false;
    }

    public GameObject getRoom()
    {
        return roomObj;
    }

    public void setRoom(GameObject room)
    {
        roomObj = room;
    }

    public void setEnding()
    {
        isEndingRoom = true;
    }
}
