# CRUD de Productos — C# (.NET Framework) + MySQL

**Curso:** Herramientas de la Programación Aplicada III (.Net)
**Laboratorio:** CRUD de productos con imágenes (Windows Forms + MySQL)

Aplicación de escritorio que permite **insertar, consultar, modificar y eliminar** productos, guardando la imagen de cada producto directamente en la base de datos (campo `LONGBLOB`) y mostrándola en una tabla (`DataGridView`). Incluye búsqueda dinámica mientras se escribe.

---

## Tabla de contenido

1. [Detalles del laboratorio](#1-detalles-del-laboratorio)
2. [Tecnologías y versiones](#2-tecnologías-y-versiones)
3. [Procesos de instalación](#3-procesos-de-instalación)
4. [Base de datos (MySQL)](#4-base-de-datos-mysql)
5. [Controles utilizados](#5-controles-utilizados)
6. [Evidencias](#6-evidencias)
   - [Insertar registros](#61-insertar-registros)
   - [Modificar registros](#62-modificar-registros)
   - [Eliminar registros](#63-eliminar-registros)
7. [Estructura del proyecto](#7-estructura-del-proyecto)
8. [Detalles del autor, fecha y referencias](#8-detalles-del-autor-fecha-y-referencias)

---

## 1. Detalles del laboratorio

**Objetivo:** desarrollar una aplicación de escritorio en C# conectada a MySQL que implemente las cuatro operaciones básicas de un CRUD (Create, Read, Update, Delete), incluyendo el manejo de imágenes binarias.

**Funcionalidades:**

| Operación | Descripción |
|---|---|
| **Crear** | Se ingresan nombre, precio, cantidad y una imagen (clic sobre el recuadro de la foto). Al presionar **Guardar** se inserta el registro. |
| **Leer** | Los productos se cargan automáticamente al abrir el formulario y se muestran en un `DataGridView` con su imagen. |
| **Buscar** | El cuadro de búsqueda filtra en tiempo real por `id`, `nombre`, `precio` o `cantidad` (consulta con `LIKE`). |
| **Modificar** | Al hacer clic en una fila, sus datos se cargan en los campos; se editan y se presiona **Modificar**. |
| **Eliminar** | Se selecciona una fila y se presiona **Eliminar**; antes se pide confirmación. |
| **Limpiar** | Restablece los campos y la imagen por defecto. |

**Aspectos técnicos destacados:**

- **Consultas parametrizadas** (`@parametro`) en `INSERT`, `UPDATE`, `DELETE` y en la búsqueda, para evitar inyección SQL.
- Métodos genéricos `InsertSeguro`, `UpdateSeguro` y `DeleteSeguro` en la clase `Conexion`, que reciben los datos en un `Dictionary<string, object>`.
- Conversión de imagen ⇄ `byte[]` mediante `MemoryStream` para almacenarla en `LONGBLOB`.
- **Validación de datos:** campos obligatorios y verificación de que precio (`decimal`) y cantidad (`int`) sean numéricos.
- Si no se elige imagen, se guarda un arreglo de bytes vacío (la columna `imagen` es `NOT NULL`).

---

## 2. Tecnologías y versiones

| Tecnología | Versión |
|---|---|
| Lenguaje | C# |
| Plataforma | .NET Framework **4.7.2** |
| Interfaz | Windows Forms (WinForms) |
| IDE | Visual Studio (proyecto con `ToolsVersion 15.0`, Visual Studio 2017 o superior) |
| Base de datos | MySQL Server (8.x recomendado) |
| Conector | **MySql.Data 26.7.0** (NuGet) |
| Conector alterno incluido | MySqlConnector 2.6.2 (NuGet) |
| Otros paquetes NuGet | Google.Protobuf 3.35.0, BouncyCastle.Cryptography 2.6.2, K4os.Compression.LZ4 1.3.8, ZstdSharp.Port 0.8.8, System.Configuration.ConfigurationManager 10.0.8, entre otros (ver `packages.config`) |
| Charset de la BD | `utf8mb4` |

---

## 3. Procesos de instalación

Pasos que se siguieron para preparar el entorno y desarrollar el laboratorio, en orden.

### 3.1 Instalación de las herramientas

1. **Visual Studio:** se instaló con la carga de trabajo **“Desarrollo de escritorio con .NET”**, que incluye Windows Forms y .NET Framework.
2. **MySQL Server:** se instaló junto con **MySQL Workbench**. Durante la instalación se definió la contraseña del usuario `root`, que luego se usa en la cadena de conexión.

### 3.2 Creación de la base de datos

1. Se abrió MySQL Workbench y se conectó al servidor local.
2. Se escribió y ejecutó el script `crear_base_datos.sql`, que crea la base `productosdb` y la tabla `productos` (ver sección 4).
3. Se verificó con `SELECT * FROM productos;` que la tabla quedó creada y vacía.

### 3.3 Creación del proyecto en Visual Studio

1. *Archivo → Nuevo → Proyecto* y se eligió la plantilla **Aplicación de Windows Forms (.NET Framework)**, con nombre `ProyectoProductos` y destino **.NET Framework 4.7.2**.
2. Se instaló el conector de MySQL desde *Herramientas → Administrador de paquetes NuGet → Administrar paquetes NuGet para la solución*, buscando e instalando **MySql.Data 26.7.0**. NuGet agregó automáticamente sus dependencias (Google.Protobuf, K4os.Compression.LZ4, ZstdSharp.Port, BouncyCastle, etc.).

### 3.4 Desarrollo de la aplicación

1. **Modelo:** se creó la clase `Producto.cs` con las propiedades `Id`, `Nombre`, `Precio`, `Cantidad` e `Imagen` (`byte[]`).
2. **Conexión:** se creó la clase `Conexion.cs` con la cadena de conexión y los métodos `ObtenerConexion`, `GetProductos`, `InsertSeguro`, `UpdateSeguro` y `DeleteSeguro`, todos con consultas parametrizadas:

   ```csharp
   private static string cadenaConexion =
       "Server=localhost;Database=productosdb;Uid=root;Pwd=TU_CONTRASEÑA;";
   ```

3. **Interfaz:** en el diseñador de `Form1` se arrastraron los controles (paneles, etiquetas, cuadros de texto, botones, `PictureBox` y `DataGridView` con sus columnas). El detalle está en la sección 5.
4. **Lógica:** en `Form1.cs` se programaron los eventos de carga, búsqueda, selección de imagen, guardar, modificar, eliminar, limpiar y clic en una fila de la tabla, junto con la validación de campos y la conversión de imagen a `byte[]`.

### 3.5 Pruebas

Se compiló (*Compilar → Compilar solución*) y se ejecutó con `F5`. Se probaron las cuatro operaciones del CRUD y la búsqueda, y se tomaron las capturas de la sección 6.

### 3.6 Cómo ejecutar el proyecto (para quien lo revise)

1. Tener instalados Visual Studio y MySQL Server.
2. Ejecutar `crear_base_datos.sql` en MySQL.
3. Abrir `ProyectoProductos.sln` en Visual Studio y restaurar los paquetes NuGet si es necesario.
4. Ajustar `Uid` y `Pwd` en `Conexion.cs` con los datos de su instalación de MySQL.
5. Ejecutar con `F5`.

> **Solución de problemas:** si al guardar aparece *“No se pudo guardar el registro. Revise la conexión a MySQL.”*, verificar que el servicio de MySQL esté activo, que la base `productosdb` exista y que el usuario y la contraseña de la cadena de conexión sean correctos.

---

## 4. Base de datos (MySQL)

Base de datos: **`productosdb`** — Tabla: **`productos`**

```sql
CREATE DATABASE IF NOT EXISTS productosdb CHARACTER SET utf8mb4;
USE productosdb;

DROP TABLE IF EXISTS `productos`;
CREATE TABLE IF NOT EXISTS `productos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `cantidad` int NOT NULL,
  `imagen` longblob NOT NULL,
  PRIMARY KEY (`id`)
);
```

| Columna | Tipo MySQL | Tipo C# | Descripción |
|---|---|---|---|
| `id` | `INT` AUTO_INCREMENT (PK) | `int` | Identificador único |
| `nombre` | `VARCHAR(100)` | `string` | Nombre del producto |
| `precio` | `DECIMAL(10,2)` | `decimal` | Precio del producto |
| `cantidad` | `INT` | `int` | Existencias |
| `imagen` | `LONGBLOB` | `byte[]` | Imagen almacenada en binario |

---

## 5. Controles utilizados

Formulario principal: **`Form1`** (`Text = "CRUD Productos"`).

| Control | Nombre(s) | Función |
|---|---|---|
| `Panel` | `panelTitulo`, `panel1`, `panel2` | Contenedores del encabezado y de la zona de búsqueda |
| `Label` | `label1`, `lblTitulo`, `lblNombre`, `lblPrecio`, `lblCantidad`, `lblBúsqueda` | Título y etiquetas de los campos |
| `TextBox` | `txtNombre`, `txtPrecio`, `txtCantidad`, `txtBusqueda` | Captura de datos y búsqueda dinámica (`TextChanged`) |
| `PictureBox` | `pictureBox1`, `pictureBox2` | `pictureBox1`: vista previa/selección de la imagen del producto; `pictureBox2`: ícono decorativo |
| `Button` | `btnGuardar`, `btnModificar`, `btnEliminar`, `btnLimpiar` | Acciones del CRUD |
| `DataGridView` | `dgvProductos` | Listado de productos (columnas: id, Producto, Precio, Cantidad, Imagen) |
| `DataGridViewTextBoxColumn` / `DataGridViewImageColumn` | `id`, `Producto`, `Precio`, `Cantidad`, `Imagen` | Columnas de la tabla; la de imagen muestra el `LONGBLOB` |
| `OpenFileDialog` | (creado en código) | Selección de imagen (`*.jpg; *.jpeg; *.png; *.bmp`) |
| `MessageBox` | — | Mensajes de validación, confirmación y resultado |
| `ImageList` | `imageList1` | Lista de imágenes del formulario |

**Eventos principales:** `Form1_Load`, `txtBusqueda_TextChanged`, `pictureBox1_Click`, `btnGuardar_Click`, `btnModificar_Click`, `btnEliminar_Click`, `btnLimpiar_Click`, `dgvProductos_CellClick`.

---

## 6. Evidencias
### 6.1 Insertar registros

<img width="842" height="860" alt="image" src="https://github.com/user-attachments/assets/f584dddb-0e56-4423-8e1b-765985d95dd7" />


![Formulario antes de insertar](docs/images/insertar_1_formulario.png)

**Mensaje de confirmación al guardar:**

<img width="1178" height="882" alt="image" src="https://github.com/user-attachments/assets/6b1f16b9-76b4-4f5f-bb0a-dcf7a6a2cf03" />


**Registro visible en la tabla (DataGridView):**

<img width="822" height="822" alt="image" src="https://github.com/user-attachments/assets/056593c4-1249-4ee5-8ef3-44b18961f034" />

**Registro en MySQL Workbench (`SELECT * FROM productos;`):**

<img width="1381" height="638" alt="image" src="https://github.com/user-attachments/assets/c7d5c05c-f001-4ad3-9224-fb44349709f0" />


### 6.2 Modificar registros

**Mensaje de confirmación y tabla actualizada:**

<img width="1132" height="881" alt="image" src="https://github.com/user-attachments/assets/a2ff7fd4-e04a-4fef-973e-6fd28b790e7c" />


### 6.3 Eliminar registros

**Selección del registro y confirmación de eliminación:**

<img width="828" height="882" alt="image" src="https://github.com/user-attachments/assets/98363b1c-d076-4b22-9d1a-258c077d395a" />


**Tabla después de eliminar el registro:**

<img width="830" height="857" alt="image" src="https://github.com/user-attachments/assets/88f7d749-8879-4c49-bd2c-fdbd3960a764" />


---

## 7. Estructura del proyecto

```
ProyectoProductos/
├── crear_base_datos.sql          # Script de creación de la BD y la tabla
├── ProyectoProductos.sln         # Solución de Visual Studio
├── packages/                     # Paquetes NuGet
└── ProyectoProductos/
    ├── Conexion.cs               # Conexión a MySQL y operaciones CRUD (parametrizadas)
    ├── Producto.cs               # Modelo de datos
    ├── Form1.cs                  # Lógica del formulario y eventos
    ├── Form1.Designer.cs         # Diseño de la interfaz
    ├── Program.cs                # Punto de entrada
    ├── App.config                # Configuración (.NET Framework 4.7.2)
    ├── packages.config           # Dependencias NuGet
    └── Resources/                # Imágenes de la interfaz
```

---

## 8. Detalles del autor, fecha y referencias

| | |
|---|---|
| **Autor:** | Josseline Gaitan |
| **Cédula:** | 8-1018-226 |
| **Curso:** | Herramientas de la Programación Aplicada III (.Net) |
| **Docente:** | Irina Fong |
| **Carrera:** |Lic.Ing.Sis.Comp. |
| **Fecha:** |  28 de septiembre de 2026 |

### Referencias

- Oracle. *MySQL 8.0 Reference Manual — Date and Time Data Types (DATETIME, TIMESTAMP).* https://dev.mysql.com/doc/refman/8.0/en/datetime.html
- Oracle. *MySQL Connector/NET Developer Guide.* https://dev.mysql.com/doc/connector-net/en/
- Microsoft. *DataGridView Class (System.Windows.Forms).* https://learn.microsoft.com/dotnet/api/system.windows.forms.datagridview
- Microsoft. *Windows Forms documentation.* https://learn.microsoft.com/dotnet/desktop/winforms/
