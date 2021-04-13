#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80/tcp
EXPOSE 443/tcp

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /tests
COPY tests/Application.IntegrationTests/*.csproj Application.IntegrationTests/
COPY tests/Application.UnitTests/*.csproj Application.UnitTests/
COPY tests/ .
WORKDIR /src
COPY src/*.sln .
COPY src/WebApi/WebApi.csproj WebApi/
COPY src/Application/Application.csproj Application/
COPY src/Domain/Domain.csproj Domain/
COPY src/Imprise.MediatR.Extensions.Caching/src/Imprise.MediatR.Extensions.Caching/Imprise.MediatR.Extensions.Caching.csproj Imprise.MediatR.Extensions.Caching/src/Imprise.MediatR.Extensions.Caching/
COPY src/Database/Database.csproj Database/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/
COPY src/ .
RUN dotnet restore
RUN dotnet build -c Release

FROM build AS db
WORKDIR /src/Database
RUN dotnet publish Database.csproj -c Release -o /app/db

FROM build AS test
RUN dotnet tool install dotnet-reportgenerator-globaltool --tool-path /dotnetglobaltools
LABEL testlayer=true
WORKDIR /tests/Application.UnitTests
RUN dotnet test --logger "trx;LogFileName=Application.UnitTests.result.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=/out/testresults/coverage/ /p:Exclude="[xunit.*]*" --results-directory /out/testresults
RUN /dotnetglobaltools/reportgenerator "-reports:/out/testresults/coverage/coverage.cobertura.xml" "-targetdir:/out/testresults/coverage/reports" "-reporttypes:HTMLInline;HTMLChart"
WORKDIR /tests/Application.IntegrationTests
RUN dotnet test --logger "trx;LogFileName=Application.IntegrationTests.result.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=/out/testresults/coverage/ /p:Exclude="[xunit.*]*" --results-directory /out/testresults
RUN /dotnetglobaltools/reportgenerator "-reports:/out/testresults/coverage/coverage.cobertura.xml" "-targetdir:/out/testresults/coverage/reports" "-reporttypes:HTMLInline;HTMLChart"

FROM build AS publish
WORKDIR /src/WebApi
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish web
COPY --from=db /app/db migration
WORKDIR /app/web
ENV ASPNETCORE_URLS http://*:80,https://*:443
HEALTHCHECK --interval=30s --timeout=3s --retries=1 CMD curl --silent --fail http://localhost:80/health || exit 1
ENTRYPOINT ["dotnet", "TenderManagement.WebApi.dll", "--dbpath", "/app/migration"]