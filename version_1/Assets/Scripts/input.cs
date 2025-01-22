using System.IO;
// using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using TMPro;


public class Scaler
{
    public float mean {get ; set ;}
    public float std {get ; set ;}
}

public class input : MonoBehaviour
{

    private int maxEpochAmount = 20;
    private int maxBatchAmount = 256;
    private int maxSampleAmount = 60000;

    [Header("Sliders")]
    [SerializeField] private Slider epochSlider;
    [SerializeField] private Slider batchSlider;
    [SerializeField] private Slider sampleSlider;

    [Header("Output Texts")]
    [SerializeField] private TextMeshProUGUI energyConsumption = null;
    [SerializeField] private TextMeshProUGUI accuracy = null;

    [Header("Model")]
    [SerializeField] private NNModel modelAsset;
    [SerializeField] private NNModel emissionsModelAsset;
    private Model model;

    private IWorker worker;
    private IWorker emissionWorker;

    // input variables
    private float epochs = 1;
    private float batch = 64;
    private float sample = 10000;

    // output variables
    // private float energyVal = 0;
    // private float accuracyVal = 0;

    // // Start is called before the first frame update
    void Start()
    {
        model = ModelLoader.Load(modelAsset);
        worker = modelAsset.CreateWorker();
        emissionWorker = emissionsModelAsset.CreateWorker();

    }

    public void OnBatchChanged(float val)
    {
        batch = maxBatchAmount * val;
    }

    public void OnEpochChanged(float val)
    {
        epochs = val*maxEpochAmount;
    }

    public void OnSampleChanged(float val)
    {
        sample = val*maxSampleAmount;
    }

    public void CalculateOutput()
    {
        // epoch = (epoch - mean) / std;

        Tensor inputs = new Tensor(1, 1, 1, 3, new float[] { epochs, batch, sample});
        worker.Execute(inputs);
        Tensor outputTensor = worker.PeekOutput();
        // float consump = outputTensor[0, 0, 0, 0]; // First output
        float acc = outputTensor[0, 0, 0, 1]; // Second output

        float e_epoch = (epochs-1)/(20-1);
        float e_batch = (batch-64)/(256-64);
        float e_sample = (sample-10000)/(60000-10000);
        Tensor e_inputs = new Tensor(1, 1, 1, 3, new float[] { e_epoch, e_batch, e_sample});
        emissionWorker.Execute(inputs);
        Tensor eOutput = emissionWorker.PeekOutput();
        float consump = eOutput[0,0,0,0];

        float max_consump = 7.2470661900F;
        float min_consump = 0.000025752939683F;
        consump = (consump * (max_consump-min_consump)) + min_consump;

        acc =( 1 / (1+(Mathf.Exp(-1 * acc))))*100;
        if (acc > 70)
        {
            accuracy.color = Color.green;
        }
        else if (acc >50)
        {
            accuracy.color = Color.yellow;
        }
        else
        {
            accuracy.color = Color.red;
        }

        // consump = consump * 1000;
        // consump = consump * (sample/10000);
        energyConsumption.text = consump.ToString("0.00");
        accuracy.text = acc.ToString("0.00");
        // Dispose tensors to free memory
        inputs.Dispose();
        outputTensor.Dispose();
        eOutput.Dispose();
    }

    void OnDestroy()
    {
        // Dispose the worker when the script is destroyed
        worker.Dispose();
        emissionWorker.Dispose();
    }
}
