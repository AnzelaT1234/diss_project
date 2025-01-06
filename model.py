import torch

class ConsumptionModel(torch.nn.Module):

    def __init__(self):
        super(ConsumptionModel, self).__init__()

        self.linear1 = torch.nn.Linear(100, 200)
        self.activation = torch.nn.ReLU()
        self.linear2 = torch.nn.Linear(200, 10)
        self.softmax = torch.nn.Softmax()

    def forward(self, x):
        x = self.linear1(x)
        x = self.activation(x)
        x = self.linear2(x)
        x = self.softmax(x)
        return x

model = ConsumptionModel()

print('The model:')
print(model)

print('\n\nJust one layer:')
print(model.linear2)

print('\n\nModel params:')
for param in model.parameters():
    print(param)

print('\n\nLayer params:')
for param in model.linear2.parameters():
    print(param)