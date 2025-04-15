# Considerações sobre Feedback Técnico do Desafio

Este documento tem o objetivo de registrar, de forma transparente e construtiva, algumas observações sobre o feedback recebido em relação ao desafio de arquitetura de soluções apresentado neste repositório.

---

## 📌 Contexto

O desafio propunha a elaboração de uma arquitetura funcional que refletisse o papel de um Arquiteto de Soluções, considerando:

- Entendimento de requisitos funcionais e não funcionais;
- Desenho de arquitetura de referência;
- Decomposição por domínios e capacidades de negócio;
- Justificativas de decisões arquiteturais;
- Registro técnico via documentação e/ou código.

No enunciado original, fica claro que **a codificação completa não era mandatória**, sendo a intenção principal avaliar:

> _"...conhecimento empírico, capacidade de tomada de decisão, aplicação de boas práticas, decomposição dos domínios e componentes, etc."_

> _“Não é necessário que todas essas premissas sejam apresentadas na codificação.”_

---

## 🧠 Intenção da Entrega e Justificativas Estratégicas

A proposta entregue foi estruturada de forma a demonstrar, dentro do tempo e escopo disponíveis, uma **visão ampla, pragmática e analítica** da solução — focando mais na **capacidade de arquitetura**, **escolhas conscientes**, e **coerência técnica**, do que em uma implementação 100% pronta para produção.

Esses pontos foram detalhados no `README.md` original nas seções:

### ✅ Cenário Proposto

O cenário desenhado inclui:
- Microsserviços isolados com seus próprios contextos e bancos;
- Comunicação assíncrona desacoplada por mensageria (RabbitMQ);
- Observabilidade integrada com Grafana + Prometheus;
- Proteção e controle de tráfego com API Gateway (Kong);
- Estratégia de consolidação de dados por batch com tolerância a falhas.

### ✅ Cenário Implementado (Repositório)

Para fins de entrega do desafio, foi implementado um subconjunto da arquitetura:
- API com persistência no MongoDB;
- Consolidador agendado com Quartz.NET;
- Comunicação via fila local (RabbitMQ);
- Observabilidade parcial (Loki com Serilog);
- Docker Compose funcional para setup inicial.

### ✅ Justificativa para as Diferenças

> _"Como este desafio tinha como objetivo avaliar o conhecimento empírico, as escolhas feitas visam **entregar um protótipo funcional**, que evidencie a arquitetura proposta, com **simplificações intencionais** voltadas à agilidade e à clareza."_

> _"Exemplo: foi mantida uma estrutura única de testes e repositório de dados compartilhado para facilitar o ciclo de execução e reduzir o tempo total de desenvolvimento."_

Essas decisões foram **documentadas abertamente** para que os avaliadores compreendessem o racional por trás de cada “quebra de boas práticas”, deixando claro que se tratava de **escolhas conscientes**, e não de desconhecimento técnico.

---

## 🧾 Feedback técnico recebido (na íntegra)

