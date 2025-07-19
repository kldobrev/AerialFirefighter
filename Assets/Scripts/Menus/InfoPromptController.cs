using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPromptController : ConfirmPromptController
{

    public override void GiveResponse()
    {
        Responded = true;
    }

}
