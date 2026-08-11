FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ParkingReservation.Domain/ParkingReservation.Domain.csproj", "ParkingReservation.Domain/"]
COPY ["ParkingReservation.Application/ParkingReservation.Application.csproj", "ParkingReservation.Application/"]
COPY ["ParkingReservation.Infrastructure/ParkingReservation.Infrastructure.csproj", "ParkingReservation.Infrastructure/"]
COPY ["ParkingReservation.Api/ParkingReservation.Api.csproj", "ParkingReservation.Api/"]

RUN dotnet restore "ParkingReservation.Api/ParkingReservation.Api.csproj"

COPY . .

WORKDIR /src/ParkingReservation.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ParkingReservation.Api.dll"]
