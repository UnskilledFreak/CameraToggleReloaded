# CameraToggleReloaded
This is the successor of the old standalone CameraToggle i used ~~a while~~ 4 years ago (as of dec 2023). This is the first time CameraToggle is a complete mod and not just a workaround which modifies config files and wishes for a reload by CameraPlus.

First of all, it is finally functional with Camera2 and CatCore... and that's basically it. It is also more like an interactive scene switcher for Camera2... for now.

However it is controlled either by you or your chat!

## Features
- use whatever command for CameraToggleReloaded to react to it (default is `!ctr`)
- on-the-fly scene switch controlled by chat
- Cooldown option per user or global
- dynamic setup for each scene
- each scene can be bound to multiple modes (Menu, Replay, Modmap, Solo...)
- each scene can have its own command

## Dependencies
- BSIPA ^4.2.2
- BSML ^1.6.3
- Camera2 ^0.6.100
- CatCore ^1.0.0
- SiraUtil ^3.0.6

## How to Use
-- todo --

## Future plans
- Twitch Reward system
- queue scenes for next upcoming scene switch (eg toggled a switch for a game mode which is not currently running)
- independent cam to scene assignment (basically on-the-fly scene creation)

## Special Thanks
- [FEFELAND](https://www.twitch.tv/fefeland) for providing many scenes to test with and for his testing help

## Changelog
- 20231212 - 0.2.0
  - added scene game mode override
  - fixed scene select which is currently in use
  - fixed sending messages multiple times
  - changed how default scene gets handled
- 20231211 - 0.1.0
  - initial test build
  - added CatCore implementation
  - added Camera2 integration
  - added settings
  - added menus
  - added custom scene handling