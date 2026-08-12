FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OsbErpLite.sln ./
COPY OsbErpLite.Domain/OsbErpLite.Domain.csproj OsbErpLite.Domain/
COPY OsbErpLite.Application/OsbErpLite.Application.csproj OsbErpLite.Application/
COPY OsbErpLite.Infrastructure/OsbErpLite.Infrastructure.csproj OsbErpLite.Infrastructure/
COPY OsbErpLite.Api/OsbErpLite.Api.csproj OsbErpLite.Api/
COPY tests/OsbErpLite.Domain.Tests/OsbErpLite.Domain.Tests.csproj tests/OsbErpLite.Domain.Tests/
COPY tests/OsbErpLite.Application.Tests/OsbErpLite.Application.Tests.csproj tests/OsbErpLite.Application.Tests/
RUN dotnet restore

COPY OsbErpLite.Domain/ OsbErpLite.Domain/
COPY OsbErpLite.Application/ OsbErpLite.Application/
COPY OsbErpLite.Infrastructure/ OsbErpLite.Infrastructure/
COPY OsbErpLite.Api/ OsbErpLite.Api/
RUN dotnet publish OsbErpLite.Api/OsbErpLite.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/logs

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OsbErpLite.Api.dll"]
