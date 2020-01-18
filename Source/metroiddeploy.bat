set outputdir="Z:\SteamLibrary\steamapps\common\RimWorld\Mods\ToolkitCustomManhunter"

rm -r %outputdir%
mkdir %outputdir%

xcopy /E /Y "..\About" %outputdir%\About\
xcopy /E /Y "..\Assemblies" %outputdir%\Assemblies\
xcopy /E /Y "..\Languages" %outputdir%\Languages\
xcopy /E /Y "..\Defs" %outputdir%\Defs\