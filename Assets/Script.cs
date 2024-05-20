using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script : MonoBehaviour
{
   

  public  void oyna()
    {
        SceneManager.LoadScene(1);
    }

    public void cýk()
    {
        Application.Quit();
    }
    
}
