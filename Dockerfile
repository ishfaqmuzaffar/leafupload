FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY LeafUpload.Core/LeafUpload.Core.csproj LeafUpload.Core/
COPY LeafUpload.Infrastructure/LeafUpload.Infrastructure.csproj LeafUpload.Infrastructure/
COPY LeafUpload.Web/LeafUpload.Web.csproj LeafUpload.Web/
RUN dotnet restore LeafUpload.Web/LeafUpload.Web.csproj -r linux-x64

COPY LeafUpload.Core/ LeafUpload.Core/
COPY LeafUpload.Infrastructure/ LeafUpload.Infrastructure/
COPY LeafUpload.Web/ LeafUpload.Web/

RUN dotnet publish LeafUpload.Web/LeafUpload.Web.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained false \
    -o /app/publish \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LeafUpload.Web.dll"]
