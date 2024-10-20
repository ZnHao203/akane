import cv2
import numpy as np
import os
import glob
import json


# path to folder for processing
folder_path = "set21_active"

# global variables
AREA_MIN_CONTOUR = 10
lower_brown_hsv = np.array([10, 80, 50])  # Lower bound (hue around 39, with wider range)
upper_brown_hsv = np.array([100, 255, 255])  # Upper bound (adjust saturation and value)

data_to_record = {folder_path:[]}

# Use glob to get a list of all image files (modify the extensions as needed)
image_files = glob.glob(os.path.join(folder_path, '*.png'))

# Count the number of image files
num_images = len(image_files)

# Print the result
print(f"Number of image files in the folder: {num_images}")

def get_bound_data(image, show_result):
    # Get the image dimensions (height, width)
    height, width = image.shape[:2]
    # Define the middle of the image
    mid_y = height // 2

    # Convert the image to grayscale
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

    # Apply Canny edge detection
    edges = cv2.Canny(gray, 50, 150, apertureSize=3)

    # Detect lines using Hough Line Transform (Probabilistic)
    lines = cv2.HoughLinesP(edges, rho=1, theta=np.pi/180, threshold=100, minLineLength=50, maxLineGap=10)

    # Initialize variables to track the min y in the upper half and max y in the lower half
    max_y_upper = -float('inf')
    min_y_lower = float('inf')

    # Iterate over the output lines and draw them on the image
    if lines is not None:
        for line in lines:
            x1, y1, x2, y2 = line[0]
            cv2.line(image, (x1, y1), (x2, y2), (0, 255, 0), 2)  # Green lines with thickness 2

            # Print the vertical (y) values of the line
            # print(f"Line detected: y1 = {y1}, y2 = {y2}")

            # Check for lines in the upper half
            if y1 < mid_y or y2 < mid_y:
                max_y_upper = max(max_y_upper, y1, y2)
            
            # Check for lines in the lower half
            if y1 > mid_y or y2 > mid_y:
                min_y_lower = min(min_y_lower, y1, y2)

        # Print the min y-value in the upper half and max y-value in the lower half
        print(f"Width of the image is: {width}")
        if max_y_upper != float('inf'):
            cv2.line(image, (0, max_y_upper), (width, max_y_upper), (0, 0, 255), 2)  # Red line
            print(f"max y-coordinate in upper half: {max_y_upper}")
        else:
            print("No lines detected in the upper half.")
        
        if min_y_lower != -float('inf'):
            cv2.line(image, (0, min_y_lower), (width, min_y_lower), (0, 0, 255), 2)  # red line
            print(f"min y-coordinate in lower half: {min_y_lower}")
        else:
            print("No lines detected in the lower half.")

    if show_result:
        # Show the result
        cv2.imshow('Lines Detected', image)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

    data = {"ymin": float(max_y_upper), "ymax": float(min_y_lower), "xmin": 0, "xmax": float(width)}
    return data


def get_fish_data(image, show_result):
    # Convert the image to HSV color space
    hsv_image = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    cv2.imshow('Image in hsv', hsv_image)

    mask1 = cv2.inRange(hsv_image, lower_brown_hsv, upper_brown_hsv)

    # Visualize the masked image
    masked_image = cv2.bitwise_and(image, image, mask=mask1)

    if show_result:
        # Show the original mask and the image with the mask applied
        cv2.imshow('Original Mask', mask1)
        cv2.imshow('Image with Mask Applied', masked_image)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

    mask = mask1

    # Find contours of the red object (the fish)
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    # print("cv2.findContours", contours)


    # Initialize variables to store the overall bounding box coordinates
    x_min, y_min = np.inf, np.inf
    x_max, y_max = -np.inf, -np.inf

    # Draw the contours on the original image and find the centroid
    for contour in contours:
        area = cv2.contourArea(contour)
        # print(area)
        if area > AREA_MIN_CONTOUR:  # Adjust the area threshold as needed
            # Get the bounding box coordinates for the contour
            x, y, w, h = cv2.boundingRect(contour)

            # Update the minimum and maximum x and y coordinates
            x_min = min(x_min, x)
            y_min = min(y_min, y)
            x_max = max(x_max, x + w)
            y_max = max(y_max, y + h)
    
    try:
        # Draw the rectangle on the original image
        cv2.rectangle(image, (x_min, y_min), (x_max, y_max), (255, 0, 0), 2)  # Blue rectangle with thickness 2


        # Print the position of the bounding box
        print(f"Combined bounding box position - X: {x_min}, Y: {y_min}, Width: {x_max - x_min}, Height: {y_max - y_min}")
    except:
        print("Error with rectangle bounding")

    if show_result:
        # Show the result
        cv2.imshow(f'Image {i} with Fish Position', image)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

    data = {"ymin": float(y_min), "ymax": float(y_max), "xmin": float(x_min), "xmax": float(x_max)}
    return data

#loop through img folder
# skip first few due to bad quality
# for i in range(4, 100):
for i in range(4, num_images):

    # Load the image
    image = cv2.imread(os.path.join(folder_path, f'frame_{i}.png'))

    #get fish rectangle data
    fish_data = get_fish_data(image, False)

    #get bound data - DRAW ON PIC!
    bound_data = get_bound_data(image, False)

    
    
    #record in json
    data_to_record[folder_path].append({
        "bound": bound_data,
        "fish": fish_data
    })

#finally write to file
with open(f"{folder_path}fishdata.json", "w") as final:
	json.dump(data_to_record, final, indent=4)
