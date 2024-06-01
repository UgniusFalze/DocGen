FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#####################
#PUPPETEER RECIPE
#####################
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list
RUN apt-get update && apt-get install -y google-chrome-stable --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf
######################
#END PUPPETEER RECIPE
######################
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"
USER $APP_UID
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["DocsManager/DocsManager.csproj", "DocsManager/"]
RUN dotnet restore "DocsManager/DocsManager.csproj"
COPY . .
WORKDIR "/src/DocsManager"
RUN dotnet build "DocsManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "DocsManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocsManager.dll"]
