# Motion Jam 2021 at MoMu

This repo uses `master` and `dev/*` as dev branches, `deploy-linux` for server deployment, and `deploy-webgl*` for WebGL client deployment throgh Unity Cloud Build. This doesn't include source code of the website which is responsible for authentication, but the HTML frame for Unity canvas is provided.

## Setups

### Playfab
- On PlayFab dashboard
    - In Add-ons tab, enable Facebook&Google services to allow authentication from access token
    - In Content tab, add a new key named "serverDetails" with value in the same format as in [this JSON](./etc/PlayFab/serverDetails.json). Game client will use this as server connection details.
    - In Automation tab -> Cloud script -> Revisions, replace the code with [this script](./etc/PlayFab/cloudScript.js). It contains some functions for cleanup all stored data and for getting all players' emails. You can trigger them through Scheduled Tasks.

### Web Server
- nginx
    - [nginx.conf](./etc/host/nginx.conf)
        - enable gzip compression
    - [sites-enabled/momu.co](./etc/host/momu.co)
        - enable SSL
        - redirect HTTP->HTTPS
        - set root directory
    - add application/wasm to /etc/nginx/mime.types
- game frame
    - See [web template (after build)](./etc/web/game)

### Multiplayer Server
- ufw
    - allow port 9001-90XX TCP depending on how many multiplayer server instances you will run
- pm2
    - use pm2 to run multiplayer server instances using taskset to assign specific CPU to a server instance.
    - See [pm2 describe output of a server instance](./etc/host/pm2 describe 0.out)

### Unity Cloud Build
- optional and not recommended as there's a problem about lighting and post-processing

## Build Process
### WebGL Client
- TODO
### Multiplayer Server
- TODO
