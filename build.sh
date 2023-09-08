#!/bin/bash

name="ft-db"

docker compose up -d

dos2unix ./db/init.sql

echo "> Copying scripts..."
sudo docker cp ./db/init.sql $name:/root/init.sql
if echo "$@" | grep -e "-c" -q; then
	docker exec $name /bin/sh -c 'mysql -u root -proot ft-db </root/init.sql'
fi
