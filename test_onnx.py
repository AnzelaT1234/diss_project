import onnxruntime as ort
import numpy as np
import pickle

# Load the ONNX model
session = ort.InferenceSession("emissions.onnx")
expected = [[1.4554786019e-04,0.34],[10,256,30000,2.4466431946e-03,0.86],[20,256,60000,7.2470661900e-03,0.89]]
i = 0
for data in [[1,128, 60_000], [10, 256, 10_000], [20,256,60_000]]:
    print(data)
    print("Expected: ", expected[i])
    i+=1

    # Prepare a sample input
    test_input = np.array([data], dtype=np.float32)
    result = session.run(["output"], {"input": test_input})
    result = np.array(result)
    print("Predicted output (energy, accuracy):", result)

