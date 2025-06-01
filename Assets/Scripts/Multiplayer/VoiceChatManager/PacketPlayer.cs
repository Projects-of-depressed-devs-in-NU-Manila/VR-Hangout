using System.Collections.Concurrent;
using UnityEngine;
using System;
using UnityOpus;

[RequireComponent(typeof(PacketDecoder))]
public class PacketPlayer : MonoBehaviour {
    const NumChannels channels = NumChannels.Mono;
    const SamplingFrequency frequency = SamplingFrequency.Frequency_48000;
    const int audioClipLength = 1024 * 6;
    AudioSource source;
    int head = 0;
    float[] audioClipData;

    PacketDecoder decoder;

    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        decoder = gameObject.GetComponent<PacketDecoder>();
        decoder.OnDecoded += OnDecoded;
    }

    void OnEnable() {
        source = GetComponent<AudioSource>();
        source.clip = AudioClip.Create("Loopback", audioClipLength, (int)channels, (int)frequency, false);
        source.loop = true;
    }

    void OnDisable() {
        source.Stop();
    }

    void OnDecoded(float[] pcm, int pcmLength) {
        if (audioClipData == null || audioClipData.Length != pcmLength) {
            // assume that pcmLength will not change.
            audioClipData = new float[pcmLength];
        }
        Array.Copy(pcm, audioClipData, pcmLength);
        source.clip.SetData(audioClipData, head);
        head += pcmLength;
        if (!source.isPlaying && head > audioClipLength / 2) {
            source.Play();
        }
        head %= audioClipLength;
    }
}