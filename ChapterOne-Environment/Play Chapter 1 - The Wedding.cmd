@echo off
cd /d "%~dp0"
if exist "Builds\ChapterOne\THAT NIGHT.exe" (
    start "THAT NIGHT - Chapter 1: The Wedding" "Builds\ChapterOne\THAT NIGHT.exe"
) else (
    echo Build not found at Builds\ChapterOne\THAT NIGHT.exe
    pause
)
