#!/bin/bash

# SSL certificate initialization script for Let's Encrypt
# Usage: ./init-letsencrypt.sh your-domain.com your-email@example.com

if [ "$#" -ne 2 ]; then
    echo "Usage: $0 <domain> <email>"
    echo "Example: $0 example.com admin@example.com"
    exit 1
fi

DOMAIN=$1
EMAIL=$2
DATA_PATH="./certbot"
STAGING=0  # Set to 1 for testing

echo "### Preparing directories for $DOMAIN ..."
mkdir -p "$DATA_PATH/conf/live/$DOMAIN"
mkdir -p "$DATA_PATH/www"

echo ""
echo "### Creating dummy certificate for $DOMAIN ..."
docker compose run --rm --entrypoint "\
  openssl req -x509 -nodes -newkey rsa:2048 -days 1\
    -keyout '/etc/letsencrypt/live/$DOMAIN/privkey.pem' \
    -out '/etc/letsencrypt/live/$DOMAIN/fullchain.pem' \
    -subj '/CN=localhost'" certbot

echo ""
echo "### Starting nginx ..."
docker compose up --force-recreate -d webserver

echo ""
echo "### Deleting dummy certificate for $DOMAIN ..."
docker compose run --rm --entrypoint "\
  rm -Rf /etc/letsencrypt/live/$DOMAIN && \
  rm -Rf /etc/letsencrypt/archive/$DOMAIN && \
  rm -Rf /etc/letsencrypt/renewal/$DOMAIN.conf" certbot

echo ""
echo "### Requesting Let's Encrypt certificate for $DOMAIN ..."

# Select appropriate email arg
case "$EMAIL" in
  "") email_arg="--register-unsafely-without-email" ;;
  *) email_arg="--email $EMAIL" ;;
esac

# Enable staging mode if needed
if [ $STAGING != "0" ]; then staging_arg="--staging"; fi

docker compose run --rm --entrypoint "\
  certbot certonly --webroot -w /var/www/certbot \
    $staging_arg \
    $email_arg \
    -d $DOMAIN \
    -d www.$DOMAIN \
    --rsa-key-size 4096 \
    --agree-tos \
    --force-renewal" certbot

echo ""
echo "### Reloading nginx ..."
docker compose exec webserver nginx -s reload

echo ""
echo "### SSL certificate successfully obtained for $DOMAIN!"
echo "### Don't forget to update nginx.conf with your domain name."
