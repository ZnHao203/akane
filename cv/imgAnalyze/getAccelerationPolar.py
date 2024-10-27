import json
import matplotlib.pyplot as plt
import numpy as np
from scipy.stats import norm

folder_path = "set21_active"

# Open and read the JSON file
with open(f"{folder_path}fishspeed.json", 'r') as file:
    data = json.load(file)

speed_list = data[folder_path]
# print(data_list)
data_len = len(speed_list)

acceleration_data = []
for i in range(1, len(speed_list)):

    # Extract the x and y components of velocity for the two points
    v1_x, v1_y = speed_list[i-1]["x"], speed_list[i-1]["y"]
    v2_x, v2_y = speed_list[i]["x"], speed_list[i]["y"]

    # Calculate the magnitudes of the vectors
    r1 = np.sqrt(v1_x**2 + v1_y**2)
    r2 = np.sqrt(v2_x**2 + v2_y**2)

    # Calculate the change in radius
    delta_r = r2 - r1

    # Calculate the angles in radians
    theta1 = np.arctan2(v1_y, v1_x)
    theta2 = np.arctan2(v2_y, v2_x)

    # Calculate the change in angle in radians and convert to degrees
    delta_theta = (theta2 - theta1) * (180 / np.pi)

    # Normalize delta_theta to be within -180 to 180 degrees
    if delta_theta > 180:
        delta_theta -= 360
    elif delta_theta < -180:
        delta_theta += 360

    # Output delta_r and delta_theta
    print(f"Change in radius (Δr): {delta_r}, Change in angle (Δθ): {delta_theta} degrees")
    acceleration_data.append({"delta_r": delta_r, "delta_theta": delta_theta})

# record data
data_to_record_acceleration = {folder_path:acceleration_data}

with open(f"{folder_path}fishaccelerationPolar.json", "w") as final:
	json.dump(data_to_record_acceleration, final, indent=4)

# plot
ar_values = [data["delta_r"] for data in acceleration_data  if np.isfinite(data["delta_r"])]
atheta_values = [data["delta_theta"] for data in acceleration_data  if np.isfinite(data["delta_theta"])]

mu_r, std_r = norm.fit(ar_values)
mu_theta, std_theta = norm.fit(atheta_values)

print("delta r ", mu_r, std_r)
print("delta theta ", mu_theta, std_theta)

# Plot a histogram of speed frequencies
plt.figure(figsize=(12, 6))
plt.subplot(1, 2, 1)
plt.hist(ar_values, bins=20, density=True, edgecolor='black', alpha=0.7)  # Adjust bins as needed

xmin_r, xmax_r = plt.xlim()  # Get x-axis limits for the curve
r = np.linspace(xmin_r, xmax_r, 100)
pdf_r = norm.pdf(r, mu_r, std_r)
plt.plot(r, pdf_r, 'k', linewidth=2)

# Add titles and labels
plt.title('R Acceleration Distribution of the Fish')
plt.xlabel('R Acceleration (pixel/s^2)')
plt.ylabel('Frequency of Occurrence')
plt.grid(True)

plt.subplot(1, 2, 2)
plt.hist(atheta_values, bins=20, density=True, edgecolor='black', alpha=0.7)  # Adjust bins as needed

xmin_t, xmax_t = plt.xlim()  # Get x-axis limits for the curve
t = np.linspace(xmin_t, xmax_t, 100)
pdf_t = norm.pdf(t, mu_theta, std_theta)
plt.plot(t, pdf_t, 'k', linewidth=2)

# Add titles and labels
plt.title('Theta Acceleration Distribution of the Fish')
plt.xlabel('Theta Acceleration (degree/s^2)')
plt.ylabel('Frequency of Occurrence')


# Show the plot
plt.grid(True)
plt.show()