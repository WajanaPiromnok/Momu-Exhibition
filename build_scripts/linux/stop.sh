screen -S mirrorServer -X stuff $'\003'
killall screen
pm2 stop unity-server