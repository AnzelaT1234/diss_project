using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlterHyperParameters : MonoBehaviour
{

    [Header("Sliders")]
    [SerializeField] private Slider bufferSlider;
    [SerializeField] private Slider batchSlider;
    [SerializeField] private Slider epochSlider;

    [Header("Initial Variables")]
    [SerializeField] private int buffer;
    [SerializeField] private int batch;
    [SerializeField] private int epoch;

    // Note: Buffer must be a multiple of batch size
    // private int minBuffer = 2048;
    // private int maxBuffer = 409600;

    // Note: Batch in discrete action space
    private int minBatchSize = 32;
    // private int maxBatchSize = 512;

    // private int minEpochs = 3;
    // private int maxEpochs = 10;

    public void OnBufferChanged(int val)
    {
        buffer = batch * val;
    }

    public void OnBatchChanged(int val)
    {
        batch = val * minBatchSize;
    }

    public void OnEpochChanged(int val)
    {
        epoch = val;
    }

    
}
