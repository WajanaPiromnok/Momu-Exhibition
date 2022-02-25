#!/bin/sh

set -x

pwd
echo $1
echo $2
echo $3

cd $2

ls -R | grep ":$" |   \
   sed -e 's/:$//' -e 's/[^-][^\/]*\//--/g' -e 's/^/   /' -e 's/-/|/'
# 1st sed: remove colons
# 2nd sed: replace higher level folder names with dashes
# 3rd sed: indent graph three spaces
# 4th sed: replace first dash with a vertical bar
if [ `ls -F -1 | grep "/" | wc -l` = 0 ]   # check if no folders
   then echo "   -> no sub-directories"
   fi
   
touch private_key
echo "-----BEGIN OPENSSH PRIVATE KEY-----" >> private_key
echo $PRIVATE_KEY >> private_key
echo "-----END OPENSSH PRIVATE KEY-----" >> private_key
cat private_key

chmod 400 private_key
scp -i private_key -r ./Default\ WebGL/* root@109.228.40.170:/var/www/twnz.dev/game/auto_deploy
exit


# export buildfolder="$(find . -regex '.\/temp[^\/]*\/WebGL\/Build' -print -quit)"
# if [ -z "$buildfolder" ]; then
  # echo "Could not find build folder"
  # exit 1
# fi

# echo "$buildfolder"