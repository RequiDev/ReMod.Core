# ReMod.Core

[![main](https://img.shields.io/github/workflow/status/RequiDev/ReMod.Core/main?style=for-the-badge)](https://github.com/RequiDev/ReMod.Core/actions/workflows/main.yml)
[![All Releases](https://img.shields.io/github/downloads/RequiDev/ReMod.Core/total.svg?style=for-the-badge&logo=appveyor)](https://github.com/RequiDev/ReMod.Core/releases)

![ReMod Core Logo](https://github.com/RequiDev/ReMod.Core/raw/master/remod_core_logo.png)

The core dependency of my VRChat mods **ReMod** and [ReModCE](https://github.com/RequiDev/ReModCE). This project contains several helper functions, wrappers and UI classes for VRChat, Unity and [MelonLoader](https://github.com/LavaGang/MelonLoader).

## Description
This library powers both my private VRChat mod and the public version of it. It's main use is to create UI elements for VRChat in an easy way without all the work behind it.

## Usage
If you don't intend to modify the project your best bet is to either download the latest release from [here](https://github.com/RequiDev/ReMod.Core/releases/latest) or [here](https://github.com/RequiDev/ReModCE/releases/latest). Both this project and ReModCE will have an automatically built version in their releases.  
You'll want to link against that version and just supply it with your mod by either automatically load version from GitHub like [here](https://github.com/RequiDev/ReModCE/blob/master/ReModCE.Loader/ReMod.Loader.cs#L194)

If you **NEED** to modify ReMod.Core and need those changes to be in here, I'd suggest adding this repository to yours as a [git submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules).
Modify ReMod.Core as you need it, test your changes and create a pull request to submit your changes.

Please don't ILMerge ReMod.Core into your project as that can cause all sorts of problems with 2 ReMod.Core libraries existing in 1 process.

## Documentation
I'll be honest, read the code in [ReModCE](https://github.com/RequiDev/ReModCE). I wrote this library to use it for my projects. I'm fine with other people using it as long as they respect the [license](https://github.com/RequiDev/ReMod.Core/blob/master/LICENSE) behind it.  
A few things are a bit convoluted, but that's because I tried my best to OOP the unity hierarchy of the UI objects behind it. It should still be easy enough to understand.

## Contribution
In case you do have something to contribute, please create a pull request. Try to stay close to the current coding style.

## Credits
[Kiokuu](https://github.com/Kiokuu)  
[imxLucid](https://github.com/imxLucid)  
[DDAkebono](https://github.com/DDAkebono)  
