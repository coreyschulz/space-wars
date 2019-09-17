//////////////////////////////////////////////////
"Space Wars: A Tragedy Story" 
Corey Schulz && JoCee Porter
CS 3500 
Fall 2017
//////////////////////////////////////

-GENERAL-
"If you keep people busy and confused, they're liable to think they're having fun."
//////////////////////////////////////////////////
To start you run the program, and enter the network and name of the player.  If you 
don't fill in these boxes, you will be given a warning message, and the game won't start until both are filled out.  

The game is very typical to what you would expect, you are a ship, and you try and shoot the other ships.  
You also want to avoid the sun, it will kill you if you touch the sun. Of which there can be multiple!? 
You will respawn in a few seconds after you die, based on the server's settings. 
//////////////////////////////////////

--MULTIPLAYER--
"Colors and lies get blacker as you add more."
//////////////////////////////////////////////////
"Space Wars: A Tragedy Story" is a multiplayer game. Because of the central server, multiple computers can connect at once. 
Have fun playing with all your best friends! Make sure each client's on the proper IP. 
Make a game night!
//////////////////////////////////////

---CONTROLS---
"It's strange how some choices mean nothing."
//////////////////////////////////////////////////
	UP ARROW: BOOST!
	LEFT ARROW: TURN LEFT!
	RIGHT ARROW: TURN RIGHT!
	SPACEBAR: FIRE!
//////////////////////////////////////

----EXTRAS----
"You cannot help what you long for."
//////////////////////////////////////////////////
Added features:
	-The scoreboard will update based on the scores, i.e. first place will be on top, last place will be on bottom!
	-HP for each person will be the same color as the ship! 
	-The bullets will shoot in random colors, adding for a great rainbow effect, livening up the scene.
//////////////////////////////////////

-----Challenges-----
"A true entertainer always keeps a smile on their face, right?"
//////////////////////////////////////////////////
For the longest time, our game had performance issues because of the way the scoreboardPanel was written. 
We'd have never guessed that's where the issue came from, but after hours and hours and hours of troubleshooting, 
Daniel ultimately figured it out! It was actually a quick fix for the panel to work! We just had to draw the HP bars out
of rectangles rather than images. I made one function, sibylSystem (anyone seen Psycho-Pass!?) and you just pass in the needed parameters
and the scoreboard'll be displayed in a loop! 

Getting the network data was confusing, but after we got past the initial confusion of callbacks, etc., it seems to make sense now. 
//////////////////////////////////////