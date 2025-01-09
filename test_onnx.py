import onnxruntime as ort
import numpy as np
import pickle

# Load the ONNX model
session = ort.InferenceSession("linear_regressor.onnx")

with open('input_scaler.pkl', 'rb') as f:
    input_scaler = pickle.load(f)
with open('output_scaler.pkl', 'rb') as f:
    output_scaler = pickle.load(f)

data = [20,256,60_000]
data_normalized = input_scaler.transform(np.array([data]))

# Prepare a sample input
test_input = np.array(data_normalized, dtype=np.float32)

# Run inference
input_name = session.get_inputs()[0].name
output_name = session.get_outputs()[0].name
result = session.run([output_name], {input_name: test_input})
denormalized_result = output_scaler.inverse_transform(np.array(result).reshape(1,-1))
print("Predicted output (energy, accuracy):", denormalized_result)

