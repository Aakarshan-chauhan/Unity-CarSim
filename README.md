# Unity-CarSim

My try at making a car sim as realistic as possible while still keeping it lighter than other sims like Carla or Nvidia Drive.

Unity MLAgents package has been added to the project to allow for Agent training.

### Observations (stacked : 3)
  1. 64 x 64 RGB Image from the camera sensor. 
  2. The speed of the car in Km/h.
  3. The distance between the car and the finish line.

### Actions
The actions are sampled from a discrete action space with 10 possible values
-  0 | Forward          (vertical = 1 , horizontal = 0, Handbrake = 0)
-  1 | Forward - Right  (vertical = 1 , horizontal = 1, Handbrake = 0)
-  2 | Right            (vertical = 0 , horizontal = 1, Handbrake = 0)
-  3 | Back - Right     (vertical = -1 , horizontal = 1, Handbrake = 0)
-  4 | Back             (vertical = -1 , horizontal = 0, Handbrake = 0)
-  5 | Back - Left      (vertical = -1 , horizontal = -1, Handbrake = 0)
-  6 | Left             (vertical = 0 , horizontal = -1, Handbrake = 0)
-  7 | Forward - Left   (vertical = 1 , horizontal = -1, Handbrake = 0)
-  8 | Handbrake        (vertical = 0 , horizontal = 0, Handbrake = 1)
-  9 | Nothing          (vertical = 0 , horizontal = 0, Handbrake = 0)

## To-do 
1. Make better wheel colliders ( Raycast based )
2. Use a suitable Car model
3. Add more details to the scene


