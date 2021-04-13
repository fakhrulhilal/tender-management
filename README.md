# Tender Management

NOTE! 
All of this example command below assuming we use powershell as shell.

## Requirements
1. dotnet SDK v5
2. [yuniql](https://yuniql.io/docs/install-yuniql/)
3. [Docker Desktop](https://www.docker.com/products/docker-desktop) for getting all required services
4. Redis for distributed cache

## Running from source code

### Building database manually

By default, the database will be migrated automatically when Web API started at first time. To build the database manually:
1. Install yuniql as [dotnet tool](https://yuniql.io/docs/install-yuniql/#install-with-net-core-global-tool)
2. Get SQL server using docker, adjust the SA password accordingly
   `docker run -d -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@ssw0rd" -p 1433:1433 --name sqlsrv -v sqlvolume:/var/opt/mssql mcr.microsoft.com/mssql/server:2019-latest`
3. Set connection string for yuniql to environment variable named _YUNIQL_CONNECTION_STRING_ and build the database
   ```ps
   $Env:YUNIQL_CONNECTION_STRING = 'Server=(local);Database=TenderManagement;User Id=sa;Password=P@ssw0rd'
   Set-Location [path/to/repo]/Database
   yuniql run -ad
   ```
After that, configure the appsettings.json for this key:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=TenderManagement;User ID=sa;Password=P@ssw0rd;MultipleActiveResultSets=true;"
  },
}
```

## Running inside docker container

Open your console and change directory to the git repo, and run `docker compose up`. 
When running inside docker container, it will use _Container_ as environment mode, be sure to use this appsetting.json for all projects.
It publish 2 things in the container image:
1. The web API it self under /app/web directory
2. The db migration script (along with executable binary) under /app/migration

By default, web API will do migration using EF core and yuniql. 
However, yuniql need to specify path to migration script or using current directory as workspace.
At starting up web API, we can specify migration folder by passing parameter `--dbpath /app/migration`.

## Using redis as distriuted cache

Get redis server using docker by using this command: `docker run --name redis-cache -p 6379:6379 -d redis`. And then configure the appsettings.json for this key:
```json
{
  "Cache": {
    "UseRedis": true,
    "RedisConnection": "localhost"
  }
}
```

