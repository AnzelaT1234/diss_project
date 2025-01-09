import torch
import torch.onnx
import torch.nn as nn
import torch.optim as optim
import numpy as np
from sklearn.preprocessing import StandardScaler
import pickle

# Create a simple linear regression model in PyTorch
class LinearRegressor(nn.Module):
    def __init__(self):
        super(LinearRegressor, self).__init__()
        self.fc = nn.Linear(3, 2)  # 3 inputs, 2 outputs

    def forward(self, x):
        return self.fc(x)

def data_prep(filename):
    data = np.loadtxt(filename, delimiter=',')
    np.random.shuffle(data)

    x_scaler = StandardScaler()
    X = data[:,:3]
    X_normalized = x_scaler.fit_transform(X)

    y_scaler = StandardScaler()
    y = data[:, 3:]
    y_normalized = y_scaler.fit_transform(y)

    test = int(0.2*len(X))
    X_train = X_normalized[test:,:]
    y_train = y_normalized[test:, :]

    X_test = X_normalized[:test,:]
    y_test = y_normalized[:test, :]

    X_train = torch.tensor(X_train, dtype=torch.float32)
    y_train = torch.tensor(y_train, dtype=torch.float32)
    X_test = torch.tensor(X_test, dtype=torch.float32)
    y_test = torch.tensor(y_test, dtype=torch.float32)
    return X_train, y_train, X_test, y_test, x_scaler, y_scaler

def train(model, X_train, y_train):
    criterion = nn.MSELoss()
    optimizer = optim.Adam(model.parameters(), lr=0.01)

    for epoch in range(1000):
        predictions = model(X_train)
        loss = criterion(predictions, y_train)

        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
        if (epoch + 1) % 100 == 0:
            print(f"Epoch [{epoch+1}/{1000}], Loss: {loss.item():.4f}")

def __main__():
    model = LinearRegressor()
    X_train, y_train, X_test, y_test, input_scaler, output_scaler = data_prep("data.csv")
    
    train(model, X_train, y_train)

    with open('input_scaler.pkl', 'wb') as f:
        pickle.dump(input_scaler, f)
    with open('output_scaler.pkl', 'wb') as f:
        pickle.dump(output_scaler, f)

    # Export to ONNX
    dummy_input = torch.randn(1, 3)  # Batch size = 1, input features = 3
    torch.onnx.export(
        model,
        dummy_input,
        "linear_regressor.onnx",
        input_names=["input"],
        output_names=["output"],
        opset_version=11
    )

    print("Model exported to linear_regressor.onnx")


if __name__=="__main__":
    __main__()