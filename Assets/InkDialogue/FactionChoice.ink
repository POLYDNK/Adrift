Welcome to the Game! #speaker:??? #portrait:default #layout:right
Adrift is a strategy role playing game with roguelike elements. Essentially try and make the best choices you can and try and win the game!
How do you win? Well the goal is simple! Play as the pirate Fisher "Cutthroat" Soren and reclaim your soul from Davy Jones!
Hey thats me! #speaker:Fisher Soren #portrait:fisher #layout:left
How you get your soul back is up to you and what faction you choose in the game. #speaker:??? #portrait:default #layout:right
Speaking of factions lets have you pick one to find out more about it. 
-> main

=== main ===
Which faction would you like to see?
+ [Pirate]
    Ain't nothing worth stealing that I haven't stole yet! Yar har har! #speaker:Allan Vayne #portrait:allen #layout: right
    So ... you wanna be a pirate do ya?
    Well great! Ready to have you aboard. Pirates play best keeping their men happy and their coin full.
    We're pirates after all, when theres something we want... we go and get. You hear me laddy?
    -> chosen("Pirate")
+ [Merchant]
    If you'd like to make money and a lot of it come and join me! #speaker:Neumann Reluft #portrait:neumann #layout:right
    Why fight all the time when you can buy your way into the future?
    Don't you see the possibility? With money anything can be purchased. 
    Perhaps ... even the thing you most desire.
    -> chosen("Merchant")
+ [Monarchy]
    The royals require your service. Should you accept we will wipe your slate clean.
        #speaker:Rolvo Valeria #portrait:rolvo #layout:right
    A quick wit and sharp tongue can aid a person such as yourself as any blade or cannon.
    Why put your crew at risk pillaging when you can work together with the people who get things done in the world. 
    Think about that next time you voyage accross the seas. I wouldn't want to lose an amicable fellow.
    -> chosen("Monarchy")
    
=== chosen(faction) ===
Are you sure about {faction}? #speaker:??? #portrait:default #layout:right
+ [Yes]
    -> choice(faction)
+ [No]
    -> main
=== choice(faction) ===
I'm choosing {faction}! #speaker:Fisher Soren #portrait:fisher #layout:left
An excellent choice! Good luck pirate and may your future be fortunate! #speaker:??? #portrait:default #layout:right
->  END
