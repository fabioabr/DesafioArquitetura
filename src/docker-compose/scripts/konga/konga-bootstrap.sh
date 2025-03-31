#!/bin/sh

echo "⏳ Aguardando o banco konga-database ficar pronto..."
until nc -z -v -w30 konga-database 5432
do
  echo "⏳ Esperando conexão com o banco de dados..."
  sleep 2
done

echo "✅ Banco pronto! Executando prepare..."
node ./bin/konga.js prepare --adapter postgres --uri postgres://konga:konga@konga-database:5432/konga

echo "🚀 Iniciando o Konga..."
node app.js
