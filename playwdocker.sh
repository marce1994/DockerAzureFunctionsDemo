git clone https://github.com/marce1994/DockerAzureFunctionsDemo.git
cd DockerAzureFunctionsDemo
docker build -t functiondemoimage .
docker run -d --rm --name functiondemocontainer -p 80:80 functiondemoimage
ip4=$(/sbin/ip -o -4 addr list eth0 | awk '{print $4}' | cut -d/ -f1)
docker run --net host -ti jpetazzo/ngrok http $ip4:80