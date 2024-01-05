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

⁤In Exercise 2, we used an approach where each rotation was individually lerped by a global value. ⁤⁤This value is increased during each iteration, as it is multiplied by a speed value, making it easily adjustable. ⁤⁤Since each joint rotates only in one axis, it makes this approach more simple. ⁤

⁤After that, the rotation of each joint is multiplied by the accumulated rotation. ⁤⁤This is done to facilitate rotation in local space rather than global space.
