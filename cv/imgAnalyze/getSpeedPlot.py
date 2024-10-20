import matplotlib.pyplot as plt
import numpy as np
import json

data_file_name = "set21_activefishspeed.json"
folder_path = "set21_active"

# Open and read the JSON file
with open(data_file_name, 'r') as file:
    data = json.load(file)

speed_list = data[folder_path]
# print(data_list)

# Extract velocity values for plotting
ax_values = [data["x"] for data in speed_list]
ay_values = [data["y"] for data in speed_list]

# Example x and y speed lists
x_speed = [-54.0, -10.0, 5.0, 20.0, 0.0, -30.0]  # Replace with your x-speed data
y_speed = [-56.0, -25.0, 15.0, 40.0, 0.0, -35.0]  # Replace with your y-speed data

# Calculate the speed (magnitude of velocity) for each point
speeds = [np.sqrt(x**2 + y**2) for x, y in zip(x_speed, y_speed)]

# Plot a histogram of speed frequencies
plt.figure(figsize=(10, 6))
plt.hist(speeds, bins=20, edgecolor='black', alpha=0.7)  # Adjust bins as needed

# Add titles and labels
plt.title('Speed Distribution of the Fish')
plt.xlabel('Speed (m/s)')
plt.ylabel('Frequency of Occurrence')

# Show the plot
plt.grid(True)
plt.show()