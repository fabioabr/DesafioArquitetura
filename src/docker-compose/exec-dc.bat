docker-compose down -v --rmi all --remove-orphans
docker image prune -af
docker volume prune -f
docker-compose build --no-cache
docker-compose up -d

pause