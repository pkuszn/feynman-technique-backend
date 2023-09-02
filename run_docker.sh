name="feynman-technique-backend"

echo "> Creating an image..."
if sudo docker image inspect $name >/dev/null 2>&1; then
	echo "Image exists locally"
else
	echo "Image does not exists locally"
	sudo docker build -t $name -f Dockerfile .
fi

echo "> Creating a $name container..."
if [ ! "$(sudo docker ps -a -q -f name=$name)" ]; then
    if [ "$(sudo docker ps -aq -f status=exited -f name=$name)" ]; then
        sudo docker rm -f $name
    fi
    sudo docker run -d -p 5200:80 --name $name $name
fi

echo "> Starting a $name container..."
if [ "$(sudo docker ps -aq -f status=exited -f name=$name)" ]; then
    sudo docker start $name
fi