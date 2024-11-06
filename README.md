# Valve Sensor Digital Twin
## Purpose
This app is made as digital prototype of industrial plug&play device, detecting rotation angle of valve of a pipeline. Algorithm of rotation detection from this digital twin can be easily translated to C or Python, and can used on any embedded device. Digital twin itself should be used to simplify the process of debugging of real prototype of device.

Simulated device is using "off-the-shelf 9-axis IMU" for detecting rotation angle of standart industrial valve. Device is mounting on valve handle and after calibration with one press of a button is able to detect turn of the handle (in degrees). As a raw data to calculate angle device uses vectors of gravity and magnetic field, receiving from IMU.

## Algorithm
Main algorithm used for detection of rotation angle, written in a file [Assets\NibrasGameStudio\Scripts\Algorithms\Algorithm.cs](Assets/NibrasGameStudio/Scripts/Algorithms/Algorithm.cs)

It consists of three main functions:
```
(Vector3 A, float gamma) GetAxisAngleFrom(Quaternion q)
```
This function calculates axis of rotation and turn angle from quaternion in stable and controlled way in range 0°-180°. For your own purposes, you may need to modify it. Article about [Quaternions and spaital rotations](https://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation) in Wikipedia could be a good starting point.

```
void Calibrate(Vector3 N, Vector3 G) {...}
```
This function doing initial calibration of algorithm. It 
- builds basises of two coordinate systems (of the sensor and of the world)
- then calculates quaternion of rotation between sensor CS and world CS. This quaternion is named **Qs2w**.
- then calculates quaternion of rotation between X axis of sensor and "base direction" (sum of gravity and magnetic field vectors, projected on XZ plane of sensor). This quaternion is named **Q0**.
- then function saving **Qs2W** and **Q0** for further usage.

```
(Vector3, float) CalculateRotation(Vector3 N, Vector3 G) {...}
```
This function calculates current rotation value of a valve. It
- calculates quaternion of rotation between X axis of sensor and "base direction" (sum of gravity and magnetic field vectors, projected on XZ plane of sensor). This quaternion is named **Q1**
- calculates quaternion of difference between **Q0** and **Q1**. This quaternion is named **dQ**.
- from **dQ** function calculate axis (as Vector3) and angle (in decimal degrees), and return them.


### IMPORTANT NOTICES!
1. In case of transmission of algorithm from Unity C# to other PLs, **MULTIPLE TIMES CHECK (!!!)** calculation results of library you are using for work with quaternions. Beware inverted notation of quaternions and clearly understand difference between rotation and transformation quaternions (or you would "have love" with them until hell melts). You can look to Unity realization of quaternion math in [their GitHub repo](https://github.com/Unity-Technologies/Unity.Mathematics/blob/master/src/Unity.Mathematics/quaternion.cs).
2. You **must** provide to functions filtered and normalized vectors N and G.
3. Digital twin is equipped with default filter, realized in [Assets\NibrasGameStudio\Scripts\Algorithms\BandPassFilter.cs](Assets/NibrasGameStudio/Scripts/Algorithms/BandPassFilter.cs)
But this filter is only for test purposes, for real applications I recommend to use Butterworth filter or something like this. Its main purpose is to filter HF noise (for example, from electric facilities nearby the sensor) and LF noise (for example, from slow changes of earth magnetic field). Ideally, filter must be tuned to sense only turns of valve made by human hands.
4. In environments with unstable magnetic fields (workshops, near big electric motors) it'd be better to use a backup magnetic field source (big magnet placed near base of valve).

## Requirements
### To run pre-compiled binary:
- Windows 7/8/10 and higher

## Usage
### Use pre-compiled binary
Zip archive with compiled app is [Binary\ValveSensorDT.zip](Binary/ValveSensorDT.zip).

**How to start?**
Pretty simple. Unzip it into a folder, and run ValveSensorDT.exe.

**Interface**
 1. *Panel "Valve attitude".* 3 controls here allow you to control spatial rotation of part of pipe with valve.
 2. *Panel "Valve rotation".* From here you can rotate valve handle itself. "Reset" button here returns valve in default rotation (zero rotation angle).
 3. *Panel "Visual settings".* From here you can control displaying helper rays. representing different important axis. *"Sensor axis"* are base axis of simulated IMU sensor. *"Base directions"* are vectors received from simulated accelerometer and magnetometer. *"Base vector"* is a sum of vectors from accelerometer and magnetometer, this vector is fundamental "pointing handle" for calculation. *"Rotation axis"* is an axis of current rotation.
 4. *Panel "Filter settings"*. It allows you to configure bandpass filters for accelerometer and magnetometer. It shows current strength of Low Pass (LP) and High Pass (HP) filtering. To change this settings, enter new values in input boxes and click appropriate "Apply" button.
 5. *Panel "Magnetic field settings"*. It allows you to switch presence of additional magnetic field sources. *"Dynamic magnetic field source"* adding to scene rotating "generator", which create strong and dynamically changing magnetic field. *"Backup magnetic field source"* adding to scene additional magnet, attached to pipe. It provides strong magnetic field, compensating noise from dynamic magnetic field source.
 6. *Panel "Debug output"*. This panel provides useful info for a programmer who creating the real prototype of a device. He can compare values from here with values from real device to minimize efforts during debugging of a translated algorithm.
 7. *Panel "Valve sensor"*. This UI panel simulates front panel of real device. Here you can see display with current rotation angle value, and a button "Calibrate", using (obviously!) for calibration.

**How to use**
After starting the app
1. From "Valve attitude" panel set up required attitude of a pipe with valve.
2. Calibrate sensor. For this, in "Valve rotation" click "Reset" button to set valve into initial position, then in "Valve sensor" click "Calibrate" button to calibrate sensor.
3. Turn valve handle using "Valve rotation", and watch how sensor detects this.

Of course, if you change valve attitude after this, sensor will loose calibration, so you'll need to calibrate it again.

## Questions
Feel free to ask me through PM if you have any questions or offers.

## Legal notice
This software is distributing under MIT license with Attribution Clause. You may read it in file [LICENSE.md](LICENSE.md)

All 3D-models, materials and textures used in this project are freeware and downloaded from [Unity Asset Store](https://assetstore.unity.com/) or from [SketchFab](https://sketchfab.com). All rights for mentioned assets are property of their creators.