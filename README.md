# API de Gestión de Tareas

Esta es una API RESTful construida con *ASP.NET Core* que permite gestionar tareas, incluyendo su creación, visualización, actualización y eliminación. Es ideal para integrarse con aplicaciones de escritorio, móviles o web.

## Tecnologías utilizadas

- ASP.NET Core 8 (Normal API)
- Entity Framework Core
- SQL Server 
- Swagger (para pruebas y documentación de la API)

## Funcionalidades

- Crear nuevas tareas
- Obtener todas las tareas
- Obtener una tarea por su ID
- Actualizar tareas existentes
- Eliminar tareas
- Filtrar por estado o fecha (opcional, según implementación)

## Estructura del proyecto


/Models/Task.cs          // Clase entidad de Tarea
/Data/TaskContext.cs     // Contexto de base de datos
/Program.cs              // Configuración principal de la API
/appsettings.json        // Configuración de conexión y parámetros


## Endpoints principales

| Método | Ruta             | Descripción                    |
|--------|------------------|--------------------------------|
| GET    | /crear           | Obtener todas las tareas       |
| GET    | /listar/{id}     | Obtener una tarea específica   |
| POST   | /Ver             | Crear una nueva tarea          |
| PUT    | /Editar/{id}     | Actualizar una tarea existente |
| DELETE | /Eliminar/{id}   | Eliminar una tarea             |

## Ejemplo de modelo de tarea (JSON)

json
{
  "id": 1,
  "description": "Comprar comida",
  "dueDate": "2025-05-10T00:00:00",
  "status": "Pendiente"
}


## Estructura del proyecto

- Models/Task.cs: Clase que representa la entidad de tarea.
- Data/TaskContext.cs: Contexto de base de datos usando Entity Framework.
- Program.cs: Configuración de la API y endpoints.
- appsettings.json: Configuración de la conexión a la base de datos.

