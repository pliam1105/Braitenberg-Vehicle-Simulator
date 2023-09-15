# Braitenberg-Vehicle-Simulator
A simulated environment for analyzing the behaviors of the Braitenberg Vehicles, using Unity.

https://github.com/pliam1105/Braitenberg-Vehicle-Simulator/assets/34167782/f8d6cd16-ec84-404f-9eb5-6490ee4878bc

## Overview
The environment consists of:
* Some *vehicles*, which have two sensor positions, with a set of sensors in each one, and two motors, whose speed is determined by the sensor output.
<table><tr><td align="center"><img src="MEDIA/vehicle.png" width="50%"></td></tr><table><br>

* Some *sources*, which radiate some type e.g. heat, temperature, which is represented from a number in $[0, number\ of\ types-1]$
<table><tr><td align="center"><img src="MEDIA/sourcebig.png" width="50%"></td></tr><table><br>

* Some *constraints*, which are some time-dependent areas where the vehicles cannot enter (they are represented by blocks with walls where the vehicles bounce on)
<table><tr><td align="center"><img src="MEDIA/constraintbig.png" width="50%"></td></tr><table><br>