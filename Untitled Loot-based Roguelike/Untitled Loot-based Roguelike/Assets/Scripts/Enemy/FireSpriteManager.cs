using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpriteManager : MonoBehaviour
{
    #region Cached Reference
    private BurningEffect burningeffect;
    private GameObject fireSprite;
    #endregion

    #region Private Variables
    private int numFires;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        burningeffect = GetComponent<BurningEffect>();
        fireSprite = transform.GetChild(0).gameObject;
        numFires = 1;
    }

    // Update is called once per frame
    void Update()
    {
        while (numFires != burningeffect.BurnStack)
        {
            if (numFires < burningeffect.BurnStack)
            {
                addFireSprite();
                numFires++;
            } else
            {
                minusFireSprite();
                numFires--;
            }
        }
    }

    private void minusFireSprite()
    {
        int childCount = transform.childCount;
        if (childCount > 1)
        {
            GameObject toBeRemoved = transform.GetChild(childCount - 1).gameObject;
            Destroy(toBeRemoved);
        }
    }

    private void addFireSprite()
    {
        GameObject newFire = Instantiate(fireSprite, gameObject.transform);
        float hShift = Random.Range(-0.5f, 0.5f);
        float vShift = Random.Range(-0.5f, 1);
        Vector2 shift = new Vector2(hShift, vShift);
        newFire.transform.position += (Vector3)shift;
    }
}
