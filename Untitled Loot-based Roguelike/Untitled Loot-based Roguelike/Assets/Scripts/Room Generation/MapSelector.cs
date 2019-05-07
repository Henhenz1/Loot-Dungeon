using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelector : MonoBehaviour
{
    public GameObject spU, spD, spR, spL,
            spUD, spLR, spUR, spUL, spDR, spDL,
            spULD, spULR, spUDR, spDLR, spULDR;
    public bool up, down, left, right;
    public int type; // 0: normal, 1: enter
    public Color normalColor, enterColor;
    Color mainColor;
    SpriteRenderer rend;
    private GameObject roomToReturn;
    void Start()
    {
        PickRoom();
    }
    public GameObject getRoom()
    {
        return roomToReturn;
    }
    public void PickRoom()
    { //picks correct room based on the four door bools
        if (up)
        {
            if (down)
            {
                if (right)
                {
                    if (left)
                    {
                        roomToReturn = spULDR;
                    }
                    else
                    {
                        roomToReturn = spUDR;
                    }
                }
                else if (left)
                {
                    roomToReturn = spULD;
                }
                else
                {
                    roomToReturn = spUD;
                }
            }
            else
            {
                if (right)
                {
                    if (left)
                    {
                        roomToReturn = spULR;
                    }
                    else
                    {
                        roomToReturn = spUR;
                    }
                }
                else if (left)
                {
                    roomToReturn = spUL;
                }
                else
                {
                    roomToReturn = spU;
                }
            }
            return;
        }
        if (down)
        {
            if (right)
            {
                if (left)
                {
                    roomToReturn = spDLR;
                }
                else
                {
                    roomToReturn = spDR;
                }
            }
            else if (left)
            {
                roomToReturn = spDL;
            }
            else
            {
                roomToReturn = spD;
            }
            return;
        }
        if (right)
        {
            if (left)
            {
                roomToReturn = spLR;
            }
            else
            {
                roomToReturn = spR;
            }
        }
        else
        {
            roomToReturn = spL;
        }
    }
}
