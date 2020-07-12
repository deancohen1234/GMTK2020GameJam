using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Opening_Controller : MonoBehaviour
{
    public bool MenuActive;
    public Animator OpeningAnimator;

    // Start is called before the first frame update
    void Start()
    {
        MenuActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (MenuActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OpeningAnimator.SetBool("StartGame", true);
            }
        }
    }

    public void StartMenu()
    {
        MenuActive = true;
    }

    public void Transition()
    {
        SceneManager.LoadScene("Lawn Mover");
    }
}
