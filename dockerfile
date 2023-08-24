FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out -r linux-x64

FROm mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from-build-env /App/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Booking.Command.dll"]