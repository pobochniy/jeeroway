#!/bin/bash

# SSL certificate renewal script
# Add to crontab: 0 3 * * * /path/to/renew-cert.sh >> /var/log/letsencrypt-renew.log 2>&1

cd "$(dirname "$0")"

echo "### Renewing SSL certificates at $(date) ..."

docker compose run --rm certbot renew

echo "### Reloading nginx ..."
docker compose exec webserver nginx -s reload

echo "### Certificate renewal completed at $(date)"
