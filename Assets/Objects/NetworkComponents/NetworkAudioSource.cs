using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

//Treat this the same way you would an audio source, one per sound
//Be specific when dragging in the editor, drag the right one.
//Note: This component will only autosync Play(), Pause(), Stop(), volume, and time. If you need more, feel free to add it here.
public class NetworkAudioSource : NetworkComponent
{
    [Tooltip("Drag any audio source here, not doing so will get the first AudioSource component on this object!")]
    [SerializeField] AudioSource aSource;

    //These are solely for reading time and volume;
    float playTime;
    float playVolume;

    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "PLAY")
                aSource.Play();

            if (flag == "PAUSE")
                aSource.Pause();

            if (flag == "STOP")
                aSource.Stop();
        }

        if (flag == "TIME")
            aSource.time = float.Parse(value);

        //NOTE: VOLUME WILL BE SYNCED ON ALL CLIENTS THIS IS MEANT FOR SYNCED SFX
        if (flag == "VOL")
            aSource.volume = float.Parse(value);

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
        if (aSource == null)
            aSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        playTime = aSource.time;
        playVolume = aSource.volume;
    }

    //Sends the data regardless of origin, cancels Towle's separation
    void SendFullUpdate(string flag, string value)
    {
        if (IsServer)
            SendUpdate(flag, value);
        if (IsClient)
            SendCommand(flag, value);
    }

    //Mimic AudioSourceCalls
    #region AUDIO_SOURCE_CALLS
    public void Play()
    {
        SendFullUpdate("PLAY", "");
    }

    public void Pause()
    {
        SendFullUpdate("PAUSE", "");
    }

    public void Stop()
    {
        SendFullUpdate("STOP", "");
    }

    public float time
    {
        get
        {
            return playTime;
        }
        set
        {
            if (IsServer)
                aSource.time = value;
            SendFullUpdate("TIME", value.ToString());
        }
    }

    public float volume
    {
        get
        {
            return playVolume;
        }
        set
        {
            if (IsServer)
                aSource.volume = value;
            SendFullUpdate("VOL", value.ToString());
        }
    }
    #endregion
}
