import matplotlib.pyplot as plt
import numpy as np
import sys

file = sys.argv[1]

data = np.genfromtxt(file, delimiter=',', skip_header=1)

# print(data)

x = data[:,1]
y = data[:,2]
plt.ylim(-15,10)

plt.plot(x,y)
plt.show()

