# Echoes-In-The-Frost
Unity-based action-adventure rescue game where players pilot a drone through a deadly blizzard to save three stranded survivors. The game showcases dynamic terrain, AI-based character interactions, and cinematic camera mechanics under extreme weather conditions.

1. # Terrian Setup 
   ![image](https://github.com/user-attachments/assets/e6f9142d-9e96-4af6-a968-227284fd04af)
- code: Train_layers
- this code makes a mix between montian nd snow with a fractrial noise 
![image](https://github.com/user-attachments/assets/3cd2d94b-a106-4437-94fd-f042d0448a9d)
![image](https://github.com/user-attachments/assets/b7501162-1aac-49b1-b3b6-609f1b16e648)

2. # Skybox
 - drag and drop material 
   ![image](https://github.com/user-attachments/assets/88a909fb-37e8-45da-b618-ed9d56ebdf9d)

3. # Camera
- added flare in main camera
- disabled input
- sphere orbit
- recentering 
   ![image](https://github.com/user-attachments/assets/e6f320e2-5f0d-4467-bf8d-b83df5d16a35)


4. # Drone 
- postionong scaling ...
- added mass , linear daming , angular damping and a slightly shifted center
- added move script
- 
![image](https://github.com/user-attachments/assets/050f08e7-ed6a-4773-bd42-0cc15679dc7d)
![image](https://github.com/user-attachments/assets/176a94b0-5ad3-4db5-be34-945f483b5b01)



5. # Characters
   -
![image](https://github.com/user-attachments/assets/65403b1f-7248-4589-a3fa-15a45e5cbc13)
![image](https://github.com/user-attachments/assets/944e0005-593d-4cf7-ae14-389d1c36763b)



6. # Particles
- FIRE
 - taken lecture practice as prefab and re used
   
- SNOW
  - took a remdered installed material and reused
  - i will create a small snow box follows drawn to make sure alwyas visable 
    
- FOG
   - created in the entire scene
 ![image](https://github.com/user-attachments/assets/11a59239-d42e-45a3-bc43-ab5db57c5308)

![image](https://github.com/user-attachments/assets/7aac8c6b-a165-48a1-9d01-effc6ecb66a6)

7. # First Aid Kit
- scale 100,100,100
- increased colider size so animations can interact with it better 
![image](https://github.com/user-attachments/assets/12bf5ded-0a17-4d91-ba1d-1aa7fc85e0ed)
![image](https://github.com/user-attachments/assets/68abcf54-1378-4ee3-943c-14fc4f368d43)
![image](https://github.com/user-attachments/assets/600d3c84-70dc-4eef-a523-cc7422ca0285)



8. # Animation
- set loop, and no looping
  
| Animation Clip | Loop Time? | Notes                     |
| -------------- | ---------- | ------------------------- |
| Sitting        | ✅ Yes      | Idle, default             |
| Waving         | ✅ Yes      | Can be triggered often    |
| TurnRight      | ❌ No       | One-time direction change |
| Walk           | ✅ Yes      | Movement animation        |
| Pickup         | ❌ No       | Final one-time action     |


![image](https://github.com/user-attachments/assets/521ac9c3-e3c0-4bd9-a4ae-b698bd8cc3c0)
- Logic covers both Drone Wave and Kit Pickup flows with fallback to Sitting

| **Parameter** | **Purpose / Triggered When**                              |
| ------------- | --------------------------------------------------------- |
| `sitting`     | Default state — idle character posture                    |
| `waving`      | When drone is near (proximity logic triggers this)        |
| `turn_right`  | Used to face the First Aid Kit (always turns right)       |
| `walk`        | Move toward the First Aid Kit using root motion           |
| `pickup`      | Plays the animation to pick up the First Aid Kit          |
| `saved`       | Indicates the character has completed the rescue sequence |

![image](https://github.com/user-attachments/assets/1d59c52b-9e8f-4c3f-a1fe-a73c3dd7a1af)

9. # Particle Effects
    - Fire
![image](https://github.com/user-attachments/assets/22a0e774-53bf-4c95-b7a2-f41bad6cb755)

![image](https://github.com/user-attachments/assets/1efbea3a-f8a0-4965-8e09-cb179586a1a0)

   - Snow
    - made the simultion to be world to stop trurning with player
     ![image](https://github.com/user-attachments/assets/c1ac6d95-83c9-44ec-8e52-079f9cad254e)

![image](https://github.com/user-attachments/assets/a36e87cb-b8f9-4049-8905-db820704caf1)

![image](https://github.com/user-attachments/assets/bbbe2e60-df48-4d63-a110-861b15da4480)

   - Fog
     
![image](https://github.com/user-attachments/assets/b670d096-0fbf-4c26-ad83-c6b353e110fa)


10. # Game Dynamics

![image](https://github.com/user-attachments/assets/4811b938-5ead-438f-8dab-ece008b46481)

![image](https://github.com/user-attachments/assets/51afdf0a-5920-4922-8a65-e79f51d55b79)


11. # Drone Damadge and Crash Logic

- code 
 - The drone gets damaged when it hits any object with a Relative Velocity greater than 3
 - The drone can only withstand 3 collisions. 
 - After each hit, the drone becomes immune to damage for 5 seconds, preventing additional 
damage during that period.
 - After the third hit: 
   - The drone crashes, triggering the Fire particle effect. 
   - A "Mission Failed !" message should be displayed. 
   - After 10 seconds, either restart the game or end it.
     
12. # Wining condition
code :
- Once all three characters have successfully received and picked up their kits, a "Mission 
Accomplished !" message must be displayed.
- After 10 seconds, either end the game or restart it.


BONUS :

A] PROFILE PICTURES HUD
- Display the profile pictures of Steve, Pete, and Kate on the lower left corner of the screen. 
- When a character is saved, a blue check mark should appear on their picture.
![image](https://github.com/user-attachments/assets/e91eddbc-5732-4926-8391-69988cf49337)

![image](https://github.com/user-attachments/assets/0f02a42c-d725-4971-9382-df71baa965d2)

![image](https://github.com/user-attachments/assets/9fb9e65a-2aff-42d9-9772-d22c84a5a629)


B] SOUND MUSIC 
- Add a background music track of your choice. 
- Play a snow blizzard sound effect during gameplay. 
- Play a propeller sound when the drone's rotors are spinning (i.e., when the SPACE BAR is 
pressed).
- added a hit sound on the ground to indicate a damdging hit 

![image](https://github.com/user-attachments/assets/f676d4e0-9bcd-4544-97ba-c672a6161e3c)



