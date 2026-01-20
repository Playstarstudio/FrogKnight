-> main

=== main ===
Which pokemon do you choose? #speaker: Prof_Oak #portrait:bob_happy #layout:left
    + [Charmander]
        -> chosen("Charmander")
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Squirtle]
        -> chosen("Squirtle")
        
=== chosen(pokemon) ===
You chose {pokemon}!
-> END
