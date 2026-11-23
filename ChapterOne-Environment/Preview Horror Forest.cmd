@echo off
cd /d "%~dp0"
if not exist "Builds\ChapterOne\THAT NIGHT.exe" (
    echo Build the current Chapter 1 preview in Unity first.
    pause
    exit /b 1
)
start "THAT NIGHT" "Builds\ChapterOne\THAT NIGHT.exe" -forestPreview
