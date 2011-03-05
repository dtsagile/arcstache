TITLE DTSAgile:  Unregister ArcStache SOE with ArcGIS Server
echo off
REM: -------------------------------------------
REM: Unregisters ArcStache SOE with ArcGIS Server.
REM: NOTE: Not necessary if you are simply redeploying the SOE
REM: -------------------------------------------

@echo ..
@echo *************************
@echo Unregistering ArcStache SOE with ArcGIS Server...
@echo *************************
@echo ..
RegisterArcStacheSOE.exe /unregister

pause