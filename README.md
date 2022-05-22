# MessageSenderService
### Приложение для эмуляции отправки уведомлений NotificationSender.

## Описание
API приложения предоставляет:

- Метод для создания и одновременной отправки уведомления,
 который сохраняет его в памяти или в персистентном хранилище (в зависимости от настроек). 
 Возвращает идентификатор уведомления и статус его эмулируемой отправки. 
 
- Метод для получения статуса отправки уведомления (Доставлено/Не доставлено) по его идентификатору.

###
##### На данный момент, уведомления могут быть двух видов:

1. Для iOS-устройств.
2. Для Android-устройств.
###
 
###### Для iOS устройств:

- PushToken - строка до 50 символов. Уникальный идентификатор девайса, куда будет отправлено уведомление. Поле обязательное.
- Alert - строка до 2000 символов. Само сообщение. Поле обязательное.
- Priority - целое число. По умолчанию должно принимать значение 10.
- IsBackground - булево значение. По умолчанию должно принимать значение true.

###### Для Android устройств:

- DeviceToken - строка до 50 символов. Уникальный идентификатор девайса, куда будет отправлено уведомление. Поле обязательное.
- Message - строка до 2000 символов. Само сообщение. Поле обязательное.
- Title - строка до 255 символов. Поле обязательное.
- Condition - строка до 2000 символов. Поле является опциональным.

А также


- В зависимости от типа уведомления выбирается их отправитель NotificationSender.
- Как часть эмуляции “выполнения отправки” производится задержка от 500мс до 2 секунд.
- В лог выводятся сообщения о названии отправителя (NotificationSender) и само уведомление.
- В рамках каждого NotificationSender: каждая пятая отправка завершается неуспешно (статус уведомления будет “Не доставлено“). 

- В случае успешной отправки статус уведомления будет “Доставлено“.


## Принятые решения и допущения

Использование REST API

#### Формат запросов и ответов:

###### + Создание нотификации для iOS устройств:

    Request:

    POST /api/v1/notifications   

    {
        "TargetType": "iOS",
        "Parameters": {
            "Alert": "AlertS",
            "IsBackground": "true",
            "Priority": "110",
            "PushToken": "PushTokenS"
        }
    }

    Response:

    {
        "id": "512dea8a-3a79-45c5-a70b-625c1561c61e",
        "status": "Доставлено"
    }
    


###### + Создание нотификации для Android устройств:
    
    Request:

    POST /api/v1/notifications   

    {
        "TargetType": "Android",
        "Parameters": {
            "DeviceToken": "DeviceToken String",
            "Message": "Message Text",
            "Title": "Title Text",
            "Condition": "Condition Text"
        }
    }

    Response:

    {
        "id": "512dea8a-3a79-45c5-a70b-625c1561c61e",
        "status": "Доставлено"
    }

###### + Получение статуса нотификации:
    
    Request:

    GET /api/v1/notifications/:id/status

    Response:

    {
        "status": "Доставлено"
    }

#### Инструменты:

Платформа .NET 6
Библиотеки логирования - Serilog
Библиотеки тестирование - XUnit, Moq, FluentAssertions
ORM - EF


В качестве хранилища данных "InMemoryDatabase" / "Postgres",
за переключения между которыми отвечают флаги в конфигах:

для "InMemoryDatabase"

    UseInMemoryDatabase: true,
    RunMigrationsIfNeeds: false,

для "Postgres"

    UseInMemoryDatabase: false,
    RunMigrationsIfNeeds: true/false,

##
#### Структура решения:

##

##### MessageSenderServiceApi
    отвечает за запуск приложения и API, описание прослушиваемых адресов.

##### MessageSenderServiceApi.Contracts
    описание сообщений для взаимодействия API.

##### MessageSenderServiceApi.Domain
    модули доменной логики в разрезах сущностей.

##### MessageSenderServiceApi.Infrastructure
    имплементация репозиториев, взаимодествия с хранилищами данных.

##### MessageSenderServiceApi.Packages
    общие пакеты.
##### [NotificationSenders].NotificationSender
    реализует механизм подбора необходимого обработчика для полученного типа уведомления
##### [NotificationSenders].NotificationSender.AndroidProvider
    обработчик отправки уведомлений приложению на Android

##### [NotificationSenders].NotificationSender.IosProvider
    обработчик отправки уведомлений приложению на iOS
    

Используя указанное решение можно по аналогии добавить,
 например, обработчик почтовых отправок, 
 или указать конкретную версию устройства.


##### MessageSenderServiceApi.Tests
    Unit-тесты (пример)
##### MessageSenderServiceApi.Tests.Integration
    Интеграционные-тесты (пример)

##
#### Запуск приложения:

##

Для запуска приложения в контейнере 
необходим Docker-Desktop (docker-compose).

При наличии которых, можно выполнить скрипт [build.sh](/build.sh) из корневой дирректории; он выполняет
- сборку контейнера,
- останановку текущей конфигурации,
- удаление сети, если она существует,
- создание сети,
- запуск конфигурации, приложения в связке с контейнером Postgres.

Постучаться в API можно с помощью [postman коллекции](/MessageSenderServiceApi.postman_collection.json).

## Комментарии к допущениям
- Структура солюшена избыточна для поставленной задачи, это для примера, кроме того, намерен доработать это приложение для обработки сообщений из очереди RabbitMQ не только нотификаций, но и отправки ответов в чат интеракциях через Телеграм и Почту.
- Также в сущности БД поле хэша сообщения тоже лишнее, и индекс на нем (относительно поставленной задачи он вреден, замедляет запись), это понадобится для проверки на повторную обработку сообщения, когда запись в БД будет выполняться в паралельных тасках (что даст возможностью отвечать на внешний запрос быстрее).





