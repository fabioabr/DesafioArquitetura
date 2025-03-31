FROM kong:3.5

USER root

RUN apt-get update && apt-get install -y netcat

RUN apt-get update && apt-get install -y postgresql-client

COPY ["docker-compose/scripts/kong/kong-bootstrap/kong-bootstrap.sh", "/kong-bootstrap.sh"]

ENTRYPOINT ["sh", "/kong-bootstrap.sh"]