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

URL: http://www.prism.gatech.edu/~agolinvaux3/GT_DeadWeek_alpha.html

The game features a student player evolving in a city-like scene, and interacting with zombies. 

The player:
- can walk and run around the scene
- has a health system attached to it (displayed in the HUD): energy decreases when the player runs, increases slowly when he walks, and increases more rapidly when he takes energy drinks (from the vending machines). When the health is too low, the player cannot run anymore
- the player can pick up and throw textbooks on the ground, modifying the weight of the backpack it is carrying
- when too heavy because of the backpack, the player is not able to run anymore

The zombies are controlled by AI which allows them to:
- follow a specific path around an area of the scene when in default mode
- are attracted by the player when he is in a certain range of them
- slow the player down when close to him
- are distracted (attracted) by the textbooks that the player throws away, if close enough to the book

The characters:
- two character models where downloaded from Fuse and animated through Mixamo. The player’s movement is animated through a blend tree. However, the backward animation is not yet added (the player for now moonwalks when going backwards!)

HUD includes:
- the energy and the weight system are displayed - the bars start blinking when the player runs low on energy or is reaching his max capacity for the backpack
- the inventory to show how many energy drinks and textbooks the player is currently carrying with him
- the timer shows you how much time there is left before the end of the level
- a text shows up to let the player know when he wins or loses

Game mechanics :
- in order to win, the player has to find the blue book and get back to the finishing point in order to finish the level and win. A green arrow helps him find the direction in which he has to go in order to first find the book, and then get back to the finishing point (shown by white particles flying towards the sky)
- if the time goes out before the player is able to successfully complete these goals, he loses
- if the player gets stuck, he can always restart the level by pressing ESC
- when the level is over, a button will appear on the HUD to restart it


Remaining Bugs:
- sometimes, when stuck in between a zombie and a wall, the player will be elevated into the air
- once in the air, the player falls back very slowly to the ground

Further improvements:
- atmosphere: sounds and lighting should still be added/modified in order to create a more spooky atmosphere
- the animations do not yet include a backwards walking animation : the player moonwalks instead!
- add a small map to know where to get the book instead of the arrow


Downloaded Resources from asset store:
- the character and its animations
- the camera controller script
- the health system script
- the terrain texture

There is only one scene to load. In order to test all the level features, make sure to:
- wander around the level, see you energy decrease as you run (as in the last assignment - you cannot run anymore if it is too low) and look for the vending machines to pick up energy drinks
- pick up textbooks on your way and see the weight of your backpack increase
- get followed and  by zombies and slowed down by them when they reach you
- when attacked by zombies, distract them by throwing a textbook close to them (lead them to the book if you throw it too far) and see their attention switch from you to the textbook
- get to the final book (a message will a appear when you pick it up) and get back to the finishing point before the time runs out to see the Victory message
- reload the game and start again!


******************************

Player Controls:

Key "W" and "S": Move forward and backward

Key “Q” and “E”: Turn around. You can hold mouse right click and move the mouse to do the same

Key "Shift": Hold to run

Key “C”: Pick up items when close enough to them and facing them

Key “X”: Throw items at your mouse pointing position (aim at enemies)

Key “V”: Use energy drink

Mouse Left: Hold and rotate to look around

Mouse Right: Hold and rotate to turn around

ESC: reload the scene