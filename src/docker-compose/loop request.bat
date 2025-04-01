for i in {1..10}; do
  curl -s -o /dev/null -w "%{http_code}\n" \
       -H "x-api-key: 9e3f2b78-d79e-4dc1-9b19-13a96d109af6" \
       http://localhost:8000/financial/api/v1/transaction
  sleep 2
done

pause