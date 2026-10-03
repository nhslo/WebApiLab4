# Лабораторная работа — Модуль 04: Product API с EF Core

**CSE5032 · Разработка веб-сервисов · Модуль 04**

Задание — построить API товаров с БД и разделением ответственности. EF Core сопоставляет классы с SQLite, миграция создаёт схему. Контроллер использует Repository, а не `DbContext` напрямую. Для клиента подготовлен DTO без Category, преобразованием занимается AutoMapper. Формат тела ответа унифицирован через `ReturnResult`, но HTTP-коды остаются. Покажу CRUD и объясню цепочку от контроллера через Repository и EF Core до базы.

---

## Цель

Построить Web API товаров с EF Core и Code First, изолировать работу с данными через Repository и возвращать клиенту DTO в едином контракте.

## Краткий отчёт

Для хранения используется SQLite (`products.db`). Миграция применяется при запуске. `ProductsController` вызывает `IProductRepository`, а не `AppDbContext`; AutoMapper преобразует сущности и входные данные. В ответах используется `ReturnResult<T>` с полями `isSuccess`, `result`, `errorMessage` и сохраняются соответствующие HTTP status codes.

## Выполнение по шагам

| Шаг по заданию | Реализация | Для чего |
|---|---|---|
| 1. Модель и конфигурация | `Product`; строка SQLite находится в `appsettings.json` | Описать товар и не хранить настройки БД в коде контроллера |
| 2. Контекст | `AppDbContext : DbContext` с `DbSet<Product>` | Работать с таблицей товаров и отслеживать изменения |
| 3. Code First | Создана EF Core миграция; `Database.Migrate()` применяется при старте | Создать или обновить схему базы по моделям |
| 4. Repository | Созданы `IProductRepository` и `ProductRepository`; зарегистрирован через DI | Скрыть детали запросов EF Core от контроллера |
| 5. DTO | `ProductDto` содержит `Id`, `Name`, `Price`; поля `Category` в DTO ответа нет | Ограничить публичные данные ответа |
| 6. AutoMapper | Настроены `Product ↔ ProductDto` и преобразование входной модели | Не копировать свойства вручную в контроллере |
| 7. ReturnResult | Успешные и ошибочные ответы используют общий класс | Сохранять одинаковую структуру тела вместе с HTTP-кодом |
| 8. CRUD | Реализованы GET списка/по Id, POST, PUT и DELETE | Управлять товарами через API |
| 9. Проверка | Swagger: 400, создание, чтение, обновление, удаление и повторный GET | Проверить контракт и HTTP-статусы |

## Модель `Product` и ответ DTO

| Поле сущности | Тип | Публичное в `ProductDto` |
|---|---|---|
| `Id` | `int` | Да |
| `Name` | `string` | Да |
| `Price` | `decimal` | Да |
| `Category` | `string` | Нет — намеренно исключено из DTO |

## Endpoint

| Метод | Endpoint | Успешный ответ | Основная ошибка |
|---|---|---|---|
| `GET` | `/api/products` | `200 OK` — список DTO | — |
| `GET` | `/api/products/{id}` | `200 OK` — один DTO | `404 Not Found` |
| `POST` | `/api/products` | `201 Created` — созданный DTO | `400 Bad Request` |
| `PUT` | `/api/products/{id}` | `200 OK` — обновлённый DTO | `400` / `404` |
| `DELETE` | `/api/products/{id}` | `200 OK` — результат удаления | `404 Not Found` |

## Пример ответа JSON

```json
{
  "isSuccess": true,
  "result": {
    "id": 1,
    "name": "Ноутбук",
    "price": 350000
  },
  "errorMessage": []
}
```

## Проверка и скриншоты

Каждая иллюстрация подтверждает отдельный запрос или запуск; изображения сохранены в `docs`.

### Шаг 1. Swagger: доступные операции

![Список endpoint Products API](docs/swagger-endpoints.png)

### Шаг 2. Список товаров — `GET /api/products`

![GET списка — 200 OK](docs/get-all-200.png)

### Шаг 3. Проверка валидации — `POST /api/products` → 400

![Пустые поля: Bad Request и ошибки в ReturnResult](docs/post-validation-400.png)

### Шаг 4. Создание — `POST /api/products` → 201

![Созданный товар; Category отсутствует в DTO](docs/post-create-201.png)

### Шаг 5. Чтение по Id — `GET /api/products/{id}`

![Результат содержит Id, Name и Price без Category](docs/get-by-id-200.png)

### Шаг 6. Обновление — `PUT /api/products/{id}`

![Товар изменён — 200 OK](docs/put-update-200.png)

### Шаг 7. Удаление — `DELETE /api/products/{id}`

![Удаление товара — успешный результат](docs/delete-200.png)

### Шаг 8. Проверка после удаления

![Повторный GET: 404 и isSuccess false](docs/get-after-delete-404.png)

### Журнал EF Core

![PowerShell с SQL-командами EF Core](docs/powershell-logs.png)

## Вывод

Реализована требуемая цепочка `Controller → Repository → EF Core → SQLite` и `Entity → AutoMapper → DTO → ReturnResult`. CRUD проверен в Swagger.
