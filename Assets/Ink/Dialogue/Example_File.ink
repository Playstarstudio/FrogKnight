//Comments can be written as such and are not displayed
The story starts here //This is the first line of text shown in the game
-> knotName //This diverts to the knot "knotName", skipping over the next line of text

This line of text is skipped :)

=== knotName ===
This is the content of the knot. Anything can be written in here
Knots can be visited multiple times in a story, allowing for 

* This is a simple choice //This is choice can be chosen only once
    This displays only after choosing the simple choice

+ This is a sticky choice //Sticky choices can be chosen multiple times
    This displays only after choosing the sticky choice
    
* Try [it] this example! //This is a complex example of a player choice option
    This displays only after choosing the complex choice
    
- This is a gather //This makes prior choice splits return to one spot

The example is now over //The next line is a special kind of divert, ending the dialogue
-> END //This ends the entire dialogue
