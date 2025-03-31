#!/bin/sh

echo "⏳ Aguardando o banco kong-database ficar pronto..."

until pg_isready -h kong-database -p 5432 -U kong; do
  echo "⏳ Esperando o banco de dados estar 100% pronto..."
  sleep 2
done

echo "✅ Banco pronto. Executando bootstrap..."
kong migrations bootstrap && echo "✅ Bootstrap finalizado!"

touch /kong-bootstrapped
