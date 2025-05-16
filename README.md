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

7. # Aid Kit
- scale 100,100,100
![image](https://github.com/user-attachments/assets/12bf5ded-0a17-4d91-ba1d-1aa7fc85e0ed)
![image](https://github.com/user-attachments/assets/68abcf54-1378-4ee3-943c-14fc4f368d43)

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



📝 Revised Transition Table
From State	To State	Conditions
Sitting	Waving	waving == true && saved == false
Waving	Sitting	waving == false
Sitting	Turn Right	turn_right == true && saved == false
Turn Right	Walk	walk == true && angleToKit < 30°
Turn Right	Turn Right	walk == true && angleToKit >= 30° (loop turn)
Turn Right	Sitting	turn_right == false (emergency exit)
Walk	Pickup	pickup == true && kitDistance <= 0.5f
Walk	Sitting	pickup == false && kitDistance > 5f (emergency exit)
Pickup	Sitting	pickup == false && ExitTime=0.9
 pickup == false && sitting == true
Final: At end of sequence, saved = true
