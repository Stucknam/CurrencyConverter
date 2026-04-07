# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY CurrencyConverter/CurrencyConverter.csproj CurrencyConverter/
RUN dotnet restore CurrencyConverter/CurrencyConverter.csproj

# Копируем всё остальное
COPY . .

# Публикуем приложение
RUN dotnet publish CurrencyConverter/CurrencyConverter.csproj -c Release -o /app/publish


# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=build /app/publish .

# Запуск приложения
ENTRYPOINT ["dotnet", "CurrencyConverter.dll"]