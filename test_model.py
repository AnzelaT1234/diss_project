import torch
from sklearn.linear_model import LinearRegression
import numpy as np
import pickle

with open('input_scaler.pkl', 'rb') as f:
    input_scaler = pickle.load(f)
with open('output_scaler.pkl', 'rb') as f:
    output_scaler = pickle.load(f)

# Load the model
with open('linear_regression_model.pkl', 'rb') as f:
    model = pickle.load(f)

# Test with dummy input
data = [20.0, 256.0, 30000.0]
data_normalized = input_scaler.transform(np.array([data]))
output = model.predict(data_normalized)
output_denormalized = output_scaler.inverse_transform(output.reshape(1, -1))

print("Denormalized Output: ", output)
print("Denormalized Output: ", output_denormalized)