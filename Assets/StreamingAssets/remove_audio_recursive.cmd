@echo off
mkdir soundless
for /r %%a in (*.mp4) do ffmpeg -i "%%a" -vcodec copy -an -n "Soundless\%%~na".mp4
pause