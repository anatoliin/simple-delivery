# Simple Delivery App

Простое веб-приложение для оформления доставки заказов.

## Требования

- .NET SDK 9+
- Node.js 18+
- npm

## Запуск backend

1. Откройте терминал в папке `backend`:
   ```bash
   dotnet restore
   dotnet run
   ```
2. API будет доступно по адресу:
   - http://localhost:5080

## Запуск frontend

1. Откройте терминал в папке `frontend`:
   ```bash
   npm install
   npm run dev
   ```
2. Frontend будет доступен по адресу:
   - http://localhost:5173

## Запуск тестов

Из корня решения:

```bash
dotnet test backend.Tests/backend.Tests.csproj --nologo
```

## Структура проекта

- `backend/` — ASP.NET Core API
- `frontend/` — React + Vite приложение
- `backend.Tests/` — xUnit тесты