> - O readme.md está bem escrito, apesar de alguns pequenos erros de copiar e colar;  
> - A arquitetura parece funcional, e os diagramas são claros;  
> - Manifestou preocupação com o lock-in no CSP ao se ampliar a adoção de serverless.  
> 
> **Pontos de melhoria:**  
> - O título do readme.md é "Desafio de Arquitetura de Software", enquanto a aplicação é para "Arquitetura de Soluções";  
> - A proposta ficou um pouco indecisa entre usar uma implementação serverless e uma aplicação web convencional;  
> - Ao executar o docker compose, apenas um serviço subiu conforme o esperado;  
> - Dos vinte e dois testes, três falharam, possivelmente por alguma dependência dos serviços;  
> - A aplicação está dividida em camadas, mas o projeto de testes é único;  
> - Existem muitas referências importadas que não são utilizadas;  
> - Há variáveis declaradas, mas não utilizadas;  
> - A validação é feita campo a campo e, para cada erro, uma exceção é lançada, em vez de validar tudo e lançar apenas uma exceção ao final;  
> - Existem campos imutáveis com "set" declarado;  
> - Há menção a custos, mas eles não foram apresentados;  
> - A proposta de arquitetura não é agnóstica, pois se concentra na AWS;  
> - Não entregou um mapeamento de domínios funcionais e capacidades de negócio;  
> - Não é o CDN que acessa a aplicação, são os artefatos distribuídos pelo CDN que acessam a partir do dispositivo do usuário;  
> - Em alguns pontos os diagramas apresentados se confundem entre diagrama de implantação, topologia e fluxo de dados;  
> - O uso de ASP.NET Core Hosted Services é questionável, uma vez que depende de um loop infinito e em caso de exceções não tratadas o processo é finalizado, indisponibilizando o consumo das mensagens;  
> - O Processador de Consolidação consultar as transações na réplica expõe a solução a inconsistências provocadas pelo atraso na réplica assíncrona. Ao se utilizar réplica síncrona com confirmação de entrega, garantindo que não haverá inconsistência pela leitura na réplica, a consulta na réplica também concorre com a replicação síncrona;  
> - A justificativa apresentada para a "quebra do conceito de microsserviço" não procede. Num microsserviço o banco de dados é exclusivo, independente se o DBMS é centralizado, distribuído, com ou sem réplica;  
> - Uma vez que o PostgreSQL é um requisito de uma dependência, este não deve ser mencionado como banco de dados da solução;  
> - Em Custo Estimado de Operação Mensal não é apresentada qualquer estimativa de custo.  
> 
> **Recomendações para futuras aplicações:**  
> - Apresentar mapeamento de domínios funcionais e capacidades de negócio;  
> - Apresentar um desenho completo da solução entregue;  
> - Apresentar um desenho completo da solução proposta;  
> - Apresentar um desenho completo da arquitetura de transição;  
> - Registrar as tomadas de decisão arquiteturais (ADR) lastreando as escolhas das ferramentas, tecnologias e tipo de arquitetura empregados:  
>   - Por que Quartz.NET e não Hangfire?  
>   - Por que delegar a persistência da transação a um fluxo assíncrono?  
>   - Por que um ASP.NET Hosted Service?  
>   - Por que implementar o próprio mapeamento de endpoints?  
> - Instruções de como rodar a aplicação funcionando a partir do clone;  
> - Segregar melhor as redes entre os serviços no docker-compose.yml;  
> - O PostgreSQL do Kong não precisa estar na mesma rede do worker.  
> 
> **Parecer:** A diversidade de assuntos abordados sugere um amplo conhecimento em diversos aspectos importantes na arquitetura de soluções. Contudo, não aproveita a oportunidade para aprofundar o suficiente para evidenciar domínio dos fundamentos, cometendo diversos pequenos erros em todos os aspectos.

---

## ⚠️ Pontos de Contradição Identificados

| Enunciado | Feedback recebido | Observação |
|----------|-------------------|------------|
| “Não é necessário que todas essas premissas sejam apresentadas na codificação.” | Cobrança por testes unitários em todas as camadas e falhas de execução no Docker Compose. | A avaliação parece priorizar execução completa e automatizada, destoando do foco em decisões arquiteturais. |
| “A intenção do desafio é analisar o seu conhecimento empírico.” | Foram cobradas justificativas altamente específicas (ex: uso de Quartz.NET ao invés de Hangfire, Hosted Services, réplica assíncrona). | Tais pontos são válidos tecnicamente, mas são típicos de entrevistas técnicas aprofundadas, não de avaliação documental. |
| “Não se prenda somente aos critérios técnicos mencionados.” | Feedback enfatiza ausência de mapeamentos, custos e diagramas extras. | Embora sejam boas práticas, o volume de exigências extrapola o tempo e foco previstos. |
| “Use o teste para demonstrar sua habilidade em tomar decisões sobre o que é importante.” | O parecer considerou “diversidade de assuntos abordados” como algo negativo por não haver aprofundamento total em cada ponto. | Esperava-se amplitude e tomada de decisão, mas o retorno penaliza a escolha de amplitude em detrimento da profundidade. |

---

## ✅ Pontos atendidos e entregues

Mesmo com as contradições apontadas, a entrega contemplou:

- Definição de arquitetura distribuída com racional por serviço;
- Desenhos claros de fluxo, estrutura e comunicação;
- Separação em camadas e estrutura de projeto limpa;
- Abordagem serverless/híbrida com consciência de lock-in;
- Implementação funcional com testes e docker-compose;
- README descritivo com instruções e contexto de negócio;
- Uso de boas práticas como DDD, separação de responsabilidade, AutoMapper, validações, e padrões de extensão.

---

## 📘 Conclusão

Este desafio foi tratado com seriedade e dedicação. A documentação, a arquitetura proposta e as decisões tomadas refletem a intenção de **demonstrar pensamento sistêmico, autonomia técnica e responsabilidade arquitetural**, que são pilares da função de Arquiteto de Soluções.

Entendo que toda avaliação tem seus critérios, mas considero importante registrar estas observações com objetivo de clareza e melhoria contínua — tanto para quem avalia quanto para quem se submete a esse tipo de processo.

---

**Fabio Rodrigues**  
[linkedin.com/in/fabioabr](https://www.linkedin.com/in/fabioabr)
