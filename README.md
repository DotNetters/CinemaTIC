# cinematic (Versión .NET 4.6)

Implementa un sistema muy básico de venta de entradas para un cine (modo TPV). 

## Propósito

El propósito principal de este repositorio es servir de base para la preparación de charlas, talleres y otros eventos de la comunidad técnica [DotNetters Zaragoza](http://dotnetters.es), pero puede jugar con él quien quiera, barra libre.

## Instalación y ejecución

### Requisitos para el entorno de desarrollo

- **IDE**
  - [Visual Studio (sirven las ediciones Community)](https://www.visualstudio.com/es/downloads/)
    - Visual Studio 2015 (Update 3) o Visual Studio 2017
- Acceso a datos [**(SQL Server)**](https://www.microsoft.com/es-es/sql-server/sql-server-downloads), sirven las ediciones [Developer](https://my.visualstudio.com/Benefits?Wt.mc_id=o~msft~sql-server-dev-edition&campaign=o~msft~sql-server-dev-edition) o [Express](https://go.microsoft.com/fwlink/?LinkID=799012) (gratuitas).

### Desde Visual Studio (>= 2015)

1. Descargar el código
2. Compilar
3. Seleccionar el proyecto web como proyecto de inicio
4. Ir a la consola del administrador de paquetes (Ver > Otras ventanas > Consola del administrador de paquetes)
5. Seleccionar en el desplegable el proyecto Cinematic.DAL
6. Ejecutar el comando: 
```<bash>
Update-Database
```
7. Seleccionar el proyecto web como proyecto de inicio
8. Pulsar F5