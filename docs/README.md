# 🌸 NewFlowersShop — интернет-магазин цветов

---

## 🚀 О проекте

**NewFlowersShop** — это интернет-магазин, построенный на ASP.NET Core MVC с использованием **Entity Framework Core** и **SQL Server**, позволяющий:
- 🛒 Покупателям — просматривать каталог, оформлять заказы, оставлять отзывы
- 👤 Вести личный кабинет с историей заказов и редактированием данных
- 💼 Сотрудникам — управлять товарами, складом, заказами и кассой
- 📄 Загружать и просматривать документы магазина
- 🎨 Настраивать главную страницу (баннеры, тексты, кнопка)

---

## 🖼 Скриншоты

<details>
<summary><b>🏠 Главная страница</b></summary>

<img src="docs/firstPage.PNG" alt="Главная страница" width="900">
<img src="docs/firstPage2.PNG" alt="Главная страница 2" width="900">

</details>

<details>
<summary><b>🔐 Авторизация</b></summary>

<img src="docs/auth.PNG" alt="Авторизация" width="700">

</details>

<details>
<summary><b>💐 Каталог товаров</b></summary>

<img src="docs/catalog.PNG" alt="Каталог" width="900">

</details>

<details>
<summary><b>🌸 Выбор фона главной страницы и добавление товара</b></summary>

<img src="docs/choosFon.PNG" alt="Выбор фона" width="900">
<img src="docs/addTovar.PNG" alt="Добавление товара" width="900">

</details>

<details>
<summary><b>🛒 Корзина и оформление заказа</b></summary>

<img src="docs/myBucket.PNG" alt="Корзина" width="900">
<img src="docs/offormZakaza.PNG" alt="Оформление заказа" width="900">

</details>

<details>
<summary><b>📦 Мои заказы и история</b></summary>

<img src="docs/myZakaz.PNG" alt="Мои заказы" width="900">
<img src="docs/history.PNG" alt="История заказов" width="900">

</details>

<details>
<summary><b>⭐ Отзывы</b></summary>

<img src="docs/otzyv.PNG" alt="Отзывы" width="900">
<img src="docs/ypravOtz.PNG" alt="Управление отзывами" width="900">

</details>

<details>
<summary><b>👤 Личный кабинет</b></summary>

<img src="docs/myData.PNG" alt="Мои данные" width="700">

</details>

<details>
<summary><b>📊 Панель сотрудника</b></summary>

<img src="docs/balance.PNG" alt="Баланс и касса" width="900">
<img src="docs/tovars.PNG" alt="Товары" width="900">
<img src="docs/ypravTovar.PNG" alt="Управление товарами" width="900">

</details>

<details>
<summary><b>📄 Документы и сотрудники</b></summary>

<img src="docs/sendDoc.PNG" alt="Отправка документов" width="900">
<img src="docs/editEmpl.PNG" alt="Редактирование сотрудника" width="900">

</details>

---

## 🛠 Технологии

<details>
<summary><b>Стек</b></summary>

| Слой | Технология |
|------|------------|
| Платформа | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core |
| БД | SQL Server |
| Аутентификация | Сессии + SHA-256 |
| Фронтенд | Razor Views, Bootstrap, JavaScript |
| Хранение файлов | BLOB в БД (Base64) |

</details>

---

## 🏗 Архитектура

<details>
<summary><b>О структуре</b></summary>

Приложение построено по MVC-архитектуре ASP.NET Core:

- **Controllers** — обработка запросов (HomeController)
- **Models** — сущности EF Core (18 таблиц)
- **Views** — Razor-страницы (.cshtml)

</details>

---

## 👥 Роли и возможности

<details>
<summary><b>Покупатель</b></summary>

- Просмотр каталога с фильтрацией
- Корзина и оформление заказа (курьер/самовывоз)
- История заказов и детализация
- Оставление отзывов (после покупки)
- Личный кабинет и редактирование данных

</details>

<details>
<summary><b>Сотрудник магазина</b></summary>

- Управление товарами (добавление, списание, запрос на пополнение)
- Управление складом и остатками по магазинам
- Обработка заказов (сборка, передача курьеру, выдача)
- Касса и баланс (приход / расход)
- Загрузка и просмотр документов
- Управление сотрудниками и ролями
- Настройка главной страницы

</details>

---

## ⚙️ Установка

<details>
<summary><b>Требования</b></summary>

- Windows 10/11
- Visual Studio 2022
- .NET 8 SDK
- SQL Server

</details>

---
