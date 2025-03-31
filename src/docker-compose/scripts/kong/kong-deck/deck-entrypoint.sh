#!/bin/sh

echo "⏳ Esperando Kong responder na porta 8001..."
until curl -s http://kong:8001/ > /dev/null; do
  echo "Esperando Kong responder na porta 8001..."
  sleep 2
done

echo "✅ Kong disponível. Aplicando configuração com deck..."
deck gateway sync /kong-config.yml --kong-addr http://kong:8001

