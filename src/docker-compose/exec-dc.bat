@echo off
title Rebuild do Projeto - Docker Compose
cls


echo ====================================
echo      REBUILD DO PROJETO DOCKER      
echo ====================================

echo.
echo Passo 1: Parando containers e removendo volumes...
docker-compose down -v --remove-orphans

echo Limpando imagens nao utilizadas...
docker image prune -a -f

echo.
echo Passo 2: Rebuildando as imagens (sem cache)...
docker-compose build --no-cache

echo.
echo Passo 3: Subindo os containers...
docker-compose up -d

echo.
echo ====================================
echo Projeto no ar! Acesse os serviços normalmente.
echo ====================================
pause
