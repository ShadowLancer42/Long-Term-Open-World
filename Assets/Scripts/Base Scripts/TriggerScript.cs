using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    //variables
    #region

    bool doSendMessage = true;

    public enum Type
    {
        GameObject = 0,
        Tag = 1,
        Any = 2
    }
    public Type triggerType = Type.GameObject;  //What type of criterion to use to determine if the collider has been triggered

    public enum Target
    {
        other = 0,
        SpecificGameObject = 1,
        self = 3
    }
    public Target targetType = Target.other;    //where should we broadcast the message to? (other -> the object that triggered the collider, SpecificGameObject -> the gameobject 'TargetGameObject')

    public enum Call
    {
        OnTriggerEnter = 0,
        OnTriggerStay = 1,
        OnTriggerExit = 2
    }
    public Call triggerCallFunc = Call.OnTriggerEnter;

    //the 2 trigger type variables
    public GameObject TriggerObject;    //Object entering the collider
    public string TriggerTag;           //tag that the object entering the collider has

    public GameObject TargetGameObject;         //the relevant TargetGameObject variable

    public string Function;             //what function to call when triggered


    //the variables for whether you need to press a button for the sendMessage function to be called
    public bool RequireButton;      //do u need to press a button
    public string ActivationInput;  //what button to press


    public bool RepeatForever;          //should the trigger be allow to be called indefinetly?
    public int RepeatCount;             //how many times should the trigger be allowed to be called? (not used if RepatForever is set to True)



    #endregion

    //functions
    #region

    //this is cuz I'm bad at programming so i don't wanna actually optimize the code in the OnTriggerStay func, 
    //so I just put the 1 line of code that i had repeated 3 times into here, and replaced it with the func name
    //then i added the extra code in there so I wouldn't have to repeat it
    //feel free to get rid of this function and actually fix the code withing the main trigger function.
    void sendMyMessage()
    {
        if (!RequireButton || Input.GetButtonDown(ActivationInput))
        {
            //if the target isn't 'self'
            if (doSendMessage)
            {
                TargetGameObject.BroadcastMessage(Function);
            }
            else
            {
                Invoke(Function, 0f);
            }
            RepeatCount -= 1;
        }

    }

    void run(Collider other)
    {
        //if the trigger is set to repeat forever, or if the trigger has not been called the desired number of times
        if (RepeatForever == true || RepeatCount >= 0)
        {
            //set TargetGameObject to the object that entered the collider if 'target' was set to 'other'
            switch (targetType)
            {
                case Target.other:
                    TargetGameObject = other.gameObject;
                    break;

                case Target.self:
                    doSendMessage = false;
                    break;
            }

            //start triggering the collider
            switch (triggerType)
            {

                case Type.GameObject:
                    if (other.gameObject == TriggerObject)
                    {
                        sendMyMessage();
                    }
                    break;

                case Type.Tag:
                    if (other.tag == TriggerTag)
                    {
                        sendMyMessage();
                    }
                    break;

                case Type.Any:
                    sendMyMessage();
                    break;
            }


        }
    }

    //does run() based upon which value is set to on the enum Call
    //ex: it will only ever do run() when the trigger enters the collider if Call is set to OnTriggerEnter
    #region Call the function
    private void OnTriggerEnter(Collider other)
    {
        if (triggerCallFunc == Call.OnTriggerEnter)
        {
            run(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerCallFunc == Call.OnTriggerStay)
        {
            run(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerCallFunc == Call.OnTriggerExit)
        {
            run(other);
        }
    }
    #endregion
    // ^pls don't delete this.

    #endregion

}
