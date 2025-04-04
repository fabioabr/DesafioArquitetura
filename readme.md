# Desafio de Arquitetura de Software

O objetivo deste desafio é desenvolver uma arquitetura de software escalável e resiliente, garantindo alta disponibilidade, segurança e desempenho. A arquitetura deve contemplar:

## Aspectos Principais

- **Escalabilidade:** Capacidade de lidar com aumentos de carga sem perda significativa de desempenho, utilizando dimensionamento horizontal, balanceamento de carga e cache.
- **Resiliência:** Projeto que permita recuperação rápida em casos de falha, incluindo redundância, failover, monitoramento e estratégias de recuperação.
- **Segurança:** Proteção de dados e sistemas com autenticação, autorização, criptografia e proteção contra ataques.
- **Padrões Arquiteturais:** Escolha adequada entre microsserviços, monolitos, SOA, serverless, considerando simplicidade e flexibilidade.
- **Integração:** Comunicação clara e eficiente entre componentes, definindo protocolos, formatos e ferramentas.
- **Requisitos Não-Funcionais:** Otimização para desempenho, disponibilidade e confiabilidade, com métricas claras.
- **Documentação:** Registro das decisões arquiteturais, diagramas e fluxos para facilitar comunicação e manutenção.

## Descrição da Solução

O projeto deve permitir ao comerciante controlar o fluxo de caixa diário com lançamentos (débitos e créditos) e gerar um relatório consolidado diário com o saldo.

## Requisitos Técnicos Obrigatórios

- Solução em C#.
- Testes unitários.
- Aplicação de boas práticas (Design Patterns, SOLID, etc.).
- Documentação clara (README).
- Repositório público no GitHub com todas as documentações necessárias.

## Requisitos Não Funcionais

- Independência entre o serviço de lançamentos e o consolidado diário (tolerância a falhas).
- Capacidade de receber até 50 requisições por segundo no serviço de consolidação, com até 5% de perda aceitável em picos.

## Observações

São valorizadas decisões técnicas assertivas, considerando também melhorias e evoluções futuras para o sistema proposto.


# Proposta para Cenário Real

Em um cenário de produção, a arquitetura ideal para este sistema seria baseada em microsserviços, proporcionando escalabilidade, resiliência e facilidade de manutenção. A solução proposta consiste em três microsserviços principais, juntamente com componentes adicionais para garantir a robustez do sistema (ou uma proposta totalmente focada em Serverless computing, detalhada no final do documento):

## Microsserviços:

### API Coletora de Transações:

* Este serviço atua como o ponto de entrada para todas as transações (débitos e créditos).
* Responsabilidades:
    * Receber requisições HTTP (RESTful API).
    * Validar os dados da transação (formato, integridade, etc.).
    * Poderia enriquecer a mensagem com metadados relevantes (timestamp, identificador da transação, etc.).
    * Publicar a mensagem validada em uma fila do RabbitMQ.
* Tecnologias: ASP.NET Core Web API ou Serverless computing (Lambda, Cloud Functions, AWS Fargate , etc)
* Motivos da alternativa : Ao escolher Serverless computing, você pode abrir mão de um cluster kubernetes para este serviço e ao mesmo tempo pode usufruir de cotas gratuitas de utilização mensal, que dependendo do contrato podem reduzir significativamente o custo com o contexto.

### Processador de Transações:

* Este serviço consome as mensagens da fila do RabbitMQ e persiste as transações no banco de dados.
* Responsabilidades:
    * Ouvir a fila do RabbitMQ.
    * Receber mensagens de transação.
    * Realizar transformações e adaptações necessárias nos dados.
    * Persistir os dados no banco de dados MongoDB.
* Tecnologias: ASP.NET Core Hosted Services ou Serverless computing (Lambda, Cloud Functions,AWS Fargate etc)
* Motivos da alternativa : Ao escolher Serverless computing, você pode abrir mão de um cluster kubernetes para este serviço e ao mesmo tempo pode usufruir de cotas gratuitas de utilização mensal, que dependendo do contrato podem reduzir significativamente o custo com o contexto.

### Processador de Consolidação:

* Este serviço é responsável por consolidar os dados de transação e gerar o saldo diário.
* Responsabilidades:
    * Executar um job agendado (Quartz.NET) várias vezes no dia (dependendo da configuração por env/appsettings).
    * Consultar o banco de dados MongoDB para obter as transações do período. Utilizando a réplica de leitura para não impactar o serviço de processamento de transações.
    * Calcular o saldo consolidado por Data/Hora/Tipo.
    * Armazenar o saldo consolidado no MongoDB.
    * Expor um endpoint de API para leitura do consolidado usando uma réplica de leitura.
