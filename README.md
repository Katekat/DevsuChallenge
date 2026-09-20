# Devsu prueba tecnica- Microserviios

Solución backend basada en microservicios para la gestión de usuarios y transacciones financieras.

---

## Decisiones de Arquitectura y Diseño 

Este proyecto descarta las estructuras monolíticas tradicionales y las capas horizontales innecesarias en favor de patrones orientados al dominio, la mantenibilidad y el rendimiento:

### 1. Vertical Slice Architecture (VSA)
* **Por qué:** En lugar de organizar el código por capas como Controllers, Services, Repositories que terminan generando acoplamiento horizontal y clases gigantescas, la solución está estructurada por **Features** (Casos de uso). Cada *slice* encapsula su propia lógica, validación y acceso a datos, maximizando la cohesión y facilitando la escalabilidad del equipo de desarrollo y en este caso factor tiempo.

### 2. Patrón Repositorio y Entity Framework Core
* **Por qué:** Se omitió la creación de capas de repositorios genéricos (IRepository<T>). En Entity Framework Core, el DbContext actúa nativamente como un *Unit of Work* y los DbSet<T> funcionan como repositorios tipados. Añadir una capa intermedia de abstracción sobre un ORM moderno introduce complejidad , respetando el principio de simplicidad de VSA.

### 3. Aislamiento de Datos (Database-per-Service)
* **Por qué:** Los microservicios Usuarios y Financiero poseen contextos delimitados Contexto Delimitado, totalmente independientes. Esto se refleja tanto en su infraestructura en contenedores como en sus bases de datos separadas (DevsuUsuarios y DevsuFinanciero), evitando acoplamientos a nivel de esquema y permitiendo ciclos de vida de despliegue.

### 4. Inmutabilidad Financiera (Dominio de Movimientos)
* **Por qué:** El módulo financiero se omitieron los endpoints de actualización (PUT) y eliminación (DELETE) para los movimientos contables. las transacciones son **inmutables**; los errores o ajustes no se sobreescriben.

## 5. Eliminación Lógica (Soft Delete) en Usuarios
* **Por qué:** Para evitar la pérdida de integridad referencial con el microservicio financiero, los usuarios no se eliminan físicamente de la base de datos (DELETE). Se implementa un borrado lógico, asegurando que el histórico de cuentas y transacciones asociadas no quede huérfano ni rompa las relaciones del sistema.

### 6. Resiliencia e Idempotencia en la Mensajería (RabbitMQ)
* **Por qué:** Ante fallos de red o reintentos automáticos del broker de mensajes, se consideran mecanismos de control de duplicados (**idempotencia**) y políticas de reintento. 

---

## Stack Tecnológico
* **Backend:** .NET (ASP.NET Core Web API).
* **Persistencia:** Microsoft SQL Server (con migraciones automáticas en arranque y volúmenes persistentes en Docker).
* **Mensajería:** RabbitMQ (para comunicación asíncrona entre servicios).
* **Contenedorización:** Docker & Docker Compose.

---

## Guía de Ejecución y Despliegue

### Prerrequisitos
* Docker y Docker Compose instalados en el sistema host.
* .NET SDK (en caso de querer compilar o ejecutar en local sin contenedores).

### Levantamiento del Entorno Completo
Para levantar todo el ecosistema (SQL Server con persistencia de volumen, RabbitMQ y los microservicios) ejecutando las migraciones de forma automática al iniciar, ejecuta en la terminal dentro de la ruta donde se haya clonado el proyecto:

docker-compose up -d --build

Pruebas de API: Se incluye la colección DevsuChallenge_KatherineFlorez.postman_collection.json en la raíz del repositorio. Las variables de entorno están preconfiguradas a nivel de colección