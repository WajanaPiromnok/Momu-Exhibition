$pre_str = 'var REMOTE_HOST_URL = "'
$post_str = '";'
$url_str = $args[0]
$replaced_str = $pre_str + $url_str + $post_str

((Get-Content -path "../../Build/webgl-build/index.html" -Raw) -replace 'var REMOTE_HOST_URL = ".*";', $replaced_str) | Set-Content -Path "../../Build/webgl_build/index.html"
Write-Host "switch multiplayer target url to $url_str"