# Tender Management

NOTE! 
All of this example command below assuming we use powershell as shell.

## Requirements
1. dotnet SDK v5
2. [yuniql](https://yuniql.io/docs/install-yuniql/)
3. [Docker Desktop](https://www.docker.com/products/docker-desktop) for getting all required services

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
