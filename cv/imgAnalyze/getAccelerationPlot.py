import matplotlib.pyplot as plt
import numpy as np
from scipy.stats import norm
import json

data_file_name = "set21_activefishacceleration.json"
folder_path = "set21_active"

# Open and read the JSON file
with open(data_file_name, 'r') as file:
    data = json.load(file)

acc_list = data[folder_path]
# print(data_list)

# Extract velocity values for plotting
ax_values = [x for x in acc_list["x"] if np.isfinite(x)]
ay_values = [y for y in acc_list["y"] if np.isfinite(y)]


mu_x, std_x = norm.fit(ax_values)
mu_y, std_y = norm.fit(ay_values)

print(mu_x, std_x)
print(mu_y, std_y)

mu1 = np.mean(ax_values)
std1 = np.std(ax_values)
print(mu1)
print(std1)
# # Plot a histogram of speed frequencies
# plt.figure(figsize=(12, 6))
# plt.subplot(1, 2, 1)
# plt.hist(ax_values, bins=20, density=True, edgecolor='black', alpha=0.7)  # Adjust bins as needed

# xmin_x, xmax_x = plt.xlim()  # Get x-axis limits for the curve
# x = np.linspace(xmin_x, xmax_x, 100)
# pdf_x = norm.pdf(x, mu_x, std_x)
# plt.plot(x, pdf_x, 'k', linewidth=2)

# # Add titles and labels
# plt.title('X Acceleration Distribution of the Fish')
# plt.xlabel('X Acceleration (pixel/s^2)')
# plt.ylabel('Frequency of Occurrence')
# plt.grid(True)

# plt.subplot(1, 2, 2)
# plt.hist(ay_values, bins=20, density=True, edgecolor='black', alpha=0.7)  # Adjust bins as needed

# xmin_y, xmax_y = plt.xlim()  # Get x-axis limits for the curve
# y = np.linspace(xmin_y, xmax_y, 100)
# pdf_y = norm.pdf(y, mu_y, std_y)
# plt.plot(y, pdf_y, 'k', linewidth=2)

# # Add titles and labels
# plt.title('Y Acceleration Distribution of the Fish')
# plt.xlabel('Y Acceleration (pixel/s^2)')
# plt.ylabel('Frequency of Occurrence')


# # Show the plot
# plt.grid(True)
# plt.show()