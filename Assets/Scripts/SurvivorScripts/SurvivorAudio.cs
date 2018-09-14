using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class SurvivorAudio : NetworkBehaviour
{
    public AudioMixerGroup survivorAudioMixerGroup;
    public AudioClip walkSound_c;
    public AudioClip heartSound_c;
    public AudioClip jumpSound_c;
    public AudioClip[] behitSound_c;
    public AudioClip[] deathSound_c;
    public AudioClip waterWalkSound_c;

    private AudioSource walkSound;
    private AudioSource heartSound;
    private AudioSource jumpSound;
    private AudioSource[] behitSound;
    private AudioSource[] deathSound;
    private AudioSource waterWalkSound;

    SurvivorController survivorController;
    SurvivorStatus survivorStatus;

    bool walkSoundFlag = false; //걷는 소리가 재생되고 있다면 true
    bool runSoundFlag = false;
    bool sneakSoundFlag = false;
    bool heartSoundFlag = false;
    bool waterWalkSoundFlag = false;
    bool waterRunSoundFlag = false;
    bool waterSneakSoundFlag = false;

    // Use this for initialization
    void Start()
    {
        walkSound = SetUpEngineAudioSource(walkSound_c, 0.5f, true);
        heartSound = SetUpEngineAudioSource(heartSound_c, 1f, true);
        jumpSound = SetUpEngineAudioSource(jumpSound_c, 0.5f, false);
        waterWalkSound = SetUpEngineAudioSource(waterWalkSound_c, 0.3f, true);

        behitSound = new AudioSource[behitSound_c.Length];
        for (int i = 0; i < behitSound_c.Length; i++)
            behitSound[i] = SetUpEngineAudioSource(behitSound_c[i], 0.15f, false);

        deathSound = new AudioSource[deathSound_c.Length];
        for (int i = 0; i < deathSound_c.Length; i++)
            deathSound[i] = SetUpEngineAudioSource(deathSound_c[i], 0.5f, false);
        
        survivorController = GetComponent<SurvivorController>();
        survivorStatus = GetComponent<SurvivorStatus>();
    }

    private AudioSource SetUpEngineAudioSource(AudioClip clip, float volume, bool loop)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.outputAudioMixerGroup = survivorAudioMixerGroup;

        source.volume = volume;
        source.playOnAwake = false;
        source.loop = loop;
        source.pitch = 1f;
        source.spatialBlend = 1f;

        source.dopplerLevel = 1;
        source.rolloffMode = AudioRolloffMode.Custom;
        source.minDistance = 0;
        source.maxDistance = 50;

        return source;
    }

    //걷는 소리 시작
    public void PlayWalkSound()
    {
        CmdPlayWalkSound();
    }

    [Command]
    public void CmdPlayWalkSound()
    {
        RpcPlayWalkSound();
        if (!walkSoundFlag)
        {
            walkSound.pitch = 0.7f;
            walkSound.Play();
            walkSoundFlag = true;
        }
    }

    [ClientRpc]
    public void RpcPlayWalkSound()
    {
        if (!walkSoundFlag)
        {
            walkSound.pitch = 0.7f;
            walkSound.Play();
            walkSoundFlag = true;
        }
    }

    //걷는 소리 정지
    public void StopWalkSound()
    {
        CmdStopWalkSound();
    }

    [Command]
    public void CmdStopWalkSound()
    {
        RpcStopWalkSound();
        if (walkSoundFlag)
        {
            walkSound.Stop();
            walkSoundFlag = false;
            runSoundFlag = false;
            sneakSoundFlag = false;
        }
    }

    [ClientRpc]
    public void RpcStopWalkSound()
    {
        if (walkSoundFlag)
        {
            walkSound.Stop();
            walkSoundFlag = false;
            runSoundFlag = false;
            sneakSoundFlag = false;
        }
    }

    //뛰는 소리 시작
    public void PlayRunSound()
    {
        CmdPlayRunSound();
    }

    [Command]
    public void CmdPlayRunSound()
    {
        RpcPlayRunSound();
        if (!runSoundFlag)
        {
            walkSound.pitch = 1f;
            runSoundFlag = true;
        }
    }

    [ClientRpc]
    public void RpcPlayRunSound()
    {
        if (!runSoundFlag)
        {
            walkSound.pitch = 1f;
            runSoundFlag = true;
        }
    }

    //뛰는 소리 정지
    public void StopRunSound()
    {
        CmdStopRunSound();
    }

    [Command]
    public void CmdStopRunSound()
    {
        RpcStopRunSound();
        if (runSoundFlag)
        {
            walkSound.pitch = 0.7f;
            runSoundFlag = false;
        }
    }

    [ClientRpc]
    public void RpcStopRunSound()
    {
        if (runSoundFlag)
        {
            walkSound.pitch = 0.7f;
            runSoundFlag = false;
        }
    }

    //앉아서 걷는 소리 시작
    public void PlaySneakSound()
    {
        CmdPlaySneakSound();
    }

    [Command]
    public void CmdPlaySneakSound()
    {
        RpcPlaySneakSound();
        if (!sneakSoundFlag)
        {
            walkSound.pitch = 0.35f;
            sneakSoundFlag = true;
        }
    }

    [ClientRpc]
    public void RpcPlaySneakSound()
    {
        if (!sneakSoundFlag)
        {
            walkSound.pitch = 0.35f;
            sneakSoundFlag = true;
        }
    }

    //앉아서 걷는 소리 정지
    public void StopSneakSound()
    {
        CmdStopSneakSound();
    }

    [Command]
    public void CmdStopSneakSound()
    {
        RpcStopSneakSound();
        if (sneakSoundFlag)
        {
            walkSound.pitch = 0.7f;
            sneakSoundFlag = false;
        }
    }

    [ClientRpc]
    public void RpcStopSneakSound()
    {
        if (sneakSoundFlag)
        {
            walkSound.pitch = 0.7f;
            sneakSoundFlag = false;
        }
    }

    //심장 소리 시작
    public void PlayHeartSound()
    {
        if (!heartSoundFlag)
        {
            heartSound.Play();
            heartSoundFlag = true;
        }
    }

    //심장 소리 정지
    public void StopHeartSound()
    {
        if (heartSoundFlag)
        {
            heartSound.Stop();
            heartSoundFlag = false;
        }
    }

    //점프 소리
    public void PlayJumpSound()
    {
        CmdPlayJumpSound();
    }

    [Command]
    public void CmdPlayJumpSound()
    {
        RpcPlayJumpSound();
        jumpSound.Play();
    }

    [ClientRpc]
    public void RpcPlayJumpSound()
    {
        jumpSound.Play();
    }

    //맞는 소리
    public void PlayBehitSound()
    {
        CmdPlayBehitSound();
    }

    [Command]
    public void CmdPlayBehitSound()
    {
        int index;
        index = Random.Range(0, behitSound.Length);

        RpcPlayBehitSound(index);

        behitSound[index].Play();
    }

    [ClientRpc]
    public void RpcPlayBehitSound(int index)
    {
        behitSound[index].Play();
    }

    //죽는 소리
    public void PlayDeathSound()
    {
        CmdPlayDeathSound();
    }

    [Command]
    public void CmdPlayDeathSound()
    {
        int index;
        index = Random.Range(0, deathSound.Length);

        RpcPlayDeathSound(index);

        deathSound[index].Play();
        StopAllSound();
    }

    [ClientRpc]
    public void RpcPlayDeathSound(int index)
    {
        deathSound[index].Play();
        StopAllSound();
    }

    //모든 소리 정지
    public void StopAllSound()
    {
        if (walkSoundFlag) walkSound.Stop();
        if (heartSoundFlag) heartSound.Stop();
    }

    //물걷는 소리 시작
    public void PlayWaterWalkSound()
    {
        CmdPlayWaterWalkSound();
    }

    [Command]
    public void CmdPlayWaterWalkSound()
    {
        RpcPlayWaterWalkSound();
        if (!waterWalkSoundFlag)
        {
            waterWalkSound.pitch = 0.8f;
            waterWalkSound.Play();
            waterWalkSoundFlag = true;
        }
    }

    [ClientRpc]
    public void RpcPlayWaterWalkSound()
    {
        if (!waterWalkSoundFlag)
        {
            waterWalkSound.pitch = 0.8f;
            waterWalkSound.Play();
            waterWalkSoundFlag = true;
        }
    }

    //물걷는 소리 정지
    public void StopWaterWalkSound()
    {
        CmdStopWaterWalkSound();
    }

    [Command]
    public void CmdStopWaterWalkSound()
    {
        RpcStopWaterWalkSound();
        if (waterWalkSoundFlag)
        {
            waterWalkSound.Stop();
            waterWalkSoundFlag = false;
            waterRunSoundFlag = false;
            waterSneakSoundFlag = false;
        }
    }

    [ClientRpc]
    public void RpcStopWaterWalkSound()
    {
        if (waterWalkSoundFlag)
        {
            waterWalkSound.Stop();
            waterWalkSoundFlag = false;
            waterRunSoundFlag = false;
            waterSneakSoundFlag = false;
        }
    }

    //물뛰는 소리 시작
    public void PlayWaterRunSound()
    {
        CmdPlayWaterRunSound();
    }

    [Command]
    public void CmdPlayWaterRunSound()
    {
        RpcPlayWaterRunSound();
        if (!waterRunSoundFlag)
        {
            waterWalkSound.pitch = 1.2f;
            waterRunSoundFlag = true;
        }
    }

    [ClientRpc]
    public void RpcPlayWaterRunSound()
    {
        if (!waterRunSoundFlag)
        {
            waterWalkSound.pitch = 1.2f;
            waterRunSoundFlag = true;
        }
    }

    //물뛰는 소리 정지
    public void StopWaterRunSound()
    {
        CmdStopWaterRunSound();
    }

    [Command]
    public void CmdStopWaterRunSound()
    {
        RpcStopWaterRunSound();
        if (waterRunSoundFlag)
        {
            waterWalkSound.pitch = 0.8f;
            waterRunSoundFlag = false;
        }
    }
    [ClientRpc]
    public void RpcStopWaterRunSound()
    {
        if (waterRunSoundFlag)
        {
            waterWalkSound.pitch = 0.8f;
            waterRunSoundFlag = false;
        }
    }

    //물앉는 소리 시작
    public void PlayWaterSneakSound()
    {
        CmdPlayWaterSneakSound();
    }

    [Command]
    public void CmdPlayWaterSneakSound()
    {
        RpcPlayWaterSneakSound();
        if (!waterSneakSoundFlag)
        {
            waterWalkSound.pitch = 0.5f;
            waterSneakSoundFlag = true;
        }
    }

    [ClientRpc]
    public void RpcPlayWaterSneakSound()
    {
        if (!waterSneakSoundFlag)
        {
            waterWalkSound.pitch = 0.5f;
            waterSneakSoundFlag = true;
        }
    }

    //물앉는 소리 정지
    public void StopWaterSneakSound()
    {
        CmdStopWaterSneakSound();
    }

    [Command]
    public void CmdStopWaterSneakSound()
    {
        RpcStopWaterSneakSound();
        if (waterSneakSoundFlag)
        {
            waterWalkSound.pitch = 0.8f;
            waterSneakSoundFlag = false;
        }
    }
    [ClientRpc]
    public void RpcStopWaterSneakSound()
    {
        if (waterSneakSoundFlag)
        {
            waterWalkSound.pitch = 0.8f;
            waterSneakSoundFlag = false;
        }
    }
}
