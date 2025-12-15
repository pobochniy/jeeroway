# Используется в git hub actions, сборка там, сюда скидываются готовые файлы

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR publishfolder
COPY . .

ENTRYPOINT [ "dotnet", "JeerowayWiki.Images.dll" ]