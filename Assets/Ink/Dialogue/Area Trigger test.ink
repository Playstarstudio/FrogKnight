INCLUDE globals.ink

This is a test of the area trigger script

After this is over, you should no longer be able to trigger this.

Ok, bye now :)

+ [Yes]
-> reset

+ [No]
 Too bad
 -> reset


=== reset ===
~ resetAreaTrigger(false)
-> END