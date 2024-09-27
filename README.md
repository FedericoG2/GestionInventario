# Gestión de Inventario

## Descripción

Aplicación de escritorio desarrollada en **.NET** y **C#** para la gestión de inventarios en una pequeña empresa. Esta herramienta te permite realizar operaciones de **CRUD** sobre productos, gestionar el stock con alertas cuando está bajo, y buscar productos por código, nombre o categoría. Es ideal para mantener un control eficiente del inventario.

## Características

- Gestión de productos (CRUD):
  - Crear nuevos productos.
  - Editar productos existentes.
  - Eliminar productos.
  - Visualizar el inventario completo.
- Alertas automáticas cuando el stock es bajo.
- Búsqueda de productos por:
  - Código
  - Nombre
  - Categoría

## Base de Datos

La aplicación utiliza una base de datos SQLite con una única tabla llamada `Productos`, que contiene los siguientes campos:

- **Código** (PK): Clave primaria que identifica de manera única a cada producto.
- **Nombre**: El nombre del producto.
- **Descripción**: Información detallada del producto.
- **Precio**: El precio del producto en la moneda local.
- **Stock**: La cantidad disponible en inventario.
- **Categoría**: El tipo o categoría a la que pertenece el producto.

## Requisitos

- .NET Framework o .NET Core
- Visual Studio o un IDE compatible con C#
- SQLite

## Demo
https://www.loom.com/share/651c537839b94091941eed1c105fb5c8?sid=7e46113a-256f-438e-9e82-91434bff167d
