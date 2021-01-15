# TrophyWizard
Just a tool that allow modify PS3 and Vita trophy folders

# Important info in compilation
TrophyParser was made thinking as a dll for multiplatform, this means using it with Blazor or any other, this is why it use netstandard2.1. Because this is also has a wrapper for darkautism TrophyParser, it use **BigEndianTool so you must modify it for use the netstandard2.1 sdk**

# How to use
**For the moment this do not decrypt folders, this means you must decrypt it and encrypt it**
For select the console there is a comboBox in the top, by default is PS3
For Vita trophies, every files must be just **in one folder**

## Contributor
Thanks to [darkautism](https://github.com/darkautism/PS3TrophyIsGood) this project use his TrophyParser for PS3 and also his GUI
