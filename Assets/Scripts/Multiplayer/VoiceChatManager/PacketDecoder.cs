using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityOpus;

[RequireComponent(typeof(PacketPlayer))]
public class PacketDecoder : MonoBehaviour
{
    public event Action<float[], int> OnDecoded;
    public Queue<VoicePacket> voicePackets = new Queue<VoicePacket>();
    const NumChannels channels = NumChannels.Mono;
    private float timer = 0;
    private float bufferTime = 2f;
    private int packetLostQTY = 1;


    Decoder decoder;
    readonly float[] pcmBuffer = new float[UnityOpus.Decoder.maximumPacketDuration * (int)channels];

    void OnEnable()
    {
        decoder = new Decoder(
            SamplingFrequency.Frequency_48000,
            NumChannels.Mono);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= bufferTime) {
            timer = 0f;
            if (voicePackets.Count == 0) {
                Decode(null, 0);
                packetLostQTY += 1; 
                return;
            }
            while (voicePackets.Count != 0) {
                VoicePacket packet = voicePackets.Dequeue();
                Decode(packet.data, packet.length);
            }
        }
    }

    void OnDisable()
    {
        decoder.Dispose();
        decoder = null;
    }

    public void QueuePacket(VoicePacket packet)
    {
        if (packetLostQTY > 0)
        {
            packetLostQTY -= 1;
            return;
        }
        voicePackets.Enqueue(packet);
    }

    public void Decode(byte[] data, int length)
    {
        var pcmLength = decoder.Decode(data, length, pcmBuffer);
        OnDecoded?.Invoke(pcmBuffer, pcmLength);
    }
}
