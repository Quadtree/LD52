# Aquaculture
Build a side-view terrarium-like hydroponic farm.

Several types of things to build:
- Pipe: Can't cross other pipe
- Filter: Removes a type of liquid, sends it out the bottom
- Pump: Collects liquids from the square. Number of pumps generally controls the output. So 2 pumps on green and 1 pump on water results in a 66%/33% mixture
- Outlet: Pushes liquid out

Three types of liquid:
- Red: Usually a toxin. Some plants like it
- Green: Usually a nutrient
- Blue: Water

Several types of plant
1. Foodleaf: Likes green: 33% water 66%, must be in light
2. Bitterleaf: Likes green: 50% water 50%. produces red, must be in light
3. AstroCoffee: Likes green 25%, water 75%, must be in shade
4. Muckroot: Likes red 50% water 50%. does not care about light. Produces green
5. Gasleaf: Likes water 100%, must be in light, produces red gas

Levels
1. Just grow some foodleaf. 1
2. Grow some bitterleaf and foodleaf. 1,2,4
3. Grow some astrocoffee and bitterleaf. 2,3,4,5
4. We don't have any green! Get some with gasleaf. Make some muckroot and bitterleaf. 1,2,4,5

