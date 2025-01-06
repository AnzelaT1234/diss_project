import itertools
import torch 
import torch.nn as nn
import torch.nn.functional as F
import torchvision
import torchvision.transforms as transforms
import torch.optim as optim
from codecarbon import EmissionsTracker
import numpy as np

""" 
    Create small pytorch model 
"""

class Net(nn.Module):
    def __init__(self):
        super(Net, self).__init__()
        self.conv1 = nn.Conv2d(1, 6, 5)
        self.pool = nn.MaxPool2d(2, 2)
        self.conv2 = nn.Conv2d(6, 16, 5)
        self.fc1 = nn.Linear(16 * 4 * 4, 120)
        self.fc2 = nn.Linear(120, 84)
        self.fc3 = nn.Linear(84, 10)

    def forward(self, x):
        x = self.pool(F.relu(self.conv1(x)))
        x = self.pool(F.relu(self.conv2(x)))
        x = x.view(-1, 16 * 4 * 4)
        x = F.relu(self.fc1(x))
        x = F.relu(self.fc2(x))
        x = self.fc3(x)
        return x

"""
    Train Model
"""
def data_prep(batch_size, num_samples):
    test_batch_size = 128

    transform = transforms.Compose(
        [transforms.ToTensor(),
         transforms.Normalize((0.5,), (0.5,))]
    )

    trainset = torchvision.datasets.FashionMNIST(root='./data', train=True, download=True, transform = transform)
        # Generate a list of indices from 0 to len(train_dataset)
    
    if num_samples==60_000:
        train_subset = trainset
    else:
        indices = np.random.choice(len(trainset), num_samples-1, replace=False)
        # Create a subset of the dataset using the indices
        train_subset = torch.utils.data.Subset(trainset, indices)

    trainloader = torch.utils.data.DataLoader(train_subset, batch_size = batch_size,
                                              shuffle = True, num_workers = 2)
    


    testset = torchvision.datasets.FashionMNIST(root='./data', train=False,
                                           download = True, transform = transform)
    testloader = torch.utils.data.DataLoader(testset, batch_size=test_batch_size,
                                             shuffle = False, num_workers = 2)
    
    return trainloader, testloader

def train(model, trainloader, epochs):
    criterion = nn.CrossEntropyLoss()
    optimizer = optim.SGD(model.parameters(), lr=0.01, momentum=0.9)    
    for epoch in range(epochs):
        running_loss = 0.0
        for i, data in enumerate(trainloader, 0):
            # get the inputs; data is a list of [inputs, labels]
            inputs, labels = data

            # zero the parameter gradients
            optimizer.zero_grad()

            # forward + backward + optimize
            outputs = model(inputs)
            loss = criterion(outputs, labels)
            loss.backward()
            optimizer.step()

            # print statistics
            running_loss += loss.item()
            if i % 2000 == 1999:    # print every 2000 mini-batches
                print(f'[{epoch + 1}, {i + 1:5d}] loss: {running_loss / 2000:.3f}')
                running_loss = 0.0

    print('Finished Training')

def eval(model, testloader):
    correct = 0
    total = 0
    # since we're not training, we don't need to calculate the gradients for our outputs
    with torch.no_grad():
        for data in testloader:
            images, labels = data
            # calculate outputs by running images through the network
            outputs = model(images)
            # the class with the highest energy is what we choose as prediction
            _, predicted = torch.max(outputs, 1)
            total += labels.size(0)
            correct += (predicted == labels).sum().item()

    print(f'Accuracy of the network on the 10000 test images: {100 * correct // total} %')
    return correct / total

def saveToFile(filename, combinations, emissions, accuracy):
    data = []
    for (epoch, batch, sample), e, a in zip(combinations, emissions, accuracy):
        data.append([epoch, batch, sample, e, a])   
    data_array = np.array(data) 
    np.savetxt('data.csv', data_array, delimiter=',', fmt='%d,%d,%d,%.10e,%.2f')

def __main__():
    """ 
        Create Combinations 
    """

    # Initialise variables
    epochs = [1,10,20]
    batch_size = [64,128,256]
    sample_size = [10_000,30_000,60_000]

    combinations = list(itertools.product(*[epochs, batch_size, sample_size]))

    # print(combinations)
    tracker = EmissionsTracker()
    emissions = []
    accuracy = []

    """
        Train the model keeping note of emissions each time
    """
    for params in combinations:
        model = Net()
        epoch, batch, sample = params
        print(f"Epoch {epoch}, Batch: {batch}, Sample: {sample}")
        tracker.start()
        train_loader, test_loader= data_prep(batch, sample)
        train(model, train_loader, epoch)
        e = tracker.stop()
        emissions.append(e)

        a = eval(model, test_loader)
        accuracy.append(a)
        print(f"\nFinshed with:\nEpochs: {epoch}\tBatch: {batch}\tSample: {sample}\nEmissions: {e}\tAccuracy: {a}\n")

    saveToFile("data_fmnist.csv", combinations, emissions, accuracy)

if __name__ == "__main__":
    __main__()