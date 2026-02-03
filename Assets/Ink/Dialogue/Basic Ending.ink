INCLUDE globals.ink
// tags
#speaker:Helpful Stranger #portrait:tutorial #layout:left

//story
{spokenTo == false:
    You made it to the end! Glad to see you're alright
    This is the end of our game for now.
}
  -> main

=== main ===
~spokenTo = true
Are you ready to finish up? You can say no and keep playing if you'd like.

+ [Yes]
    //GAME END SCRIPT
    Great! I hope you enjoyed the game!
    -> END
+ [No]
    Sounds good, I'll wait for you here to make sure you get out okay!
    -> END
