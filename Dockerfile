# BUILD STAGE
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj dulu (lebih cepat jika dependency berubah)
COPY *.csproj ./
RUN dotnet restore

# Copy semua source code
COPY . ./

# Publish ke /app dengan Release mode
RUN dotnet publish -c Release -o /app


# RUNTIME STAGE
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build /app .

# Railway pakai port 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

# Ganti BudgetTracker.dll sesuai nama project lu
ENTRYPOINT ["dotnet", "BudgetTracker.dll"]
