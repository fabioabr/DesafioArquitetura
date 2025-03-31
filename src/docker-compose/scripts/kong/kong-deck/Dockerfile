FROM kong/deck:latest

USER root
RUN apk add --no-cache curl

COPY ["docker-compose/scripts/kong/kong-deck/deck-entrypoint.sh", "/deck-entrypoint.sh"]
COPY ["docker-compose/scripts/kong/kong-deck/kong-config.yml", "/kong-config.yml"]

ENTRYPOINT ["sh", "/deck-entrypoint.sh"]
