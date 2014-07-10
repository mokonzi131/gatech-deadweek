******************************

Chenglong Jiang
avaloncy@gmail.com
cjiang33

Michael Landes
Mokonzi131@gmail.com

Arnaud Golinvaux
arnaud.golinvaux@gatech.edu
agolinvaux3

Michael Yao 
yaoninja@gmail.com
cyao3

Josephine Simon
josephine.simon@gatech.edu
jsimon34

******************************

Web Plugin address: http://www.prism.gatech.edu/~agolinvaux3/MilestoneAI.html 

All of the compulsory requirements as well as the extra credit work has been completed in this assignment, and are illustrated by 3 NPC enemies:

1/ each NPC is controlled by a state machine composed of 4 different states:
- the default “walk” state in which the enemy follows a personal pre-defined route by joining waypoints
- an “alert” state triggered when the enemy sees the player from far away: it gets closer to it
- an “attack” state: when the enemy is in throwing range, it aims at the player and throws an axe at it
- a “melee attack”: when the enemy is close enough, it with run to the player to attack it one on one
- a “hide idle” state: when the player aims at an enemy with its own axe, if the enemy is close enough to a tree, it will hide behind it for a few seconds before coming back out again

2/ in their first state, each NPC follows its own path on its side of the scene. The path is defined by a certain number of waypoints it must reach in a given order

3/ When the enemy is throwing the axe at the player, it will take the player’s speed and distance into consideration in order to extrapolate a future position to which the axe can reach the player, and throw it in the right direction. An approximation in the time of flight of the axe according to the player’s final destination was made, so that the result is not always exactly accurate. However, in most cases, it works reasonably well.

EXTRA CREDIT: the path between each waypoint in the first state of the NPCs is computed with an A* implementation (Astar class). First, a discretized map of the scene is built by defining the cells containing an obstacle. Then the path between each waypoint is computed once at the beginning and stored. The character is moved from one cell to another with the  characterController.Move function.
To use the NavMeshAgent for the path following instead of the A* algorithm, simply switch the “AIScriptHybridWithHide” script attached to the enemy with the “AIScriptNavMeshWithHide” one.


Downloaded Resources from asset store:
- the character and its animations
- the camera controller script
- the health system script
- the terrain texture and trees


There is only one scene to load. In order to experience all the particularities of this level, try the following:
- wander around the level, see you energy decrease as you run (as in the last assignment - you cannot run anymore if it is too low) and look for the vending machines to pick up energy drinks
- get closer to an enemy and see its state change to alert
- get close enough that it will start throwing its axe at you
- even closer, the enemy will run to you and attack you directly
- go to the enemy patrolling around the trees and through your axe at him: watch him hide behind the closest tree
- run far away from an enemy that it will go back to patrolling its assigned path



Player Controls:

Key "W" and "S": Move forward and backward

Key "A" and "D": Turn around. You can hold mouse right click and move the mouse to do the same

Key "Q" and "E": Move on the side

Key "Space": Jump

Key "Shift": Hold to run

Key "F": Pick up items at your mouse pointing position (for vending machines)

Key "T": Throw items at your mouse pointing position (aim at enemies)

Key "C": Use energy drink

Mouse Left: Hold and rotate to look around

Mouse Right: Hold and rotate to turn around