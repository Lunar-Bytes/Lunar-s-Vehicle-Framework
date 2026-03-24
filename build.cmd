@echo off
REM Build script for Lunar's Vehicle Framework
REM Usage: build.cmd [Release|Debug]

SET "SUBNAUTICA_DIR=L:\SteamLibrary\steamapps\common\Subnautica\BepInEx\plugins\"
SET "GAMEPLAY_LIB_DIR=D:\SubnauticaMods\Gameplay\LunarsNewVehicles\Libs"

setlocal enabledelayedexpansion

REM Set the configuration (Release or Debug)
set CONFIGURATION=%1
if "!CONFIGURATION!"=="" (
    set CONFIGURATION=Release
)

REM Set source and destination based on configuration
if /i "!CONFIGURATION!"=="Debug" (
    set "SOURCE=D:\SubnauticaMods\Libs\Lunar's Vehicle Framework\LunarsVehicleFramework\bin\Debug\net472\LunarsVehicleFramework.dll"
    set "DEST=%GAMEPLAY_LIB_DIR%"
) else (
    set "SOURCE=D:\SubnauticaMods\Libs\Lunar's Vehicle Framework\LunarsVehicleFramework\bin\Release\net472\LunarsVehicleFramework.dll"
    set "DEST=%SUBNAUTICA_DIR%"
)

echo Source: !SOURCE!
echo Destination: !DEST!
echo.

REM Get the script directory
set SCRIPT_DIR=%~dp0
cd /d "!SCRIPT_DIR!"

echo.
echo ====================================
echo Lunar's Vehicle Framework Build
echo ====================================
echo Configuration: !CONFIGURATION!
echo.

REM Try to find .NET SDK
where dotnet >nul 2>&1
if errorlevel 1 (
    echo ERROR: dotnet CLI not found. Please install .NET SDK.
    echo Download from: https://dotnet.microsoft.com/download
    exit /b 1
)

REM Check if solution file exists
if not exist "LunarsVehicleFramework.sln" (
    echo ERROR: LunarsVehicleFramework.sln not found!
    exit /b 1
)

echo Restoring NuGet packages...
dotnet restore "LunarsVehicleFramework.sln"
if errorlevel 1 (
    echo ERROR: Package restoration failed!
    exit /b 1
)

echo.
echo Building project (Configuration: !CONFIGURATION!)...
dotnet build "LunarsVehicleFramework.sln" -c !CONFIGURATION! --no-restore
if errorlevel 1 (
    echo ERROR: Build failed!
    exit /b 1
)

echo.
echo Output location:
if /i "!CONFIGURATION!"=="Debug" (
    echo   Debug:   !DEST! (LunarsNewVehicles gameplay mod)
) else (
    echo   Release: !DEST! (Subnautica plugins)
)
echo.
echo The compiled DLL is ready to be used as a BepInEx plugin.
echo.
echo ==========================================================
if /i "!CONFIGURATION!"=="Debug" (
    echo Want to copy DEBUG DLL to LunarsNewVehicles?
) else (
    echo Want to copy RELEASE DLL to Subnautica plugins?
)
echo ==========================================================

set /p choice=Do you want to copy the file? (y/n): 

if /i "!choice!"=="y" (
    if not exist "!SOURCE!" (
        echo File not found: !SOURCE!
        exit /b 1
    ) else (
        if not exist "!DEST!" (
            echo Creating directory: !DEST!
            mkdir "!DEST!"
        )
        copy "!SOURCE!" "!DEST!\LunarsVehicleFramework.dll"
        if errorlevel 1 (
            echo ERROR: Failed to copy file!
            exit /b 1
        )
        echo File copied successfully to: !DEST!
    )
) else (
    echo Operation cancelled.
)

echo ==========================================================
endlocal
exit /b 0
