import torch
import numpy as np
from sklearn.preprocessing import StandardScaler
from sklearn.linear_model import LinearRegression
import pickle

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

def predict(model, X_test, output_scaler):
    # Make predictions with the trained model
    y_pred = model.predict(X_test)

    # Denormalize the predictions
    y_pred_denormalized = output_scaler.inverse_transform(y_pred)

    return y_pred_denormalized

def __main__():
    model = LinearRegression()
    X_train, y_train, X_test, y_test, input_scaler, output_scaler = data_prep("data.csv")
    model.fit(X_train, y_train)

    # Make predictions and denormalize them
    y_pred_denormalized = predict(model, X_test, output_scaler)

    # Save the trained model and scalers
    with open('linear_regression_model.pkl', 'wb') as f:
        pickle.dump(model, f)

    with open('input_scaler.pkl', 'wb') as f:
        pickle.dump(input_scaler, f)
    with open('output_scaler.pkl', 'wb') as f:
        pickle.dump(output_scaler, f)

    print("Predicted values (denormalized):", y_pred_denormalized)


if __name__=="__main__":
    __main__()