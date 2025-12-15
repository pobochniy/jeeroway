# HTTPS/SSL Setup Guide для Jeeroway

## Предварительные требования

1. ✅ Домен настроен и указывает на IP вашей виртуальной машины
2. ✅ Порты 80 и 443 открыты в файрволе
3. ✅ Docker и Docker Compose установлены
4. ✅ Сеть `nginx-proxy-man` создана: `docker network create nginx-proxy-man`

## Шаг 1: Обновите конфигурацию

### 1.1 Обновите nginx.conf

Замените `your-domain.com` на ваш реальный домен в файле `IaC/nginx/conf/nginx.conf`:

```bash
sed -i 's/your-domain.com/example.com/g' nginx/conf/nginx.conf
```

Или вручную отредактируйте строки:
- `server_name your-domain.com www.your-domain.com;`

### 1.2 Проверьте docker-compose.yml

Убедитесь, что в `IaC/docker-compose.yml` указаны правильные пароли для MySQL:
- `MYSQL_ROOT_PASSWORD`
- `MYSQL_PASSWORD`

## Шаг 2: Получите SSL сертификат

### 2.1 Сделайте скрипт исполняемым

```bash
cd IaC
chmod +x init-letsencrypt.sh
chmod +x renew-cert.sh
```

### 2.2 Запустите инициализацию SSL

```bash
./init-letsencrypt.sh your-domain.com your-email@example.com
```

**Пример:**
```bash
./init-letsencrypt.sh jeeroway.com admin@jeeroway.com
```

Скрипт выполнит:
1. Создание временного самоподписанного сертификата
2. Запуск nginx
3. Получение настоящего сертификата от Let's Encrypt
4. Перезагрузку nginx

## Шаг 3: Разверните приложение Images

### 3.1 Запустите GitHub Actions workflow

Workflow `publish-img.yml` автоматически:
- Соберет .NET приложение
- Создаст Docker образ
- Запустит контейнер `jimgs` в сети `nginx-proxy-man`

### 3.2 Проверьте, что контейнер запущен

```bash
docker ps | grep jimgs
```

Должен быть контейнер с именем `jimgs` в сети `nginx-proxy-man`.

## Шаг 4: Проверка работы

### 4.1 Проверьте HTTP → HTTPS редирект

```bash
curl -I http://your-domain.com
```

Должен вернуть `301 Moved Permanently` с `Location: https://...`

### 4.2 Проверьте HTTPS

```bash
curl -I https://your-domain.com
```

Должен вернуть `200 OK` с заголовком `Strict-Transport-Security`.

### 4.3 Проверьте Images API

```bash
# Загрузка изображения
curl -X POST https://your-domain.com/images/upload \
  -F "file=@test.jpg"

# Получение изображения
curl https://your-domain.com/images/{id}
```

### 4.4 Проверьте SSL сертификат

```bash
openssl s_client -connect your-domain.com:443 -servername your-domain.com
```

Или откройте в браузере и проверьте замок 🔒 в адресной строке.

## Автоматическое обновление сертификатов

### Вариант 1: Docker автообновление (уже настроено)

Certbot контейнер автоматически проверяет обновления каждые 12 часов.

### Вариант 2: Cron на хосте

Добавьте в crontab:

```bash
crontab -e
```

Добавьте строку:
```
0 3 * * * /path/to/IaC/renew-cert.sh >> /var/log/letsencrypt-renew.log 2>&1
```

## Структура проекта после настройки

```
IaC/
├── certbot/
│   ├── conf/              # SSL сертификаты
│   │   └── live/
│   │       └── your-domain.com/
│   │           ├── fullchain.pem
│   │           └── privkey.pem
│   └── www/               # Для валидации Let's Encrypt
├── nginx/
│   └── conf/
│       └── nginx.conf     # Конфигурация с SSL
├── docker-compose.yml
├── init-letsencrypt.sh    # Первичная настройка SSL
└── renew-cert.sh          # Обновление сертификатов
```

## Endpoints после настройки

- `https://your-domain.com/` - Frontend
- `https://your-domain.com/api/` - Main API
- `https://your-domain.com/images/` - Images API (JeerowayWiki.Images)

## Troubleshooting

### Ошибка: "Connection refused"

Проверьте, что контейнеры запущены:
```bash
docker compose ps
```

### Ошибка: "Certificate not found"

Запустите заново:
```bash
./init-letsencrypt.sh your-domain.com your-email@example.com
```

### Ошибка: "Too many certificates already issued"

Let's Encrypt имеет лимиты. Используйте staging режим для тестирования:
- Откройте `init-letsencrypt.sh`
- Измените `STAGING=0` на `STAGING=1`
- Запустите скрипт
- После успешного теста верните `STAGING=0`

### Nginx не видит контейнер jimgs

Проверьте, что контейнер в правильной сети:
```bash
docker inspect jimgs | grep NetworkMode
```

Должно быть: `"NetworkMode": "nginx-proxy-man"`

## Безопасность

Конфигурация включает:
- ✅ TLS 1.2 и 1.3
- ✅ HSTS (Strict-Transport-Security)
- ✅ X-Frame-Options
- ✅ X-Content-Type-Options
- ✅ X-XSS-Protection
- ✅ Автоматическое обновление сертификатов
- ✅ HTTP → HTTPS редирект

## Полезные команды

```bash
# Перезагрузить nginx
docker compose exec webserver nginx -s reload

# Проверить конфигурацию nginx
docker compose exec webserver nginx -t

# Посмотреть логи nginx
docker compose logs -f webserver

# Посмотреть логи certbot
docker compose logs certbot

# Проверить срок действия сертификата
openssl x509 -in certbot/conf/live/your-domain.com/fullchain.pem -noout -dates

# Принудительно обновить сертификат
docker compose run --rm certbot renew --force-renewal
docker compose exec webserver nginx -s reload
```

## Дополнительная информация

- [Let's Encrypt Documentation](https://letsencrypt.org/docs/)
- [Nginx SSL Configuration](https://nginx.org/en/docs/http/configuring_https_servers.html)
- [SSL Labs Test](https://www.ssllabs.com/ssltest/) - проверка качества SSL конфигурации
