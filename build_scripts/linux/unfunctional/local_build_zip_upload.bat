rmdir /s /q ..\..\Build\linux_build
mkdir ..\..\Build\linux_build

D:\Programs\2020.3.13f1\Editor\Unity.exe -batchMode -nographics -projectPath "C:\Users\Motterja\Documents\Repos\experiment-multiplayer-and-cloud-build" -buildLinux64Player %cd%\..\..\Build\linux_build\linux_build.x86_64 -quit

rm ..\..\Build\linux_build.zip
"C:\Program Files\7-Zip\7z.exe" a -tzip ..\..\Build\linux_build.zip ..\..\Build\linux_build\*

scp ..\..\Build\linux_build.zip root@109.228.40.170:~/upload_landing
# ssh root@twnz.dev "cd ~/unity-multiplayer-playground/; ./deploy.sh"
REM exit /s