using Godot;
using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;
using NumSharp;

class MovementControl
{
    // polar coordinates
    double r;
    // in degree
    double theta;

    double maxR;

    /* timer to apply acceleration to r value */
    private static System.Timers.Timer rTimer;
    

    // possible action for a fish
    enum rAction
    {
        IDLE, // =0
        KEEP, // keep the same
        SPEEDUP,
        SPEEDDOWN,
    }

    enum thetaAction
    {
        KEEP, // keep the same
        INCREASE,
        DECREASE,
    }
    // current action for r and theta
    rAction currRAction;
    thetaAction currThetaAction;
    // initialize variables
    public MovementControl() {
        // initialize variables
        maxR = 40; // TODO: needs adjustments
        r = 50;
        theta = 0;
        currRAction = rAction.KEEP;
        currThetaAction = thetaAction.KEEP;

        // Create a timer with a 0.5 second interval.
        rTimer = new System.Timers.Timer(500);
        // Hook up the Elapsed event for the timer. 
        rTimer.Elapsed += OnTimedEvent;
        rTimer.AutoReset = true;
        rTimer.Enabled = true;
    }

    private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                          e.SignalTime);
        /* try a Gaussian model for r */
        updateAcceleration_v1();
    }

    private double degreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0;
    }
    // converts polar coordinates to cartesian
    private Vector2 polarToCartesian()
    {
        var thetaInRad = degreeToRadian(theta);
        var x = Math.Round( r * Math.Cos(thetaInRad) );
        var y = Math.Round( r * Math.Sin(thetaInRad) );
        // return new Vector2I( Convert.ToInt16(x), Convert.ToInt16(y) );
        return new Vector2((float)x, (float)y);
    }
    // return current speed
    public Vector2 getDisplacement()
    {
        /* this uses constant transition between speed */
        // updateAction();
        // updateSpeed();
		GD.Print("current speed is: " + r.ToString());        

        return polarToCartesian();
    }

    private double getNextGaussian(double mean, double std)
    {    
        // Generate a single Gaussian random number
        var gaussianValue = np.random.normal(mean, std);
		GD.Print("next gaussian value is: " + gaussianValue.GetDouble().ToString());

        return gaussianValue.GetDouble();
    }
    public void updateAcceleration_v1()
    {
        /* directly uses a mean and std from x/y direction, no polar conversion */
        /* use it on r */
        double rChange = getNextGaussian(-0.0098, 16.72);
        if (rChange > 0) {
            currRAction = rAction.SPEEDUP;
        } else if (rChange < 0) {
            currRAction = rAction.SPEEDDOWN;
        } else {
            currRAction = rAction.KEEP;
        }
        r += rChange;
        r = Math.Max(0, r); // need to keep r (speed) non-negative
        if (r == 0) {
            currRAction = rAction.IDLE;
        }

        double thetaChange = getNextGaussian(0.40, 55.54);
        if (rChange > 0) {
            currThetaAction = thetaAction.INCREASE;
        } else if (rChange < 0) {
            currThetaAction = thetaAction.DECREASE;
        } else {
            currThetaAction = thetaAction.KEEP;
        }
        theta += thetaChange;
        // Normalize theta to be within -180 to 180 degrees
        if (theta > 180) {
            theta -= 360;
        } else if (theta < -180) {
            theta += 360;
        }

        // Random random = new Random();
		// int randomNumber = random.Next(0, 100);
        
        
        // // update theta action - Mealy state machine
        // randomNumber = random.Next(0, 100);
        // if (currThetaAction == thetaAction.KEEP)
        // {
        //     if (theta % 180 < 30) 
        //     {
        //         // more horizontal movement -> likely to keep
        //         // 90% keep
        //         // 5% increase 
        //         // 5% decrease
        //         if (randomNumber < 5) currThetaAction = thetaAction.INCREASE;
        //         else if (randomNumber < 10) currThetaAction = thetaAction.DECREASE;
        //     } else
        //     {
        //         // up/down movement -> likely to change
        //         // 40% keep
        //         // 30% increase
        //         // 30% decrease
        //         if (randomNumber < 30) currThetaAction = thetaAction.INCREASE;
        //         else if (randomNumber < 60) currThetaAction = thetaAction.DECREASE;
        //     }
        // } else if (currThetaAction == thetaAction.INCREASE)
        // {
        //     // more prone to decrease
        //     // 20% increase
        //     // 40% decrease
        //     // 40% keep
        //     if (randomNumber < 40) currThetaAction = thetaAction.DECREASE;
        //     else if (randomNumber < 80) currThetaAction = thetaAction.KEEP;
        // } else if (currThetaAction == thetaAction.DECREASE)
        // {
        //     // more prone to increase
        //     // 20% decrease
        //     // 40% increase
        //     // 40% keep
        //     if (randomNumber < 40) currThetaAction = thetaAction.INCREASE;
        //     else if (randomNumber < 80) currThetaAction = thetaAction.KEEP;
        // }
        // return;
    }

    // return how much to rotate - keep in mind the correct flipping is always assumed
    // TODO: this is very unstable
    // TODO: add a smooth transition
    public float getRotationDegrees(float currentDegree)
    {
        // no rotate if idle
        if (currRAction == rAction.IDLE) return 0;
        double rotateDegree = theta;
        if (rotateDegree > 90) {
            rotateDegree = 180 - rotateDegree;
        }
        // no rotate if small angle
        // if (rotateDegree < 30 && rotateDegree > -30) return 0;
        int stepDegree = 1;
        if (rotateDegree - currentDegree > stepDegree) {
            return (float) currentDegree -1;

        } else if (rotateDegree - currentDegree < stepDegree) {
            return (float) currentDegree +1;
        }
        return (float) -rotateDegree;
    }
    // update action - this is constant percentage
    private void updateAction()
    {
        // TODO: currently it's just a simple state machine, improve it!
        // these numbers all need adjustment
        Random random = new Random();
		int randomNumber = random.Next(0, 100);
        // update r action
        // force reset
        if (r <= 0) currRAction = rAction.IDLE;
        // regular update - Moore state machine
        if (currRAction == rAction.IDLE) 
        {
            // idle state
            // 95% -> still idle
            // 5% -> speedup
            if (randomNumber < 5) currRAction = rAction.SPEEDUP;
        } else if (currRAction == rAction.SPEEDUP) 
        {
            // speedup state
            // 30% -> still speedup
            // 20% -> speed down
            // 50% -> keep
            if (randomNumber < 20) currRAction = rAction.SPEEDDOWN;
            else if (randomNumber < 90) currRAction = rAction.KEEP;
        } else if (currRAction == rAction.KEEP) 
        {
            // keep state
            // 80% -> still keep
            // 10% -> speeddown
            // 10% -> speedup
            if (randomNumber < 10) currRAction = rAction.SPEEDUP;
            else if (randomNumber < 20) currRAction = rAction.SPEEDDOWN;
        } else if (currRAction == rAction.SPEEDDOWN)
        {
            // speeddown state
            // 30% -> still speeddown
            // 20% -> speed up
            // 50% -> keep
            if (randomNumber < 20) currRAction = rAction.SPEEDUP;
            else if (randomNumber < 70) currRAction = rAction.KEEP;
        }
        
        // update theta action - Mealy state machine
        randomNumber = random.Next(0, 100);
        if (currThetaAction == thetaAction.KEEP)
        {
            if (theta % 180 < 30) 
            {
                // more horizontal movement -> likely to keep
                // 90% keep
                // 5% increase 
                // 5% decrease
                if (randomNumber < 5) currThetaAction = thetaAction.INCREASE;
                else if (randomNumber < 10) currThetaAction = thetaAction.DECREASE;
            } else
            {
                // up/down movement -> likely to change
                // 40% keep
                // 30% increase
                // 30% decrease
                if (randomNumber < 30) currThetaAction = thetaAction.INCREASE;
                else if (randomNumber < 60) currThetaAction = thetaAction.DECREASE;
            }
        } else if (currThetaAction == thetaAction.INCREASE)
        {
            // more prone to decrease
            // 20% increase
            // 40% decrease
            // 40% keep
            if (randomNumber < 40) currThetaAction = thetaAction.DECREASE;
            else if (randomNumber < 80) currThetaAction = thetaAction.KEEP;
        } else if (currThetaAction == thetaAction.DECREASE)
        {
            // more prone to increase
            // 20% decrease
            // 40% increase
            // 40% keep
            if (randomNumber < 40) currThetaAction = thetaAction.INCREASE;
            else if (randomNumber < 80) currThetaAction = thetaAction.KEEP;
        }
        return;
    }
    // update speed based on past speed
    private void updateSpeed()
    {
        // TODO: these increase/decrease amount also need to be verified
        // update r
        if (currRAction == rAction.IDLE)
        {
            r = 0;
        } else if (currRAction == rAction.SPEEDUP)
        {
            r += r / 10 * 1 + 1;
        } else if (currRAction == rAction.SPEEDDOWN)
        {
            if (r > 0) r -= r / 10 * 2 + 1;
        }

        // update theta
        if (currThetaAction == thetaAction.INCREASE)
        {
            theta += 5;
        } else if (currThetaAction == thetaAction.DECREASE)
        {
            theta -= 5;
        }
        if (theta % 360 == 0) theta = 0; // +-360 degree is just 0 degree
    }

    public void increaseSpeed()
    {
        currRAction = rAction.SPEEDUP;
        r += 20;
    }
}