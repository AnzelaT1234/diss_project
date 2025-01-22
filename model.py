import torch
import torch.onnx
import torch.nn as nn
import torch.optim as optim
import numpy as np
from sklearn.preprocessing import StandardScaler
import json

# Create a simple linear regression model in PyTorch
class LinearRegressor(nn.Module):
    def __init__(self):
        super(LinearRegressor, self).__init__()
        self.linear1 = nn.Linear(3, 16)
        self.relu = nn.ReLU()
        self.leakyrelu = nn.LeakyReLU(negative_slope=0.01)
        self.linear2 = nn.Linear(16,8)
        self.linear3 = nn.Linear(8,1)

    def forward(self, x):
        x = self.linear1(x)
        x = self.leakyrelu(x)
        x = self.linear2(x)
        x = self.leakyrelu(x)
        x = self.linear3(x)
        return x

def data_prep(filename):
    data = np.loadtxt(filename, delimiter=',')
    np.random.shuffle(data)

    X = data[:,:3]
    # X = (X-np.max(X))/(np.max(X)-np.min(X))
    min_X = np.min(X, axis=0)
    max_X = np.max(X, axis=0)
    X = (X-min_X)/(max_X-min_X)

    y = data[:, 3]
    y = y*1_000
    y = (y-np.min(y))/(np.max(y)-np.min(y))
    # y = np.log(y)

    test = int(0.2*len(X))
    X_train = X # [test:,:]
    y_train = y #[test:]

    X_test = X[:test,:]
    y_test = y[:test]

    X_train = torch.tensor(X_train, dtype=torch.float32)
    y_train = torch.tensor(y_train, dtype=torch.float32)
    X_test = torch.tensor(X_test, dtype=torch.float32)
    y_test = torch.tensor(y_test, dtype=torch.float32)

    # X_train = torch.log(torch.tensor(X_train).float())
    # y_train = torch.log(torch.tensor(y_train).float()) 
    return X_train, y_train, X_test, y_test

def train(model, X_train, y_train):
    model.train()
    criterion = nn.MSELoss()
    optimizer = optim.Adam(model.parameters(), lr=0.00001)
    epochs = 10000
    for epoch in range(epochs):
        predictions = model(X_train)
        loss = criterion(predictions, y_train)

        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
        if (epoch + 1) % 100 == 0:
            print(f"Epoch [{epoch+1}/{epochs}], Loss: {loss.item():.4f}")

def __main__():
    model = LinearRegressor()
    X_train, y_train, X_test, y_test= data_prep("data.csv")
    
    train(model, X_train, y_train)

    # Export to ONNX
    model.eval()
    dummy_input = torch.randn(1, 3)  # Batch size = 1, input features = 3
    torch.onnx.export(
        model,
        dummy_input,
        "emissions.onnx",
        input_names=["input"],
        output_names=["output"],
        opset_version=11
    )

    print("Model exported to linear_regressor.onnx")


if __name__=="__main__":
    __main__()