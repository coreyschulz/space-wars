//////////////////////////////////////////////////
"Space Wars: Serverside"
Corey Schulz && JoCee Porter
CS 3500 
Fall 2017
//////////////////////////////////////


 ______     ______   ______     ______     ______        __     __     ______     ______     ______    
/\  ___\   /\  == \ /\  __ \   /\  ___\   /\  ___\      /\ \  _ \ \   /\  __ \   /\  == \   /\  ___\   
\ \___  \  \ \  _-/ \ \  __ \  \ \ \____  \ \  __\      \ \ \/ ".\ \  \ \  __ \  \ \  __<   \ \___  \  
 \/\_____\  \ \_\    \ \_\ \_\  \ \_____\  \ \_____\     \ \__/".~\_\  \ \_\ \_\  \ \_\ \_\  \/\_____\ 
  \/_____/   \/_/     \/_/\/_/   \/_____/   \/_____/      \/_/   \/_/   \/_/\/_/   \/_/ /_/   \/_____/


"Like I could ever get any of my friends to read this..."
ABOUT
//////////////////////////////////////////////////
Welcome! This is the Space Wars game server. This server handles clients connecting to the Space Wars game. 
It supports many connections at once, and players can join asynchronously from one another! 
//////////////////////////////////////


Change things up a bit.
THE SETTINGS FILE
//////////////////////////////////////////////////
In the settings file, you can tweek the game's variables quite easily. 
Pretty much everything relevant to the game mechanics are in there, so it's quite easy 
to customize the game to your heart's desire. 
//////////////////////////////////////


"Bullethell games are the best games." -- nobody
EXTRA MODE: TOUHOU MODE
//////////////////////////////////////////////////
In the settings file described above, you can set Touhou Mode to "true" to start the game right! 
In this mode, bullets will wraparound the screen for extra challenge. Bullets can come from anywhere
so be extra careful maneuvering your ship.
//////////////////////////////////////

"It sucks you in."
EXTRA MODE: BULLET GRAVITY
//////////////////////////////////////////////////
In the settings file described above, you can set Bullet Gravity to "true".
In this mode, bullets will be affected by each star's gravity, like a ship!
//////////////////////////////////////

"Darkness will prevail."
EXTRA MODE: SUPERMASSIVE
//////////////////////////////////////////////////
In the settings file described above, you can set Supermassive Mode to "true".
In this mode, each time a star is hit by a bullet, its mass increases! Be careful to not hit the 
star too many times, or else you'll be doomed to be stuck in a loop. Masochist.
//////////////////////////////////////


"Let's all have fun together."
THE GAME
//////////////////////////////////////////////////
How the client displays the game is up to that entity, but the game functions are 
the same across all platforms! Ships can shoot projectiles, decrement health, die, etc. 
//////////////////////////////////////

"Who wants a stylus?"
DESIGN DECISIONS
//////////////////////////////////////////////////
Making the game was fairly simple after we got the connection code in order. 
We did switch the dictionaries to ConcurrentDictionaries at some point to get rid
of some miscellanious exceptions that were strewn about. These dictionaries are made 
for parallel processing, so they fit quite nicely in this project. 

Other than that, save the few bugs that can always be expected in an assignment like this, we
didn't really have any other large problems and this assignment went smoothly overall!
//////////////////////////////////////


_This section doesn't deserve a cool quote._
UNIT TESTING
//////////////////////////////////////////////////
Unit testing this project is hard! Since we forgot about this section 
'till the last day of the project, we had tested this game as we went 
on programming it the entire time. Honestly, this was the worst part of all. 
It's hard to test things without access to the server / playing the game. 

We tried to write modular code to begin with, but it was still hard to test.
//////////////////////////////////////