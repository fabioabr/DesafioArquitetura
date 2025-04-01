@echo off
title Rebuild do Projeto - Docker Compose
cls


echo.
echo Deseja rodar 'docker image prune -a -f' para limpar imagens antigas?
set /p PRUNE=Digite S para sim ou N para nao: 


echo ====================================
echo      REBUILD DO PROJETO DOCKER      
echo ====================================

echo.
echo Passo 1: Parando containers e removendo volumes...
docker-compose down -v --remove-orphans

if /I "%PRUNE%"=="S" (
    echo.
    echo Limpando imagens nao utilizadas...
    docker image prune -a -f
) else (
    echo.
    echo Pulando limpeza de imagens.
)

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
