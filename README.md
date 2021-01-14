# TrophyWizard
Just a nice dll that allow to unlock PS3 and Vita trophies

#Important info in compilation
TrophyParser was made thinking as a dll for multiplatform, this means using it with Blazor or any other, this is why it use netstandard2.1. Because this is also has a wrapper for darkautism TrophyParser, it use **BigEndianTool so you must modify it for use the netstandard2.1 sdk**