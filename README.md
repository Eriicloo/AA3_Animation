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



**Exercise 2 AA3 Explanation:**

In Exercise 2, we used an approach where each rotation was individually lerped by a global value. This value is increased during each iteration, as it is multiplied by a speed value, making it easily adjustable. Since each joint rotates only in one axis, it makes this approach more simple.

After that, the rotation of each joint is multiplied by the accumulated rotation. This is done to facilitate rotation in local space rather than global space.

**- Angular Velocity:**

impactVector = (contactPoint - transform.position).normalized;<br />
angularMomentum = Vector3.Cross(impactVector, mData.velocity.normalized);<br />
angularVel = angularMomentum * (effectStrength.value * -1);<br />

impactVector is the vector between the contactPoint, which is the moment of colision between the tail and the ball, and the own position of the ball.<br />
We use the effectStrength to give a value of power in order to shoot the ball with more or less velocity.

**- Magnus Force:**

The formula of the Magnus Force we used is the following:

L = p * vfree * (2 PI r)^2 * f * l<br />
**L = density * freeStream * Mathf.Pow((2.0f * Mathf.PI * 0.25f), 2) * 1.0f * angularVel**

Where:

L = Magnus Force (Newtons)<br />
density = Air density of the fluid (km/m^3)<br />
freeStream = The free velocity of the fluid (m/s)<br />
Mathf.Pow((2.0f * Mathf.PI * 0.25f), 2) = Radius<br />
1.0f = Length<br />
f = Angular Velocity<br />


**Exercise Location**

The order of each location is represented in this order: Object in scene (if existent), Script & Function

Exercise 1:
 1. IK_tentacles, NotifyShoot()
 2. BlueTarget, MovingTarget, Update()
 3. Scorpion, IK_Scorpion, Update()
 4. Main Camera, Reset, Update()

Exercise 2:
 1. Scorpion, IK_Scorpion, Update()
 2. Ball, MovingBall, OnCollisionEnter()
 3. Ball, MovingBall, CalculateTrajectory() & Update()
 4. Ball, MovingBall, Update()


Exercise 3:
 1. Scorpion, IK_Scorpion, UpdateFutureLegBases()
 2. Located inside Obstacles Object in Unity Editor
 3. MyScorpionController (DLL) -> LerpLegs()
 4. Scorpion, IK_Scorpion, UpdateBodyPosition()
 5. Scorpion, IK_Scorpion, UpdateBodyRotation()


Exercise 4:
 1. MyScorpionController, updateTail() & TargetApproach(Vector3 target)
 2. MyScorpionController, TargetApproach(Vector3 target) & NewError(int i)


Exercise 5:
 1. MyOctopusController, update_ccd()
 2. OctopusBlueTeam, NotifyShoot()
 3. TargetFollow, TargetFollow, InitFollowTarget(Transform newTarget)
