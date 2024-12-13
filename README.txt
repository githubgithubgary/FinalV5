I tried to put at much into it as possible.  I struggled with the Monster assignment and
the ability assignment so the code does not have alot of that build in.

I figured C level based on what I can manage in the game.

I used the Week 13 template to start with.  The menus are interactive and have lots of 
error handling.

The interface has lots of whitty lines of humor and some suttle movie references

When game starts it can auto load a random number of items into a random set of rooms.
If there are rooms with items when you restart the game the auto load does not occur.

The map also default to the current location of the active player on start up.

Most of the items for the player management effect the active player.  You can switch 
the active character around and then edit another character.

Player Maintenance Menu
	- List the active player and their equipment and current location
	- Add a new character and save to DB
	- You can use the Equip Player option to add a weapon to an existing player that is not all 
  ready carrying a weapon of some type	
	- You can get a list of all the players

Edit Menu
	- You can active another character if there is one (switch out one for another)
	- You can edit the active characters health, curent armor, weapon, potion and accessory
	but you can only choose a different item type if the player has more of that time.  It is a 
  like for like swap Weapon for Weapon, Potion for Potion.  If the player only has one weapon
  the you cannot swap it out.

Player Action Menu
	- You can just walk around the map/dungeon
		- You can pick up the items found in the room but unfortunately you cannot ever drop them.

Admin Menu
	- You can remove any character from the game
	- You can update any characters experience points
	- You can clear out all of the items in one room or all rooms

I did not implement any monsters or player actions, sorry just ran out of time.  I don't have
any ability to add rooms to the game either.