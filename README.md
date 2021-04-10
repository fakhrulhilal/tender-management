# Tender Management

NOTE! 
All of this example command below assuming we use powershell as shell.

## Requirements
1. dotnet SDK v5
2. [yuniql](https://yuniql.io/docs/install-yuniql/)
3. [Docker Desktop](https://www.docker.com/products/docker-desktop) for getting all required services

## Building from source code
### Building database
1. Install yuniql as [dotnet tool](https://yuniql.io/docs/install-yuniql/#install-with-net-core-global-tool)
2. Get SQL server using docker, adjust the SA password accordingly
   `docker run -d -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@ssw0rd" -p 1433:1433 --name sqlsrv -v sqlvolume:/var/opt/mssql mcr.microsoft.com/mssql/server:2019-latest`
3. Set connection string for yuniql to environment variable named _YUNIQL_CONNECTION_STRING_ and build the database
   ```ps
   $Env:YUNIQL_CONNECTION_STRING = 'Server=(local);Database=TenderManagement;User Id=sa;Password=P@ssw0rd'
   Set-Location [path/to/repo]/Database
   yuniql run -ad
   ```
