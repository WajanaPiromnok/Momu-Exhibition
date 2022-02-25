del ..\..\Build\linux_build.zip
"C:\Program Files\7-Zip\7z.exe" a -tzip ..\..\Build\linux_build.zip ..\..\Build\linux_build\*

scp ..\..\Build\linux_build.zip %1@%2:~/unity-server/

setlocal EnableDelayedExpansion

SET myFlags= ^
set -x; ^
pm2 delete unity-server-1; ^
pm2 delete unity-server-2; ^
pm2 delete unity-server-3; ^
pm2 delete unity-server-4; ^
pm2 delete unity-server-5; ^
pm2 delete unity-server-6; ^
pm2 delete unity-server-7; ^
pm2 delete unity-server-8; ^
cd ~/unity-server; ^
rm -r linux_build; ^
unzip -o linux_build.zip -d linux_build; ^
cp cert.* linux_build/; ^
chmod +x linux_build/linux_build.x86_64; ^
cd linux_build; ^
pm2 start taskset --name unity-server-1 -- 0x1 ./linux_build.x86_64 -batchmode -nographics -port 9001 -maxPlayer 80; ^
pm2 start taskset --name unity-server-2 -- 0x2 ./linux_build.x86_64 -batchmode -nographics -port 9002 -maxPlayer 80; ^
pm2 start taskset --name unity-server-3 -- 0x4 ./linux_build.x86_64 -batchmode -nographics -port 9003 -maxPlayer 80; ^
pm2 start taskset --name unity-server-4 -- 0x8 ./linux_build.x86_64 -batchmode -nographics -port 9004 -maxPlayer 80; ^
pm2 start taskset --name unity-server-5 -- 0x10 ./linux_build.x86_64 -batchmode -nographics -port 9005 -maxPlayer 80; ^
pm2 start taskset --name unity-server-6 -- 0x20 ./linux_build.x86_64 -batchmode -nographics -port 9006 -maxPlayer 80; ^
pm2 start taskset --name unity-server-7 -- 0x40 ./linux_build.x86_64 -batchmode -nographics -port 9007 -maxPlayer 80; ^
pm2 start taskset --name unity-server-8 -- 0x80 ./linux_build.x86_64 -batchmode -nographics -port 9008 -maxPlayer 80

ssh %1@%2 %myFlags%

REM exit /s