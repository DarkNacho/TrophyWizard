# TrophyWizard
Hello, this is a Two-in-One trophy stamper for PlayStation 3 and PlayStation Vita!

## How to use
In order to load Vita trophy folders, you will need a decrypted folder using Vitashell on your console. Transfer a Vita folder to your ur0:user/trophy/ directory on the Vita, then on there highlight the NPWR folder that you're wanting to use, then press your start button, and choose the option 'Open decrypted files', select the title and trans files, and copy them out of the data folder. 

You will want to transfer these two files via FTP to your computer. Make sure they are in an isolated folder on your computer, and then take the three files present in the 'conf' directory: TROP.SFM, TRPPARAM.INI, and the ICON0.PNG (and GRXX.PNG(s)) and copy them to that same folder. Now, open up Trophy Wizard, change the console selection to Vita, then drag the prepared folder into the program. You will then be able to edit your stamps, lock trophies, and clone other users' times (using PSNTrophyLeaders). Once you are done, make sure to save.

Then transfer the title/trans back to your Vita, and from there, copy the two files and again, open the data folder with the 'Decrypted Files' option, and replace the title/trans there. Now make sure to delete the db directory as well trop_sys directory, and then proceeed to load the trophy app. Just to make sure the game loads, and times are correct. Once all is done, you can sync. Have fun!

## Compilation
This is mainly a TrophyParser dll, build for netstandard2.1. This contains a wrapper for darkautism [TROPHYParser](https://github.com/darkautism/TROPHYParser) that uses [BigEndianTool](https://github.com/darkautism/BigEndianTool) so you must modify the project information to use **netstandard2.1**


## Contributor
Thanks to [darkautism](https://github.com/darkautism/PS3TrophyIsGood) this project use his [TROPHYParser](https://github.com/darkautism/TROPHYParser) Implementation for PS3 and I did also make a few modifications to his [PS3TrophyIsGood](https://github.com/darkautism/PS3TrophyIsGood) this is only just for have a nice GUI if somebody wants to use it.

## How to use the dll
``` Csharp
 IUnlocker unlocker = new PS3Unlocker(path)
 //IUnlocker unlocker = new VitaUnlocker(path) //This would be for PSVita
 foreach (var trophy in unlocker) //Iterate through each trophy
    unlocker.UnlockTrophy(trophy.Id, DateTime.Now); //unlock it with the current date
```
