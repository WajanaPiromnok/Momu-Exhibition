$login_user = $args[0]
$login_url = $args[1]
$multiplayer_url = $args[2]
$upload_path = $args[3]

$destination = $login_user + "@" + $login_url + ":" + $upload_path
# & $PSScriptRoot\switch_multiplayer_server.ps1 $multiplayer_url
& $PSScriptRoot\rename_html.ps1
Write-Host "Begin uploading ..."

scp -r "../../Build/webgl-build/Build/*" $destination