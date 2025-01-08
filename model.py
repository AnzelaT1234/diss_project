import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np
import tqdm
import copy

class ConsumptionModel(torch.nn.Module):

    def __init__(self):
        super(ConsumptionModel, self).__init__()

        self.linear1 = torch.nn.Linear(3, 10)
        self.activation = torch.nn.ReLU()
        self.linear2 = torch.nn.Linear(10, 2)
        self.softmax = torch.nn.Softmax()

    def forward(self, x):
        x = self.linear1(x)
        x = self.activation(x)
        x = self.linear2(x)
        x = self.softmax(x)
        return x

def data_prep(filename):
    data = np.loadtxt(filename, delimiter=',')
    np.random.shuffle(data)
    X = data[:,:3]
    y = data[:, 3:]
    test = int(0.2*len(X))
    X_train = X[test:,:]
    y_train = y[test:, :]

    X_test = X[:test,:]
    y_test = y[:test, :]

    X_train = torch.tensor(X_train, dtype=torch.float32)
    y_train = torch.tensor(y_train, dtype=torch.float32)
    X_test = torch.tensor(X_test, dtype=torch.float32)
    y_test = torch.tensor(y_test, dtype=torch.float32)
    # print(f"X: {X_train}\ny:{y_train}")
    # print("TEST:")
    # print(f"X: {X_test}\ny:{y_test}")
    return X_train, y_train, X_test, y_test

def train(model, X_train, y_train, X_test, y_test, batch_size, n_epochs):
    # loss function and optimizer
    loss_fn = nn.MSELoss()  # mean square error
    optimizer = optim.Adam(model.parameters(), lr=0.0001)
    batch_start = torch.arange(0, len(X_train), batch_size)
 
    # Hold the best model
    best_mse = np.inf   # init to infinity
    best_weights = None
    history = []
    
    # training loop
    for epoch in range(n_epochs):
        print(f"Epoch {epoch}")
        model.train()
        epoch_loss = 0
        with tqdm.tqdm(batch_start, unit="batch", mininterval=0, disable=True) as bar:
            bar.set_description(f"Epoch {epoch}")
            for start in bar:
                # take a batch
                X_batch = X_train[start:start+batch_size]
                y_batch = y_train[start:start+batch_size]
                # forward pass
                y_pred = model(X_batch)
                loss = loss_fn(y_pred, y_batch)
                epoch_loss += loss.item() * len(X_batch)  # accumulate total loss
                # backward pass
                print(f"LOSS: {loss.item()}")
                optimizer.zero_grad()
                loss.backward()
                # update weights
                optimizer.step()
                # print progress
                bar.set_postfix(mse=float(loss))
        # evaluate accuracy at end of each epoch
        model.eval()
        # Calculate average training loss
        avg_epoch_loss = epoch_loss / len(X_train)
        y_pred = model(X_test)
        mse = loss_fn(y_pred, y_test)
        mse = float(mse)
        history.append(mse)
        if mse < best_mse:
            best_mse = mse
            best_weights = copy.deepcopy(model.state_dict())
        # Log training loss and test MSE
        history.append(mse)
        print(f"Training Loss: {avg_epoch_loss:.4f} | Test MSE: {mse:.4f}")
 
    # restore model and return best accuracy
    model.load_state_dict(best_weights)
    print("FINISHED TRAINING")

def __main__():
    model = ConsumptionModel()
    X_train, y_train, X_test, y_test = data_prep("data.csv")
    batch_size = 4
    epochs = 10

    train(model, X_train, y_train, X_test, y_test, batch_size, epochs)
    torch.save(model.state_dict(), "consumption_model.pth")
if __name__=="__main__":
    __main__()