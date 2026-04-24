# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /workspace

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

CMD ["dotnet", "watch", "--project", "backend/Api/Api.csproj", "run", "--urls", "http://+:8080"]
