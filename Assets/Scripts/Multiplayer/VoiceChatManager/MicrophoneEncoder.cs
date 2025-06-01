using System.Collections.Generic;
using UnityEngine;
using System;
using UnityOpus;

[RequireComponent(typeof(MicrophoneRecorder))]
public class MicrophoneEncoder : MonoBehaviour {
    public MicrophoneEncoder Instance;

    public event Action<byte[], int> OnEncoded;

    const int bitrate = 96000;
    const int frameSize = 120;
    const int outputBufferSize = frameSize * 4; // at least frameSize * sizeof(float)

    MicrophoneRecorder recorder;
    Encoder encoder;
    Queue<float> pcmQueue = new Queue<float>();
    readonly float[] frameBuffer = new float[frameSize];
    readonly byte[] outputBuffer = new byte[outputBufferSize];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() {
        recorder = GetComponent<MicrophoneRecorder>();
        recorder.OnAudioReady += OnAudioReady;
        encoder = new Encoder(
            SamplingFrequency.Frequency_48000,
            NumChannels.Mono,
            OpusApplication.Audio) {
            Bitrate = bitrate,
            Complexity = 10,
            Signal = OpusSignal.Music
        };
    }

    void OnDisable() {
        recorder.OnAudioReady -= OnAudioReady;
        encoder.Dispose();
        encoder = null;
        pcmQueue.Clear();
    }

    void OnAudioReady(float[] data) {
        foreach (var sample in data) {
            pcmQueue.Enqueue(sample);
        }
        while (pcmQueue.Count > frameSize) {
            for (int i = 0; i < frameSize; i++) {
                frameBuffer[i] = pcmQueue.Dequeue();
            }
            var encodedLength = encoder.Encode(frameBuffer, outputBuffer);
            VoiceNetworkManager.Instance.Broadcast(outputBuffer, encodedLength);
        }
    }
}