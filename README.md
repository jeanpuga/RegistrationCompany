# RegistrationCompany
Esse projeto exemplo, segue a filosofia de DDD apreentado por Evans E., e a concepção de arquitetura de Vertical Slices de Jimmy Bogard.
Aqui apresento exemplo de:
- Api Minimals
- Behaviors
- Mediator
- Problem Details
- CQRS
- EF Latest
- .Net8
- Testes Unitários
- Organização src/test
- Docker e docker-compose

## Plus
Foi implementado um sistema de cache distribuído utilizando MongoDB, no qual cada execução de um comando "COMMAND" limpa a tabela de cache para a operação GetAll. Na primeira iteração, essa tabela é recriada completamente. Os resultados individuais (Get Singles) armazenados em cache são atualizados a cada execução do comando subsequente.

Dessa forma, todas as consultas "QUERY" realizadas na segunda chamada são direcionadas ao cache, sem a necessidade de acessar a base de dados transacional do SqlServer no Entity Framework em memória, como exemplificado aqui.

Essa é uma implementação de CQRS, não mais puramente de arquitetura de código, mas sim também de de banco de dados real.
