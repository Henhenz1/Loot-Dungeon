using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestChamber : MonoBehaviour
{
    public void reload()
    {
        SceneManager.LoadScene(2);
    }
}
