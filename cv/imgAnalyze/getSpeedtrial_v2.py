import json
import matplotlib.pyplot as plt
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
    #update data
    speed_list.append({"x":delta_x, "y":delta_y})
    current += 1

# Extract velocity values for plotting
ax_values = [data["x"] for data in speed_list]
ay_values = [data["y"] for data in speed_list]
time_values = list(range(1, len(speed_list) + 1))
# Plot the speed in x and y directions
plt.figure(figsize=(10, 6))

# Acceleration in x direction
plt.plot(time_values, ax_values, marker='o', label='Velocity in X')

# Acceleration in y direction
plt.plot(time_values, ay_values, marker='o', label='Velocity in Y')

# Add titles and labels
plt.title('Velocity in X and Y Directions')
plt.xlabel('Time (seconds)')
plt.ylabel('Velocity (pixel/s)')
plt.legend()
plt.grid(True)

# Display the plot
plt.show()

data_to_record = {folder_path:speed_list}

with open(f"{folder_path}fishspeed.json", "w") as final:
	json.dump(data_to_record, final, indent=4)

acceleration_data = []
for i in range(1, len(speed_list)):
    ax = speed_list[i]["x"] - speed_list[i-1]["x"]
    ay = speed_list[i]["y"] - speed_list[i-1]["y"]
    acceleration_data.append({"ax": ax, "ay": ay})

# Extract acceleration values for plotting
ax_values = [data["ax"] for data in acceleration_data]
ay_values = [data["ay"] for data in acceleration_data]
time_values = list(range(1, len(acceleration_data) + 1))

data_to_record_acceleration = {folder_path:{"x": ax_values, "y": ay_values}}

with open(f"{folder_path}fishacceleration.json", "w") as final:
	json.dump(data_to_record_acceleration, final, indent=4)

# Plot the acceleration in x and y directions
plt.figure(figsize=(10, 6))

# Acceleration in x direction
plt.plot(time_values, ax_values, marker='o', label='Acceleration in X')

# Acceleration in y direction
plt.plot(time_values, ay_values, marker='o', label='Acceleration in Y')

# Add titles and labels
plt.title('Acceleration in X and Y Directions')
plt.xlabel('Time (seconds)')
plt.ylabel('Acceleration (pixel/s²)')
plt.legend()
plt.grid(True)

# Display the plot
plt.show()


def getAccelerationTransitProability(acceleration_data):
    # Sample acceleration data (x-direction accelerations)
    acceleration_x = acceleration_data  # Replace with your acceleration data

    # Define the states for acceleration: 1 for >0, 0 for =0, -1 for <0
    def get_state(acceleration):
        if acceleration > 0:
            return 1 # speedup
        elif acceleration == 0:
            return 0 # idle
        else:
            return -1 # speeddown

    # Calculate transitions between states
    state_transitions = {
        (1, 1): 0, (1, 0): 0, (1, -1): 0,
        (0, 1): 0, (0, 0): 0, (0, -1): 0,
        (-1, 1): 0, (-1, 0): 0, (-1, -1): 0
    }

    # Track the current and next state transitions
    for i in range(1, len(acceleration_x)):
        current_state = get_state(acceleration_x[i-1])
        next_state = get_state(acceleration_x[i])
        state_transitions[(current_state, next_state)] += 1

    # Count total transitions from each state
    state_counts = {1: 0, 0: 0, -1: 0}
    for (current, _), count in state_transitions.items():
        state_counts[current] += count

    # Calculate transition probabilities
    transition_probabilities = {
        (1, 1): state_transitions[(1, 1)] / state_counts[1] if state_counts[1] > 0 else 0,
        (1, 0): state_transitions[(1, 0)] / state_counts[1] if state_counts[1] > 0 else 0,
        (1, -1): state_transitions[(1, -1)] / state_counts[1] if state_counts[1] > 0 else 0,
        (0, 1): state_transitions[(0, 1)] / state_counts[0] if state_counts[0] > 0 else 0,
        (0, 0): state_transitions[(0, 0)] / state_counts[0] if state_counts[0] > 0 else 0,
        (0, -1): state_transitions[(0, -1)] / state_counts[0] if state_counts[0] > 0 else 0,
        (-1, 1): state_transitions[(-1, 1)] / state_counts[-1] if state_counts[-1] > 0 else 0,
        (-1, 0): state_transitions[(-1, 0)] / state_counts[-1] if state_counts[-1] > 0 else 0,
        (-1, -1): state_transitions[(-1, -1)] / state_counts[-1] if state_counts[-1] > 0 else 0
    }

    # Display transition probabilities
    print(transition_probabilities)


getAccelerationTransitProability(ax_values)
getAccelerationTransitProability(ay_values)