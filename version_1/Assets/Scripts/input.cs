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

    [Header("Sliders")]
    [SerializeField] private Slider epochSlider;
    // TODO: ADD BATCH AND SAMPLE SLIDERS

    [Header("Output Texts")]
    [SerializeField] private TextMeshProUGUI energyConsumption = null;
    [SerializeField] private TextMeshProUGUI accuracy = null;

    [Header("Model")]
    [SerializeField] private NNModel modelAsset;
    private Model model;

    private IWorker worker;
    // private Scaler scaler;
    private float mean;
    private float std;
    // input variables
    private float epochs = 1;

    // output variables
    // private float energyVal = 0;
    // private float accuracyVal = 0;

    // // Start is called before the first frame update
    void Start()
    {
        model = ModelLoader.Load(modelAsset);
        worker = modelAsset.CreateWorker();
        // readWeights();
        // scaler = new Scaler();
        mean = 11164.333333333334F;
        std = 19659.05352079121F;

    }

    // private void readWeights()
    // {
    //     scaler = JsonUtility.FromJson<Scaler>("@./scaler.json");
    // }

    public void OnSliderChanged(float val)
    {
        epochs = val*maxEpochAmount;
        // energyConsumption.text = changedVal.ToString("0");
        // float accTest = changedVal * 100;
        // accuracy.text = accTest.ToString("0");
        // switch(name)
        // {
        //     case "epoch":
        //         epochs = value;
        //         break;
        // }

        // CalculateOutput(changedVal);
    }

    public void CalculateOutput()
    {
        // epoch = (epoch - mean) / std;

        Tensor inputs = new Tensor(1, 1, 1, 3, new float[] { epochs, 128.0F, 60000.0F });
        worker.Execute(inputs);
        Tensor outputTensor = worker.PeekOutput();
        float consump = outputTensor[0, 0, 0, 0]; // First output
        float acc = outputTensor[0, 0, 0, 1]; // Second output

        energyConsumption.text = consump.ToString("0.000");
        accuracy.text = acc.ToString("0.000");
        // Dispose tensors to free memory
        inputs.Dispose();
        outputTensor.Dispose();
    }

    void OnDestroy()
    {
        // Dispose the worker when the script is destroyed
        worker.Dispose();
    }
}
