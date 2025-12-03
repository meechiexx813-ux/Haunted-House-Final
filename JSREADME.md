# Haunted House – Modified Stealth Game  
## Final Project – Jan Di Sapone & Demetrius Dorsey

## Project Overview
This project is a modified version of the Haunted House stealth game. It includes one major gameplay modification and one minor visual modification. These changes were created to expand player interaction and atmosphere while still maintaining the original structure of the assignment.

## Major Modification: Charged Dash with Cooldown and UI Bar
For the major modification, we redesigned the dash mechanic entirely. Instead of a simple, instant dash, the player now holds the dash key to charge power. The dash distance is determined by the length of the charge, and a fully charged dash allows the player to teleport a short distance. A UI slider bar was added to show cooldown progress after using the dash, preventing the player from using it repeatedly.

To implement this system, we added a new Input Action for the charged dash, rewrote sections of the original movement script, and introduced charge duration, dash distance, cooldown logic, and teleportation functionality. We also connected the system to a UI Slider to visually represent the cooldown timer. This modification significantly changes how players move through the map and escape enemies.

## Minor Modification: Ghost Flicker Visibility Effect
The minor modification introduces a ghost flicker effect. Ghosts now fade in and out of visibility over short intervals. This effect is purely visual and does not impact gameplay, movement, or detection. It was achieved by adjusting the ghosts' material alpha values over time. While subtle, this change adds extra atmosphere without altering core mechanics.

## Repository
https://github.com/meecheixx813-ux/Haunted-House-Final

## Drive Playthrough Link
https://drive.google.com/drive/folders/1oTdq27bE_dRVSysehXJIKDbXFa4KU0Q?usp=drive_link

## Video Link
https://youtu.be/AjYl3KceWIk
