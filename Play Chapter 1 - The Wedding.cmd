@echo off
cd /d "%~dp0"
if exist "ChapterOne-Environment\Builds\ChapterOne\THAT NIGHT.exe" (
    start "THAT NIGHT - Chapter 1: The Wedding" "ChapterOne-Environment\Builds\ChapterOne\THAT NIGHT.exe"
) else (
    echo Build not found at ChapterOne-Environment\Builds\ChapterOne\THAT NIGHT.exe
    pause
)
