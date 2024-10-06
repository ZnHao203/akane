// motionCapture.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

//#include <iostream>

//int main()
//{
   /* std::cout << "Hello World!\n";
}*/

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file


#include <iostream>
#include <string>
#include <Windows.h>
#include <opencv2/opencv.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace cv;
using namespace std;

bool takeImage = false;

void demoDisplayImg()
{
    std::string image_path = "C:/Users/hower/Documents/akane/cv/capture/fishTurn.png";
    Mat img = imread(image_path, IMREAD_COLOR);

    imshow("Display", img);
    int k = waitKey(0); // Wait for a keystroke in the window
    destroyWindow("Display");
}

void demoPlayVideo()
{
    namedWindow("Window2", WINDOW_AUTOSIZE);
    VideoCapture cap;
    cap.open("video path");
    Mat frame;
    while (1) {
        cap >> frame;
        if (!frame.data) break; // ran out of film
        imshow("Window2", frame);
        if (waitKey(33) >= 0) break;
    }
}

void CALLBACK TimerProc(HWND hwnd, UINT message, UINT idTimer, DWORD dwTime)
{
    takeImage = true;
}

int main()
{
    //demoDisplayImg();

    namedWindow("Window3", WINDOW_AUTOSIZE);
    VideoCapture cap;
    cap.open(0);

    if (!cap.isOpened()) {
        // check if succeed
        std::cerr << "COULD NOT OPEN CAPTURE. " << std::endl;
        return -1;
    }

    // timer every 1 sec
    HWND hwnd = NULL;
    UINT_PTR timerID = SetTimer(hwnd,             // handle to main window 
        1,            // timer identifier 
        1000,                 // 10-second interval 
        (TIMERPROC)TimerProc);     // no timer callback 
    if (timerID == 0) {
        // handle error
        return 1;
    }

    int counter = 0;
    string filename = "";
    Mat frame;
    int result;
    while (1) {
        cap >> frame;
        if (!frame.data) break; // ran out of film
        imshow("Window3", frame);
        if (waitKey(33) >= 0) break;

        // save frame every second
        if (takeImage) {
            takeImage = false;
            if (counter < 1000) {
                filename = "frame_" + to_string(counter) + ".png";
                cout << filename << endl;
                result = imwrite(filename, frame);
                if (result != true) {
                    cerr << "COULD NOT SAVE CAPTURE. " << endl;
                }
            }
            counter += 1;
        }
        
    }

    return 0;
}
