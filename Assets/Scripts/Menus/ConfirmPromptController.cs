using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPromptController : PopupMenuController
{
    public bool Confirmed { private set; get; }
    public bool Responded { private set; get; }

    // Start is called before the first frame update
    void Start()
    {
        Responded = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        Responded = false;
        Debug.Log("Show prompt");
    }

    public void ChooseOption()
    {
        Debug.Log("Hide prompt");
        Responded = true;
    }
}
