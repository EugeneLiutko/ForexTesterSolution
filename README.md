# Інструкція з запуску проєкту

## Крок 1. Створення змінних оточення

Розмістіть файл `.env` у **кореневій папці** проєкту.

## Крок 2. Запуск сервісів

У кореневій папці виконайте команду:

```bash
docker-compose up --build
```

## Доступ до мікросервісів

🧩 Мікросервіс 1: Subscriptions + Users
📍 Swagger UI: http://localhost:5001/swagger/index.html

🧩 Мікросервіс 2: Projects + UserSettings
📍 Swagger UI: http://localhost:5002/swagger/index.html


