rm ..\..\Build\linux_build.zip
"C:\Program Files\7-Zip\7z.exe" a -tzip ..\..\Build\linux_build.zip ..\..\Build\linux_build\*

scp ..\..\Build\linux_build.zip root@109.228.40.170:~/upload_landing

REM exit /s