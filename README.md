# Proyecto de Autenticación de Usuarios LIN

Este es un proyecto desarrollado en C# y .NET 7 que proporciona funcionalidades de autenticación de usuarios para sistemas LIN. El proyecto se centra en garantizar la seguridad y la gestión de usuarios en entornos LIN, permitiendo un acceso controlado a la información.

# Características

- **Autenticación de Usuarios:** El sistema permite a los usuarios autenticarse utilizando sus credenciales de acceso únicas, asegurando que solo usuarios autorizados puedan acceder.

- **Gestión de Usuarios:** Los administradores tienen la capacidad de gestionar usuarios, lo que incluye la creación, edición y eliminación de cuentas de usuario. Esto permite un control total sobre quién tiene acceso y qué permisos se les otorgan.

- **Roles y Permisos:** El sistema implementa un sistema de roles y permisos, lo que permite definir diferentes niveles de acceso para los usuarios.

- **Registro de Actividades:** Se registra cada actividad de inicio de sesión y gestión de usuarios para garantizar la trazabilidad y la seguridad del sistema. Los registros se pueden utilizar para auditorías y para detectar posibles intentos de acceso no autorizados.


## Requisitos del Sistema

- [.NET 7 Runtime](https://dotnet.microsoft.com/download/dotnet/7.0)
- Base de datos compatible con Entity Framework (SQL Server)

## Configuración

1. Clona este repositorio en tu máquina local.

   ```
   git clone https://github.com/LINServices/LIN.Auth.git
   ```

2. Abre la solución en Visual Studio o tu IDE preferido.

3. Configura la conexión a la base de datos en el archivo `appsettings.json`, proporcionando la cadena de conexión correcta.

4. Compila y ejecuta la aplicación.

## Contribución

Si deseas contribuir a este proyecto, ¡serás bienvenido! Puedes contribuir de las siguientes formas:

- Reportando problemas y errores.
- Proponiendo nuevas características.
- Enviando solicitudes de extracción para solucionar problemas o agregar características.

## Licencia

Este proyecto se distribuye bajo la Licencia MIT. Siéntete libre de utilizar, modificar y distribuir este proyecto de acuerdo con los términos de la licencia.
