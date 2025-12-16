#!/bin/bash

# SSL Setup using DNS-01 challenge (works without opening port 80)

set -e

if [ "$#" -ne 2 ]; then
    echo "Usage: $0 <domain> <email>"
    echo "Example: $0 img.jeeroway.ru admin@jeeroway.ru"
    exit 1
fi

DOMAIN=$1
EMAIL=$2

echo "=== SSL Setup using DNS validation for $DOMAIN ==="
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

# 2. Obtain certificate using DNS challenge
echo ""
echo "Step 2: Obtaining SSL certificate using DNS validation..."
echo ""
echo "Certbot will ask you to add a TXT record to your DNS."
echo "Follow the instructions carefully!"
echo ""

sudo certbot certonly --manual \
    --preferred-challenges dns \
    -d $DOMAIN \
    -d www.$DOMAIN \
    --agree-tos \
    --email $EMAIL \
    --no-eff-email

# 3. Convert to PFX format
echo ""
echo "Step 3: Converting certificate to PFX format..."
sudo openssl pkcs12 -export \
    -out /etc/letsencrypt/live/$DOMAIN/certificate.pfx \
    -inkey /etc/letsencrypt/live/$DOMAIN/privkey.pem \
    -in /etc/letsencrypt/live/$DOMAIN/fullchain.pem \
    -passout pass:

# 4. Set permissions
echo ""
echo "Step 4: Setting permissions..."
sudo chmod 644 /etc/letsencrypt/live/$DOMAIN/certificate.pfx

echo ""
echo "=== SSL Setup Complete! ==="
echo ""
echo "Certificate location: /etc/letsencrypt/live/$DOMAIN/"
echo "PFX file: /etc/letsencrypt/live/$DOMAIN/certificate.pfx"
echo ""
echo "Next steps:"
echo "1. Add GitHub secret: DOMAIN_NAME_IMG = $DOMAIN"
echo "2. Update appsettings.Production.json to enable HTTPS"
echo "3. Update GitHub Actions workflow to mount certificate"
echo "4. Run your GitHub Actions workflow"
echo ""
echo "Note: DNS validation requires manual renewal."
echo "For auto-renewal, you need to open port 80 in your cloud firewall."
