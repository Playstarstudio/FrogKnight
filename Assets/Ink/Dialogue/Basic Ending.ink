INCLUDE globals.ink
EXTERNAL endGame(boolean)
//story
{spokenTo == false:
You made it to the end! Glad to see you're alright #speaker:Helpful Stranger #portrait:tutorial #layout:left
    This is the end of our game for now.
}
  -> main

=== main ===
~spokenTo = true
Are you ready to finish up? You can say no and keep playing if you'd like. #speaker:Helpful Stranger #portrait:tutorial #layout:left

+ [Yes]
    Great! I hope you enjoyed the game!
    ~  endGame(true)
    -> END
+ [No]
    Sounds good, I'll wait for you here to make sure you get out okay!
    -> END