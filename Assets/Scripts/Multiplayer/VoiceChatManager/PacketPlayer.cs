using System.Collections.Concurrent;
using UnityEngine;
using System;
using UnityOpus;

[RequireComponent(typeof(PacketDecoder))]
[RequireComponent(typeof(AudioSource))]
public class PacketPlayer : MonoBehaviour
{
    const int SampleRate = 48000 / 2;
    const int NumChannels = 1;                  // Mono
    const int BufferSeconds = 2;                // 2-second ring buffer
    static readonly int BufferSize = SampleRate * BufferSeconds;

    private float[] ringBuffer = new float[BufferSize];
    private int writePos = 0;
    private int readPos = 0;
    private object lockObj = new object();

    private PacketDecoder decoder;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        decoder = GetComponent<PacketDecoder>();
        decoder.OnDecoded += OnDecoded;
    }

    void OnEnable()
    {
        // Ensure AudioSource exists and starts playing so that OnAudioFilterRead() is called.
        source = GetComponent<AudioSource>();
        source.clip = null;    // No clip—audio will come from OnAudioFilterRead
        source.loop = true;
        source.playOnAwake = false;
        // source.pitch = 0.5f;
        source.Play();
    }

    void OnDisable()
    {
        source.Stop();
    }

    // This is called whenever PacketDecoder has a decoded PCM frame
    void OnDecoded(float[] pcm, int pcmLength)
    {
        lock (lockObj)
        {
            for (int i = 0; i < pcmLength; i++)
            {
                ringBuffer[writePos] = pcm[i];
                writePos = (writePos + 1) % BufferSize;

                // If writePos catches up to readPos, advance readPos to avoid overwrite
                if (writePos == readPos)
                {
                    readPos = (readPos + 1) % BufferSize;
                }
            }
        }
    }

    // Unity will call this when the AudioSource needs more data
    // 'data' is the buffer we must fill; its length = frames * NumChannels
    void OnAudioFilterRead(float[] data, int channels)
    {
        lock (lockObj)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = ringBuffer[readPos];
                readPos = (readPos + 1) % BufferSize;
            }
        }
    }
}

