using Godot;
using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;

class MovementControl
{
    // polar coordinates
    double r;
    // in degree
    double theta;

    double maxR;
    

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
        r = 0;
        theta = 0;
        currRAction = rAction.KEEP;
        currThetaAction = thetaAction.KEEP;

    }

    private double degreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0;
    }
    // converts polar coordinates to cartesian
    private Vector2I polarToCartesian()
    {
        var thetaInRad = degreeToRadian(theta);
        var x = Math.Round( r * Math.Cos(thetaInRad) );
        var y = Math.Round( r * Math.Sin(thetaInRad) );
        return new Vector2I( Convert.ToInt16(x), Convert.ToInt16(y) );
    }
    // return current speed
    public Vector2I getDisplacement()
    {
        updateAction();
        updateSpeed();
        return polarToCartesian();
    }
    // return how much to rotate - keep in mind the correct flipping is always assumed
    // TODO: this is very unstable
    // TODO: add a smooth transition
    public float getRotationDegrees()
    {
        // no rotate if idle
        if (currRAction == rAction.IDLE) return 0;
        double rotateDegree = theta;
        if (rotateDegree > 90) {
            rotateDegree = 180 - rotateDegree;
        }
        // no rotate if small angle
        if (rotateDegree < 30 && rotateDegree > -30) return 0;
        return (float) -rotateDegree;
    }
    // update action
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
            // 98% -> still idle
            // 2% -> speedup
            if (randomNumber < 2) currRAction = rAction.SPEEDUP;
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