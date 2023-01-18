<h1 style="color: orange; text-align: center">RemoteServer </h1>

<h2 style="color: orange; text-align: center">Туториал по установке.</h2>

1. ### Реквизиты ###

   - [.Net 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
   - [(Опционально)  PostgreSQL](https://www.postgresql.org/)

2. ### Подготовка конфигурации ###
   * [Заходим в файл конфигурации](RemoteMessenger/Server/appsettings.json) и меняем его под свои нужды.
        * InMemory: false если используем PostgreSQL, иначе true.
        * DataBase: Строка подключения к PostgreSQL если она используется.
        * JWT:
          * Secret: Секрет сервера. Заменить на случайную и длинную строку.
          * Issuer: База адреса сервера. ex: "https://localhost:5001/"
          * Audience: База адреса клиента. ex: "https://localhost:5000/"
          * ExpireDays: Время жизни токена аутентификации в днях. ex: 14 
3. ### Создание базы данных ###
    #### В первый запуск ####
    Установка EF Core
   ```shell
    dotnet tool install --global dotnet-ef.
    ```
   #### При каждом обновлении ####
   1. Миграция базы данных
   ```shell
    dotnet efcore migrations add <ИмяМиграцииЗдесь>
   ```
   2. Обновление базы данных
   ```shell
    dotnet efcore database update
    ```
4. ### Запуск сервера. ###
    ```shell
   dotnet run
   ```