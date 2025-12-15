#!/bin/bash

# SSL Setup Script for JeerowayWiki.Images
# This script sets up Let's Encrypt SSL certificate for ASP.NET Core app

set -e

if [ "$#" -ne 2 ]; then
    echo "Usage: $0 <domain> <email>"
    echo "Example: $0 example.com admin@example.com"
    exit 1
fi

DOMAIN=$1
EMAIL=$2

echo "=== SSL Setup for $DOMAIN ==="
echo ""

# 1. Install Certbot
echo "Step 1: Installing Certbot..."
if ! command -v certbot &> /dev/null; then
    if [ -f /etc/debian_version ]; then
        sudo apt-get update
        sudo apt-get install -y certbot
    elif [ -f /etc/redhat-release ]; then
        sudo yum install -y certbot
    else
        echo "Please install certbot manually"
        exit 1
    fi
else
    echo "Certbot already installed"
fi

# 2. Stop any service on port 80
echo ""
echo "Step 2: Checking port 80..."
if sudo lsof -Pi :80 -sTCP:LISTEN -t >/dev/null ; then
    echo "Warning: Port 80 is in use. Stopping docker containers..."
    docker stop jimgs 2>/dev/null || true
fi

# 3. Obtain certificate
echo ""
echo "Step 3: Obtaining SSL certificate from Let's Encrypt..."
sudo certbot certonly --standalone \
    -d $DOMAIN \
    -d www.$DOMAIN \
    --non-interactive \
    --agree-tos \
    --email $EMAIL \
    --preferred-challenges http

# 4. Convert to PFX format for ASP.NET Core
echo ""
echo "Step 4: Converting certificate to PFX format..."
sudo openssl pkcs12 -export \
    -out /etc/letsencrypt/live/$DOMAIN/certificate.pfx \
    -inkey /etc/letsencrypt/live/$DOMAIN/privkey.pem \
    -in /etc/letsencrypt/live/$DOMAIN/fullchain.pem \
    -passout pass:

# 5. Set permissions
echo ""
echo "Step 5: Setting permissions..."
sudo chmod 644 /etc/letsencrypt/live/$DOMAIN/certificate.pfx

# 6. Setup auto-renewal
echo ""
echo "Step 6: Setting up auto-renewal..."
RENEW_SCRIPT="/usr/local/bin/renew-cert-aspnet.sh"
sudo tee $RENEW_SCRIPT > /dev/null <<'EOF'
#!/bin/bash
DOMAIN=$(ls /etc/letsencrypt/live/ | grep -v README | head -n 1)
certbot renew --quiet
openssl pkcs12 -export \
    -out /etc/letsencrypt/live/$DOMAIN/certificate.pfx \
    -inkey /etc/letsencrypt/live/$DOMAIN/privkey.pem \
    -in /etc/letsencrypt/live/$DOMAIN/fullchain.pem \
    -passout pass:
chmod 644 /etc/letsencrypt/live/$DOMAIN/certificate.pfx
docker restart jimgs 2>/dev/null || true
EOF

sudo chmod +x $RENEW_SCRIPT

# Add to crontab if not exists
if ! sudo crontab -l 2>/dev/null | grep -q "renew-cert-aspnet.sh"; then
    (sudo crontab -l 2>/dev/null; echo "0 3 * * * $RENEW_SCRIPT >> /var/log/letsencrypt-renew.log 2>&1") | sudo crontab -
    echo "Auto-renewal added to crontab (runs daily at 3 AM)"
fi

echo ""
echo "=== SSL Setup Complete! ==="
echo ""
echo "Certificate location: /etc/letsencrypt/live/$DOMAIN/"
echo "PFX file: /etc/letsencrypt/live/$DOMAIN/certificate.pfx"
echo ""
echo "Next steps:"
echo "1. Add GitHub secret: DOMAIN_NAME = $DOMAIN"
echo "2. Run your GitHub Actions workflow to deploy the app"
echo "3. Your app will be available at:"
echo "   - http://$DOMAIN (redirects to HTTPS)"
echo "   - https://$DOMAIN"
echo ""
echo "Certificate will auto-renew every 60 days."
