INCLUDE globals.ink
// tags
#speaker:tutorial #portrait:tutorial #layout:left
{spokenTo == false:
    You made it to the end! Glad to see you're alright
    This is the end of our game for now
}
  -> main

=== main ===
~spokenTo = true
Are you ready to finish up in here? You can say no and keep playing

+ [Yes]
    //GAME END SCRIPT
    -> END
+ [No]
    Sounds good, I'll wait for you here to make sure you get out okay!
    -> END
