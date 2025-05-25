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
X-API-KEY: tu_clave
```

---

## Pruebas de integración

Si tienes el proyecto `ProductsService.IntegrationTests`, puedes ejecutar:

```bash
dotnet test ProductsService.IntegrationTests
```

---

## Autor

Diego (nerve)