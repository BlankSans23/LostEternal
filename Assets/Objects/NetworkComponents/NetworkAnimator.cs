using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

[RequireComponent(typeof(Animator))]
public class NetworkAnimator : NetworkComponent
{
    Animator anim;

    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            string[] parameters = value.Split(',');

            if (flag == "SETBOOL")
                anim.SetBool(parameters[0], bool.Parse(parameters[1]));

            if (flag == "SETFLOAT")
                anim.SetFloat(parameters[0], float.Parse(parameters[1]));

            if (flag == "SETINT")
                anim.SetInteger(parameters[0], int.Parse(parameters[1]));

            if (flag == "SETTRIGGER")
                anim.SetTrigger(value);
        }
        //All messages will echo to clients on this component, that is the whole point
        if (IsServer)
            SendUpdate(flag, value);
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyId.UpdateFrequency);
    }


    void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    //Sends an update with the provided network message tag, and the information needed for the animator.
    //Mainly for modularity
    void SendAnimationUpdate<T>(string msgflag, string parameterName, T value)
    {
        if (IsServer)
            SendUpdate(msgflag, parameterName + "," + value.ToString());
        if (IsClient)
            SendCommand(msgflag, parameterName + "," + value.ToString());
    }

    //Mimic animation function calls, allows you to just change Animatior to NetworkAnimator with no errors assuming only this section's functions are used.
    #region ANIMATOR_CONTROLLER_CALLS
    public void SetBool(string parameterName, bool value)
    {
        SendAnimationUpdate<bool>("SETBOOL", parameterName, value);
    }

    public void SetInteger(string parameterName, int value)
    {
        SendAnimationUpdate<int>("SETINT", parameterName, value);
    }

    public void SetFloat(string parameterName, float value)
    {
        SendAnimationUpdate<float>("SETFLOAT", parameterName, value);
    }

    public void SetTrigger(string parameterName)
    {
        if (IsServer)
            SendUpdate("SETTRIGGER", parameterName);
        if (IsClient)
            SendCommand("SETTRIGGER", parameterName);
    }
    #endregion
}