* Tecnologias: ASP.NET Core Web API ou Serverless computing (Lambda, Cloud Functions, AWS Fargate, etc)
* Motivos da alternativa : Ao escolher Serverless computing, você pode abrir mão de um cluster kubernetes para este serviço e ao mesmo tempo pode usufruir de cotas gratuitas de utilização mensal, que dependendo do contrato podem reduzir significativamente o custo com o contexto.

## Componentes Adicionais:

### RabbitMQ:

* Utilizado como um message broker para comunicação assíncrona entre a API Coletora e o Processador de Transações.
* Garante a resiliência e a escalabilidade do sistema, permitindo que os serviços funcionem de forma independente.
* Pode ser substituído por serviços gerenciados como por exemplo o SQS da AWS e possuí freetier de 1MM de mensagens gratuitas por mês.

### MongoDB:

* Utilizado como banco de dados NoSQL para armazenar as transações.
* Oferece alta escalabilidade e flexibilidade para lidar com grandes volumes de dados.
* Pode ser substituído por serviços gerenciados como, por exemplo, o DynamoDB, mas necessitaria da implementação de um novo adapter.
* Pode ser substituído pelo Atlas MongoDB gerenciados que apesar de fornecer uma cota de Freetier interessante, para o cenário de produção não serviria. Mas para ambientes de desenvolvimento e staging poderia ser usado as cotas reduzindo os custos ate o ambiente de staging.

### PostgreSQL:

* Utilizado como banco de dados do Api Gateway Kong e da interface web Konga.
* Usado apenas pera tornar possivel a utilização do Kong
* Se usassemos algum API Gateway gerenciado, seria desnecessário a criação do deployment do Postgres

### Quartz.NET (Pacote):

* Pacote Quartz.Net utilizado para agendar a execução do job de consolidação.
* Permite a configuração de horários e intervalos de execução flexíveis.
* Poderia ser substituido por um evento agendado no AWS CloudWatch que dispara uma função lambda para executar o processo de consolidação.

### AspectInjector (Pacote):

* Pacote AspectInjector utilizado para permitir injeção de código em tempo de compilação, de forma que conseguimos criar um "middleware" na execução de qualquer método do sistema, permitindo então a implementação de Cache por método utilizando apenas uma decoração com Atrributo de metodo.
* Permite a configuração de horários e intervalos de execução flexíveis.
* Poderia ser substituido por serviços como Redis, ElastiCache (AWS), DynamoDB + DAX (AWS) ou até mesmo um S3 com Cloudfront.

### Docker / Kubernetes:

* Os microsserviços serão conteinerizados usando Docker e orquestrados pelo Kubernetes (Existe a possibilidade de alguns serviços serem gerenciados e substituir deployments do kubernetes... mas isso é um tradeoff sobre custo x praticidade).
* Isso garante a portabilidade, a escalabilidade e a alta disponibilidade do sistema.
* Para o caso de escolhermos o Kubernetes, teriamos a vantagem do autoscaling do proprio kubernetes, em compensação teriamos o custo do cluster/gerenciado (GKE, EKS, etc). Existe a possibilidade de gerar as imagens do serviço de disponibilizar em um AWS Fargate por trás de um WAF e um Load Balancer a fim de garantir Resiliencia e Escalabilidade. Outra possibilidade é usarmos AWS Lambda para execução Serverless com melhores possibilidades de escalabilidade e disponibilidade.

### Monitoramento e Log:

* Implementação de um sistema de monitoramento robusto para acompanhar a saúde e o desempenho dos serviços usando Grafana, Prometheus e Loki (para logs).
* Utilização de logs estruturados para facilitar a depuração e a análise de problemas.
* Implementação de alertas e notificações para garantir a resposta rápida a problemas críticos
* Possibilidade de integrar metricas e logs ao CloudWatch sem precisar de um cluster kubernetes com Grafana/Loki/Prometheus.

### Segurança:

* Implementação de mecanismos de autenticação e autorização usando API Key para proteger o serviço de coleta de transações.
* Proteção dos serviços expostos com rate limit configurado no Gateway.
* Utilização de criptografia TLS para comunicação segura entre os serviços.

## Fluxo de Dados:

1.  Uma requisição de transação é enviada para a API Coletora.
2.  A API Coletora valida a requisição e publica a mensagem no RabbitMQ.
3.  O Processador de Transações consome a mensagem do RabbitMQ e persiste os dados no MongoDB.
4.  O Quartz.NET dispara o job de consolidação no Processador de Consolidação de N em N minutos (configurável).
5.  O Processador de Consolidação consulta o MongoDB, calcula o saldo consolidado e armazena de volta no MongoDB em outra collection. (Isso quebra o conceito de micro serviços, no entanto quando temos a possibilidade de implantação muilt-AZ ou Cross Region Replication o banco centralizado passa a ser apenas um ponto lógico, pois fisicamente está em diversos lugares aumentando a segurança, disponibilidade e confiabilidade)
6.  O saldo consolidado pode ser consultado através de um endpoint da API do Processador de Consolidação, que mantem o response em cache de memória por N minutos (configurável).

## Observações:
Esta arquitetura garante a escalabilidade, a resiliência e a manutenibilidade do sistema, permitindo que ele se adapte às necessidades do negócio e lide com grandes volumes de dados. Além disso, a implementação dos MessageBrokers e dos Repositórios foram feitos com Adapters genéricos facilitando a migração de tecnologia de Mensageria e Banco respectivamente.

Na maioria dos casos coloquei opções gerenciadas, mas precisamos analisar com cautela estas escolhas, pois quanto mais perto ficamos do ServerLess (Facilidade, auto gerenciamento, custo baixo) mais perto ficamos do lockin  de cloud também. 


# Cenário de avaliação local com Docker-Compose

## Cenário Proposto 

A solução proposta para o cenário real descreve uma arquitetura de microsserviços robusta e escalável, adequada para um ambiente de produção. As principais características incluem:

* **Microsserviços**: Arquitetura baseada em serviços independentes para coleta, processamento e consolidação de transações.
* **Mensageria (RabbitMQ)**: Comunicação assíncrona para melhorar o desempenho e a resiliência.
* **Persistência de Dados (MongoDB e PostgreSQL)**: Bancos de dados NoSQL e relacional para armazenamento eficiente de transações e saldos consolidados.
* **Segurança**: Implementação de API Keys, rate limiting e criptografia TLS.
* **Abstração e Adapters**: Uso de Adapters genéricos para facilitar a migração de tecnologias.
* **Soluções Serverless**: Possibilidade do uso de tecnologia serverless aproveitando o melhor da cloud (auto gerenciamento e disponibilidade)

## Cenário Implementado (Repositório)

A implementação no repositório, por outro lado, é mais simplificada e focada em uma demonstração local. As principais diferenças em relação ao cenário proposto são:

* **Arquitetura Monolítica**: Um projeto ÚNICO de Web API (Minimal API .Net 9.0) que lida com todas as operações, em vez de microsserviços separados. No entanto em um cenário de implantação com imagens docker é perfeitamente possivel distribuir fisicamente a imagem com envs configuradas de formas diferntes para que possa atingir comportamentos distintos em cada implantação (usando a mesma Imagem docker). Neste caso, temos um projeto com tudo, mas implantação funcionalidades distintas em hardware separado.
* **Mensageria**: A implantação de mensageria é feita na própria API
* **Segurança Simplificada**: Foi utilizado apenas controle de autenticação com API Key na API ee controle de ratelimit no Kong, demonstrando que podemos escolher entre usar o Gateway para Autenticação/Autorização e segurança do serviço, como podemos usar a própria API (Na maioria das vezes é melhor ter este controle no Gateway, pois vc impede processamento desnecessário no serviço).;
* **Abstração Limitada**: Abstração usada apenas na camada de infra estruturacom os Adapters de Banco, de Mensageria e os Repositórios implementados com Abstração. O Serviço de controle de cache por exemplo poderia ter uma abstração para plugar em um redis ou outra coisa, mas não foi implementado.


## Justificativa para as Diferenças

É importante ressaltar que a implementação completa do cenário proposto com varios microsserviços, aplicação total dos conceitos de Solid, BFF (se houvesse um frontend), testes em todos as classes, testes integrados, etc, não faz sentido para uma demonstração local. O objetivo principal do desafio é avaliar as habilidades de arquitetura e design de software, e não a capacidade de configurar um ambiente de produção complexo.

A implementação atual serve como um protótipo funcional que demonstra os conceitos principais e as decisões de design. Ela pode ser facilmente expandida e adaptada para um ambiente de produção, seguindo as diretrizes do cenário proposto.

## Conclusão

Apesar das diferenças, a implementação no repositório demonstra um bom entendimento dos conceitos de arquitetura e design de software. As decisões tomadas foram justificadas pelo foco na simulação local e na limitação de tempo para o desenvolvimento.

Este documento serve como um guia para entender as diferenças entre os cenários e as decisões tomadas durante a implementação.


