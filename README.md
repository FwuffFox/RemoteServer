<h1>RemoteServer </h1>

## What is this?

A backend for a personal project, goal of which was to create a basic messaging application. You can find WebUi [here](https://github.com/FwuffFox/RemoteWeb/)


### How to run
[Open](RemoteMessenger/Server/appsettings.json) and change for your own needs
* `InMemory`: false if using PostgreSQL, else true.
* `DataBase`: Postgres connection string. Not needed if `InMemory` is true
* `JWT`.
       - `Secret`: Server secret. Needs to be long.
       - `Issuer`: URL of server. ex: "https://localhost:5001/"
       - `Audience`: Url of WebUI. ex: "https://localhost:5000/"
       - `ExpireDays`: Expire days for JWT token. ex: 14
   
3. `dotnet run`
