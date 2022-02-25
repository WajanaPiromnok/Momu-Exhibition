mv ~/upload_landing/linux_build.zip ./
rm -r linux_build
unzip -o linux_build.zip -d linux_build
cp cert.* linux_build/
cd linux_build/
chmod +x linux_build.x86_64
# screen -d -m -S mirrorServer ./linux_build.x86_64 -batchmode -nographics -logfile ~/server.log

pm2 stop unity-server
pm2 start ./linux_build.x86_64 --name "unity-server" -- -batchmode -nographics