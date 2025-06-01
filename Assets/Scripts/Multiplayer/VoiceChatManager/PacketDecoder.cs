using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityOpus;

[RequireComponent(typeof(PacketPlayer))]
public class PacketDecoder : MonoBehaviour
{
    public event Action<float[], int> OnDecoded;

    const NumChannels channels = NumChannels.Mono;

    Decoder decoder;
    readonly float[] pcmBuffer = new float[UnityOpus.Decoder.maximumPacketDuration * (int)channels];

    void OnEnable()
    {
        decoder = new Decoder(
            SamplingFrequency.Frequency_48000,
            NumChannels.Mono);
    }

    void OnDisable()
    {
        decoder.Dispose();
        decoder = null;
    }

    public void Decode(byte[] data, int length)
    {
        var pcmLength = decoder.Decode(data, length, pcmBuffer);
        OnDecoded?.Invoke(pcmBuffer, pcmLength);
    }
}
