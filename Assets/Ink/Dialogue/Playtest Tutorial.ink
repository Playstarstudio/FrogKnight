// tags
#speaker:tutorial #portrait:tutorial #layout:left

//Story start
Welcome! You can advance the dialogue with the "F" key 

This is going to be a playtest for the game Frog Knight!
By continuing this playtest, you are consenting to your data being gathered
Your identity will be anonymized and any data will not be connected with you
The 1-4 keys can be used to select dialogue options
Do you consent to your data being gathered?
* [Yes]
  -> main
* [No]
//GAME END SCRIPT
->  END

=== main ===
This is a turn-based rpg  where enemies move after you. When in dialogue, like now, you cannot move.
When you move, time advances, and enemies move according to how long it takes to complete the action.

+ [Yes]
    -> controls
+ [No]
    Alright, I'll let you get on with it.
    Beware the enemies in this cave, defend yourself against them!
    -> END


=== controls ===

Once outside of dialogue, you can use WASD to move around
E opens and closes your inventory, where you can right click to use items like potions
F picks up objects and lets you interact with things or even certain people!

To attack an enemy with your sword, simply walk into them.
To cast spells, use 1-4 on the keyboard and then click on the target. Each spell has a different effect
-> spells


=== spells ===
1 casts Magic Missile, dealing direct damage to an enemy
2 casts fireball, dealing area of effect damage to multiple opponents
3 casts heal, healing the target and 4 casts slow, slowing the target of your spell

Would you like me to repeat any of that?

+ [Spell Effects]
    -> spells
+ [Repeat Everything]
    -> controls
+ [No]
    Beware the enemies in this cave, defend yourself against them!
    -> END