# Stack tecnológica que eu escolheria

Para este projeto, eu optaria por uma arquitetura serverless (neste exemplo AWS), utilizando serviços gerenciados para maximizar a escalabilidade, a resiliência e a eficiência de custos. Acredito que essa abordagem oferece vantagens significativas em relação a implantações em Kubernetes e Fargate, especialmente para um projeto como este.

## Diagrama

![Diagrama](https://github.com/fabioabr/DesafioArquitetura/blob/master/docs/Diagramas/Proposta%20Serverless.png?raw=true)

## Explicação dos Serviços

* **API Gateway**: Gerencia as requisições de entrada, roteando-as para as funções Lambda apropriadas.
* **Lambda**: Executa o código da aplicação de forma serverless, escalando automaticamente conforme a demanda.
* **SQS**: Gerencia a fila de mensagens para o processamento assíncrono das transações.
* **DocumentDB**: Armazena as transações e os dados consolidados.
* **EventBridge**: Agenda a execução da função Lambda de consolidação de dados.
* **ElastiCache**: Armazena os dados consolidados em cache para otimizar o desempenho da API de consolidação.

### Benefícios do Serverless

* **Escalabilidade Automática**: Os serviços serverless escalam automaticamente para lidar com picos de carga, sem a necessidade de provisionar e gerenciar servidores.
* **Resiliência**: Os serviços gerenciados da AWS são altamente resilientes, com alta disponibilidade e tolerância a falhas.
* **Eficiência de Custos**: O modelo de pagamento por uso do serverless permite otimizar os custos, pagando apenas pelos recursos consumidos.
* **Menos Gerenciamento**: A AWS gerencia a infraestrutura, permitindo que a equipe se concentre no desenvolvimento da aplicação.
* **Implantação Rápida**: A implantação de funções Lambda e outros serviços gerenciados é rápida e fácil.

### Comparação com Kubernetes e Fargate

Embora Kubernetes e Fargate ofereçam flexibilidade e controle, eles exigem mais gerenciamento e podem ser mais caros para cargas de trabalho variáveis. O serverless é ideal para aplicações com picos de tráfego, onde a escalabilidade automática e a eficiência de custos são cruciais.

### Custo Estimado de Operação Mensal

O custo estimado de operação mensal para esta arquitetura serverless foi baseado nos seguintes fatores:

* Número de requisições da API Gateway
* Número de invocações das funções Lambda
* Quantidade de dados armazenados no DocumentDB
* Quantidade de mensagens na fila SQS
* Tempo de execução das funções Lambda
* Quantidade de dados armazenados em cache no ElastiCache


### Considerações Finais

A arquitetura serverless proposta oferece uma solução robusta, escalável e eficiente para o controle de fluxo de caixa diário. A utilização de serviços gerenciados da AWS permite otimizar os custos e reduzir o esforço de gerenciamento, permitindo que a equipe se concentre no desenvolvimento da aplicação.


## Executando a Aplicação Localmente

Para executar a aplicação localmente e acessar todos os componentes, siga os passos abaixo. Certifique-se de que você possui as seguintes versões instaladas:

* **Docker Desktop:** v4.40 ou superior
* **Visual Studio 2022:** v17.13.5 ou superior
* **.NET 9**

### Passos para Execução

1.  **Clone o Repositório:**

    ```bash
    git clone <URL do repositório>
    cd <nome do repositório>
    ```

2.  **Execute o Docker Compose:**
    
    ```bash
    docker-compose up --build
    ```
    A partir da pasta:

    ```bash
    /src/docker-compose/
    ```

    Este comando irá construir e iniciar todos os containers necessários para a aplicação, incluindo:

    * Financial API
    * MongoDB
    * RabbitMQ
    * Grafana
    * Konga / Konga-db
    * Kong / Kong-db
    * Prometheus
    
3.  **Acesse os Componentes:**

    * **API:**
        * A API estará disponível através do Kong em `http://localhost:8000`.
        
        * Criação de Transação:

        ```bash
        curl --location 'http://localhost:8000/financial/api/v1/transaction' \
            --header 'x-api-key: 9e3f2b78-d79e-4dc1-9b19-13a96d109af6' \
            --header 'Content-Type: application/json' \
            --data '{
            "type": "Debit",
            "amount": 368.00,
            "description": "Payment for invoice #1234",
            "timestamp": "2025-04-03T11:46:00Z",
            "sourceAccount": "ACC123456",
            "destinationAccount": "XXXXXX",
            "originalTransactionId": null
            }'
        ```

        * Obtenção do Relatório consolidado por dia:
  
        ```bash
        curl --location 'http://localhost:8000/reports/api/v1/daily-report/2025-03-30' \
            --header 'X-Api-Key: 9e3f2b78-d79e-4dc1-9b19-13a96d109af6' \
            --header 'X-Timezone: America/Sao_Paulo'
        ```

        Importante passar o timezone para o relatório ser montado corretamente de acord com a Data do timezone do usuário.


    * **MongoDB:**
        * O MongoDB estará acessível na porta padrão (27017). Você pode usar um cliente como o MongoDB Compass para se conectar.
    * **RabbitMQ Admin:**
        * O painel de administração do RabbitMQ estará disponível em `http://localhost:15672`.
        * Use as credenciais padrão ( `admin/admin`) para fazer login.
    * **Grafana:**
        * O Grafana estará disponível em `http://localhost:3000`.
        * Use as credenciais padrão (geralmente `admin/admin`) para fazer login.
        * Existem dashboards JA CONFIGURADOS para o Kong, MongoDB e RabbitMQ

          * Mongo DB
         ![Dash MongoDB](https://github.com/fabioabr/DesafioArquitetura/blob/master/docs/Diagramas/DashboardMongodb.png?raw=true)

          * RabbitMQ
         ![Dash MongoDB](https://github.com/fabioabr/DesafioArquitetura/blob/master/docs/Diagramas/DashboardRabbitmq.png?raw=true)

          * Kong
         ![Dash MongoDB](https://github.com/fabioabr/DesafioArquitetura/blob/master/docs/Diagramas/DashboardKong.png?raw=true)
          
    * **Konga:**
        * O Konga estará disponivel em `http://localhost:1337`.
        * Usuario será criado no primeiro acesso... sugestão (admin123/admin123)
    * **Prometheus:**
        * O Prometheus estará disponivel em `http://localhost:9090`.
        * Não é necessário ter usuário e senha
          
4.  **Executando Testes:**

    * Para executar os testes unitários, utilize o comando:

    ```bash
    dotnet test
    ```
    Ou execute pelo Test Explorer do próprio visual studio

5.  **App Settings:**

    * Abaixo o JSON de configuração contido no APP Settings
      
    ```json
    {
     "CustomSettings": {
       "UseDevelopmentTransactionBigSeed": false, // Roda um script de alimentação de dados com 3 dias de carga, gera aproximadamente 50 registros por minuto na média  (Diretamente no banco de dados)
       "UseDevelopmentTransactionContinuousSeed": false, // Roda um job por 20 minutos que fica inserindo randomicamente de 4 a 10 registros a cada 2 segundos, passando pelo UseCase. Ou seja, gera mensageria.
       "UseTransactionEndpoints": true, // se false, não expoe os endpoints       
       "UseConsolidationReportJob": true, // se false não roda o job schedulado para consolidar as transações
       "UseSubscriptions": true, // se false, não le a fila para processar as transações

       "DatabaseToUse": "MongoDB", // Usado para a factory de construção do adapter de banco de dados 
       "EventBusToUse": "RabbitMQ", // Usado para a factory de construção do adapter de mensageria

       "DatabaseSettings": {
         "MongoDB": {
           "ConnectionString": "mongodb://admin:admin@mongodb:27017/FinancialDB?authSource=admin",
           "DatabaseName": "FinancialDB"
         }
   
       },
       "ObservabilitySettings": {
         "GrafanaLokiUrl": "http://loki:3100"
       },
       "EventBusSettings": {
         "RabbitMQSettings": {
           "HostName": "rabbitmq",
           "Port": 5672,
           "User": "admin",
           "Password": "admin",
           "ExchangeType": "topic"
         }
       },
       "JobSettings": {
         "CreateReportsJob": {
           "CronScheduleConfig": "0 */20 * * * ?", // se UseConsolidationReportJob = true, usa essa configuração para definir o intervalo de execução
           "Timezones": [ "UTC", "America/Sao_Paulo" ] // se UseDevelopmentTransactionBigSeed = true, recria o cache para os timezones configurados aqui ao recriar os relatórios consolidados
         }
       }
     },
   
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
    }

    

### Considerações

* Certifique-se de que o Docker Desktop esteja em execução antes de executar o `docker-compose up`.
* As portas utilizadas pelos componentes podem ser configuradas no arquivo `docker-compose.yml`.
* As credenciais de acesso dos componentes, também podem ser configuradas no arquivo `docker-compose.yml`.

Este tópico fornece instruções claras para executar a aplicação localmente e acessar todos os componentes necessários para desenvolvimento e teste.

