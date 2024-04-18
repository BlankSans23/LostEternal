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
    float playTime = 0;
    float playVolume;
    bool playing;

    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "PLAY")
            {
                if (isPlaying)
                    playTime = 0;
                playing = true;

                if (IsClient)
                    aSource.Play();
            }

            if (flag == "PAUSE")
            {
                playing = false;

                if (IsClient)
                    aSource.Pause();
            }

            if (flag == "STOP")
            {
                playing = false;
                playTime = 0;

                if (IsClient)
                    aSource.Stop();
            }
        }

        if (flag == "TIME")
        {
            aSource.time = float.Parse(value);
            playTime = float.Parse(value);
        }

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
        while (IsServer)
        {
            if (IsDirty)
            {
                time = time;
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }

    void Start()
    {
        if (aSource == null)
            aSource = GetComponent<AudioSource>();
        playing = aSource.isPlaying;
    }

    void Update()
    {
        playVolume = aSource.volume;

        if (isPlaying)
            playTime += Time.deltaTime;
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
        if (IsServer)
        {
            if (isPlaying)
                playTime = 0;
            playing = true;
        }

        SendFullUpdate("PLAY", "");
    }

    public void Pause()
    {
        if (IsServer)
            playing = false;

        SendFullUpdate("PAUSE", "");
    }

    public void Stop()
    {
        if (IsServer)
        {
            playing = false;
            playTime = 0;
        }
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
            {
                aSource.time = value;
                playTime = value;
            }
            SendFullUpdate("TIME", value.ToString());
        }
    }

    public bool isPlaying
    {
        get
        {
            return playing;
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
