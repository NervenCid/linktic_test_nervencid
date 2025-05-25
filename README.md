# linktic_test_nervencid

Prueba técnica para LinkTic: Microservicios de Productos e Inventario

## Descripción

Este proyecto implementa dos microservicios en .NET 8:

- **ProductsService**: Gestión de productos (CRUD) con MongoDB.
- **InventoryService**: Consulta y actualización de stock, integrando con ProductsService.

Ambos servicios exponen endpoints REST y requieren autenticación mediante API Key.

---

## Estructura del proyecto

```
linktic_test_nervencid/
├── ProductsService/                # Microservicio de productos
├── InventoryService/               # Microservicio de inventario
├── ProductsService.IntegrationTests/ # Pruebas de integración (opcional)
├── docker-compose.yaml             # Orquestación de servicios y MongoDB
├── README.md
└── ...
```

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (opcional, recomendado para pruebas rápidas)
- MongoDB

---

## Configuración

### Variables de entorno

Ambos servicios pueden configurarse mediante variables de entorno o `appsettings.json`.

- **Mongo__ConnectionString**: Cadena de conexión a MongoDB.
- **API_KEYS**: Lista de claves API válidas (separadas por `;`).
- **PRODUCTS_API_BASE**: (Solo InventoryService) URL base del microservicio de productos.

### Ejemplo de `appsettings.json` para desarrollo local

```json
{
  "Mongo__ConnectionString": "mongodb://localhost:27017",
  "ApiKeys": [
    "key_test_local"
  ]
}
```

---

## Ejecución con Docker Compose

1. Construye y levanta los servicios:
   ```bash
   docker-compose up --build
   ```

