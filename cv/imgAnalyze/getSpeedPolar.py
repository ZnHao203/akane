import json
import matplotlib.pyplot as plt
from scipy.stats import norm, gamma, rayleigh, fit, beta, boxcox, probplot, shapiro, skewnorm
import numpy as np


data_file_name = "set21_activefishdata.json"
folder_path = "set21_active"

# Open and read the JSON file
with open(data_file_name, 'r') as file:
    data = json.load(file)

data_list = data[folder_path]
# print(data_list)
data_len = len(data_list)

# algorithm 2
# ignore the bound, just fish
# ignore the rectangle, just center point
# we extract speed per second (1 frame/sec) by
# delta_x = (x2max-x1max) - (x2min-x1min) ; positive to right (need check)
# delta_y = (y2max-y1max) - (y2min-y1min) ; positive to below (need check)

current = 1 # skip first img, to subtract
speed_list = []

while (current < data_len):
    #get data
    fish_data_2 = data_list[current]["fish"]
    fish_data_1 = data_list[current-1]["fish"]
    #calculate displacement
    delta_x = (fish_data_2["xmax"] - fish_data_1["xmax"] + fish_data_2["xmin"] - fish_data_1["xmin"])/2
    delta_y = (fish_data_2["ymax"] - fish_data_1["ymax"] + fish_data_2["ymin"] - fish_data_1["ymin"])/2
    # print debug

    # print(f"delta x {delta_x}, delta y {delta_y}")

    # Extract the x and y components of velocity for the two points
    v1_x, v1_y = delta_x, delta_y

    # Calculate the magnitudes of the vectors
    r = np.sqrt(v1_x**2 + v1_y**2)

    # Calculate the angles in radians
    theta = np.arctan2(v1_y, v1_x)

    # Calculate the change in angle in radians and convert to degrees
    theta = theta* (180 / np.pi)

    # Normalize delta_theta to be within -180 to 180 degrees
    if theta > 180:
        theta -= 360
    elif theta < -180:
        theta += 360

    speed_list.append({"r":r, "theta":theta})
    current += 1


# Extract velocity values for plotting
ar_values = [data["r"] for data in speed_list  if np.isfinite(data["r"])]

# logr = np.log(ar_values)
# alogr_values = [ar_values for data in logr  if np.isfinite(data)]

alogr_values = []
for i in ar_values:
    if not np.isfinite(i): continue
    if i <= 0: continue
    alogr_values.append(np.log(i))

# print(alogr_values)

# # Fit a skew-normal distribution to the data
# a, loc, scale = skewnorm.fit(alogr_values)

# # Generate a range of values to plot the fitted distribution
# x = np.linspace(min(alogr_values), max(alogr_values), 1000)
# fitted_pdf = skewnorm.pdf(x, a, loc, scale)

# # Plot the histogram and the fitted distribution
# plt.figure(figsize=(10, 5))
# plt.hist(alogr_values, bins=30, density=True, alpha=0.6, color='blue', label='Log(R) Data')
# plt.plot(x, fitted_pdf, 'r-', label=f'Fitted Skew-Normal Distribution\nshape={a:.2f}, loc={loc:.2f}, scale={scale:.2f}')
# plt.title('Fit of Skew-Normal Distribution to Log(R) Data')
# plt.xlabel('Log(R) Value')
# plt.ylabel('Density')
# plt.legend()
# plt.show()

atheta_values = [data["theta"] for data in speed_list  if np.isfinite(data["theta"])]

mu_r, std_r = norm.fit(ar_values)
mu_theta, std_theta = norm.fit(atheta_values)

print(mu_r, std_r)
print(mu_theta, std_theta)

# Plot a histogram of speed frequencies
plt.figure(figsize=(18, 6))
plt.subplot(1, 3, 1)
plt.hist(ar_values, bins=20, density=True, edgecolor='black', alpha=0.7)  # Adjust bins as needed

xmin_r, xmax_r = plt.xlim()  # Get x-axis limits for the curve
r = np.linspace(xmin_r, xmax_r, 100)
pdf_r = norm.pdf(r, mu_r, std_r)
plt.plot(r, pdf_r, 'k', linewidth=2)

# Add titles and labels
plt.title('R Velocity Distribution of the Fish')
plt.xlabel('R Velocity (pixel/s)')
plt.ylabel('Frequency of Occurrence')
plt.grid(True)

# plot log(R)
plt.subplot(1, 3, 2)
plt.hist(alogr_values, bins=30, density=True, edgecolor='black', alpha=0.7)  # Adjust bins as needed

# Fit a skew-normal distribution to the data
a, loc, scale = skewnorm.fit(alogr_values)
print("for log(R) a, log, scale = ", a, loc, scale)
# Generate a range of values to plot the fitted distribution
x = np.linspace(min(alogr_values), max(alogr_values), 1000)
fitted_pdf = skewnorm.pdf(x, a, loc, scale)

plt.plot(x, fitted_pdf, 'r-', label=f'Fitted Skew-Normal Distribution\nshape={a:.2f}, loc={loc:.2f}, scale={scale:.2f}')
plt.title('Fit of Skew-Normal Distribution to log(R) Data')
# Generate a range of values to plot the fitted distribution
x = np.linspace(min(alogr_values), max(alogr_values), 1000)
plt.plot(x, fitted_pdf, 'k', linewidth=2)


# Add titles and labels
plt.title('Log(R) Velocity Distribution of the Fish')
plt.xlabel('log(R) Velocity (pixel/s)')
plt.ylabel('Frequency of Occurrence')
plt.grid(True)

# plot theta
plt.subplot(1, 3, 3)
plt.hist(atheta_values, bins=20, density=True, edgecolor='black', alpha=0.7)  # Adjust bins as needed

xmin_t, xmax_t = plt.xlim()  # Get x-axis limits for the curve
t = np.linspace(xmin_t, xmax_t, 100)
pdf_t = norm.pdf(t, mu_theta, std_theta)
plt.plot(t, pdf_t, 'k', linewidth=2)

# Add titles and labels
plt.title('Theta Velocity Distribution of the Fish')
plt.xlabel('Theta Velocity (degree/s)')
plt.ylabel('Frequency of Occurrence')


# Show the plot
plt.grid(True)
plt.show()