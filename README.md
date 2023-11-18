# Team Description
**ID:** 

  - Grup C

**Names:**

  - Victor Romero

<img src="/Images/VictorImg.jpg" style=" width:160px ; height:200px "  >

  
  - Eric Lopez
<img src="/Images/EricImg.jpg" style=" width:160px ; height:200px "  >



**Emails:**

  - victor.romero.2@enti.cat
  
  - eric.lopez@enti.cat


**Explanation Exercise 2:**

The most efficent way to do the interpolation in this exercise was to perform a simple lerp for all the joints separately.
This can be performed since all the rotations only need to be executed in one axis alone. 
This is also made to avoid breaking the robot constraints since it would start rotating on axis we don't want it to rotate to.

In addition, we implemented the Slerp which uses the quaternions directly, but as we've seen this was less efficient since it needs the whole quaternion as an argument in our case, 
therefore it is more expensive than just rotate in one axis each joint.