2. Accede a la documentación Swagger:
   - [http://localhost:5071/swagger](http://localhost:5071/swagger) (ProductsService)
   - [http://localhost:5117/swagger](http://localhost:5117/swagger) (InventoryService)

---

## Ejecución local (sin Docker)

1. Inicia MongoDB localmente.
2. Configura las variables de entorno necesarias:
   ```bash
   set Mongo__ConnectionString=mongodb://localhost:27017
   set API_KEYS=key_test_local
   ```
3. Ejecuta cada microservicio:
   ```bash
   cd ProductsService
   dotnet run
   # En otra terminal:
   cd ../InventoryService
   dotnet run
   ```

---

## Endpoints principales

### ProductsService

- `GET /products` - Lista todos los productos
- `GET /product/{id}` - Obtiene un producto por ID
- `POST /product` - Crea un producto
- `PUT /product/{id}` - Actualiza un producto
- `DELETE /product/{id}` - Elimina un producto

### InventoryService

- `GET /stock/{id}` - Consulta el stock de un producto
- `POST /buy/{id}` - Realiza una compra y descuenta stock

---

## Seguridad

Todos los endpoints requieren el header:

```
X-API-KEY: key_test_local
```
---

## Pruebas de integración

Si tienes el proyecto `ProductsService.IntegrationTests`, puedes ejecutar:

```bash
docker compose-up
dotnet test ProductsService.UnitTests
dotnet test InventoryService.UnitTests
```

---

## Base de datos: MongoDB

Ambos microservicios utilizan **MongoDB** como base de datos NoSQL para almacenar la información de productos y gestionar el inventario.

- **ProductsService**: Guarda los productos en una colección de MongoDB.
- **InventoryService**: Consulta y actualiza el stock de productos a través de ProductsService, que a su vez usa MongoDB.

Puedes levantar MongoDB fácilmente con Docker Compose, o instalarlo localmente y configurar la cadena de conexión mediante la variable de entorno `Mongo__ConnectionString`.

Se utilizo esta base de datos por los siguientes motivos:

- **Escalabilidad:** MongoDB permite escalar horizontalmente de forma sencilla, ideal para microservicios que pueden crecer en volumen de datos.
- **Modelo flexible:** Al ser NoSQL, puedes modificar la estructura de los documentos sin migraciones complejas, facilitando la evolución de los modelos de productos e inventario.
- **Alto rendimiento:** Las operaciones de lectura y escritura son rápidas, lo que mejora la respuesta de los endpoints REST.
- **Integración sencilla:** Existen drivers oficiales y soporte nativo en .NET, lo que simplifica la integración y el acceso a los datos.
- **Ideal para microservicios:** Cada microservicio puede tener su propia base o colección, permitiendo independencia y desacoplamiento entre servicios.

---

## Patrones de diseño utilizados

- **Repository:** Abstracción del acceso a datos para productos e inventario mediante interfaces y clases concretas.
- **Dependency Injection:** Inyección de dependencias para servicios, repositorios y clientes HTTP.
- **Middleware:** Validación de API Key mediante un middleware personalizado.
- **Controller/Endpoint Registration:** Organización de endpoints en clases estáticas tipo controlador.
- **Modelos:** Uso de clases de modelo (`Product`, `BuyRequest`) para representar y transferir datos entre servicios y capas.

---

## Arquitectura

Cada microservicio (ProductsService e InventoryService) sigue principalmente una **arquitectura de capas simple** dentro del contexto de microservicios.  
El enfoque de responsabilidades y organización interna de cada microservicio uno:

### **ProductsService**
- **Capa de Presentación:**  
  Minimal API (endpoints definidos en `ProductsController`).
- **Capa de Aplicación/Lógica:**  
  La lógica de negocio básica se encuentra en los controladores y en la manipulación de los modelos.
- **Capa de Acceso a Datos:**  
  Uso del patrón Repository (`IProductRepository`, `ProductRepository`) para interactuar con MongoDB.
- **Modelos:**  
  Clases como `Product` definen la estructura de los datos.


### **InventoryService**
- **Capa de Presentación:**  
  Minimal API (endpoints definidos en `InventoryController`).
- **Capa de Aplicación/Lógica:**  
  Lógica para validar stock, procesar compras y coordinar la actualización de productos.
- **Capa de Acceso a Datos/Integración:**  
  Repository (`IInventoryRepository`, `InventoryRepository`) que además de acceder a datos, consume el microservicio de productos vía HTTP.
- **Modelos:**  
  Clases como `Product` y `BuyRequest` para estructurar los datos.

Cada microservicio implementa una **arquitectura de capas** (Presentación, Lógica, Acceso a Datos/Integración, Modelos) de manera simple y clara, siguiendo buenas prácticas de separación de responsabilidades dentro del enfoque de microservicios.

---

## Propuesta de mejoras y escalabilidad futura

- **Orquestación con Kubernetes:** Utilizar Kubernetes para gestionar el ciclo de vida, escalado y despliegue de los microservicios de forma eficiente.
- **Autenticación y autorización avanzada:** Implementar OAuth2, JWT o integración con IdentityServer para una gestión de usuarios y permisos más robusta.
- **Observabilidad:** Integrar herramientas de monitoreo, logging centralizado (como ELK, Grafana, Prometheus) y trazabilidad distribuida para facilitar el diagnóstico y la operación.
- **Pruebas automatizadas:** Ampliar la cobertura de pruebas unitarias y de integración, e incorporar pruebas end-to-end (E2E) y pipelines CI/CD.
- **Mensajería asíncrona:** Incorporar colas de mensajes (RabbitMQ, Kafka, Azure Service Bus) para desacoplar procesos y permitir operaciones asíncronas (por ejemplo, compras masivas o eventos de stock).
- **Versionado de APIs:** Implementar versionado de endpoints para facilitar la evolución de los servicios sin afectar a los clientes existentes.
- **Documentación dinámica:** Mejorar la documentación con ejemplos de uso, colecciones de Postman y generación automática de clientes.
- **Seguridad avanzada:** Añadir validaciones adicionales, protección contra ataques comunes (rate limiting, CORS, CSRF) y auditoría de operaciones.

---

## Autor

Diego Camilo Peña Ramírez