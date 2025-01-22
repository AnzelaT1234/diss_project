import onnxruntime as ort
import numpy as np
import pickle

# Load the ONNX model
session = ort.InferenceSession("emissions.onnx")
raw_data = np.loadtxt("data.csv", delimiter=',')
y = raw_data[:, 3]
y = y*1000

X = raw_data[:,:3]
X_min = np.min(X, axis=0)
X_max = np.max(X, axis=0)

expected = [[1.4554786019e-04,0.34],[2.4466431946e-03,0.86],[7.2470661900e-03,0.89]]
i = 0
for data in [[1,128, 60_000], [10, 256, 10_000], [20,256,60_000]]:
    print(data)
    print("Expected: ", expected[i])
    i+=1

    data = (data-X_min)/(X_max-X_min)

    # Prepare a sample input
    test_input = np.array([data], dtype=np.float32)
    result = session.run(["output"], {"input": test_input})
    result = np.array(result)
    print("Predicted output (energy, accuracy):", result)

    result_n = (result*(np.max(y)-np.min(y)))+np.min(y)
    result_n = result_n/1000
    print(f"PREDICTED OUTPUT DENORMALISED: {result_n}")

