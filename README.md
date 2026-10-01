# Laboratorio-Repaso-del-CRUD-M-todos-Sobrecargados

## 🟥PROBLEMA 1 – Ejemplos de Inyección SQL

En este problema se desarrollaron **tres ejemplos de consultas SQL** para comprender de forma práctica cómo funciona una vulnerabilidad de **Inyección SQL**.

La inyección SQL ocurre cuando una consulta permite que ciertos valores introducidos sean interpretados como parte del código SQL y no solamente como datos. Esto puede cambiar la lógica original de la consulta, permitir consultar información que normalmente no debería mostrarse o incluso aprovechar el comportamiento de la base de datos para obtener información indirectamente.

Los tres ejemplos realizados fueron:

- Manipulación de condiciones mediante `OR '1'='1'`.
- Uso de comentarios `--` para ignorar parte de una consulta.
- Inyección basada en tiempo utilizando `SLEEP()`.

---

### Ejemplo 1 – Manipulación de la condición con `OR '1'='1'`

### Código utilizado

```sql
USE cristell;

SELECT * 
FROM productos
WHERE id = 0 OR '1'='1'
AND cantidad = 0 OR '1'='1';
```

### Explicación

En este primer ejemplo se utiliza la condición:

```sql
'1'='1'
```

Esta comparación siempre devuelve **verdadero**, ya que el valor `1` es igual a `1`.

El objetivo es demostrar cómo una condición agregada mediante `OR` puede modificar completamente la lógica original de un `WHERE`.

Aunque inicialmente se intenta buscar productos con:

```sql
id = 0
```

y:

```sql
cantidad = 0
```

también se agregan condiciones como:

```sql
OR '1'='1'
```

Al utilizar `OR`, basta con que una de las condiciones sea verdadera para que la expresión completa pueda cumplirse.

Por esta razón, la consulta puede terminar mostrando varios o incluso todos los registros de la tabla `productos`, aunque los valores originales de `id` o `cantidad` no coincidan.

Este ejemplo representa una **Inyección SQL basada en lógica booleana**.


### Imagen de ejecución
![Ejecución Ejemplo 1](CAPTURAS/ejemplo1SQL.png)

---

### Ejemplo 2 – Uso de comentarios para ignorar parte de la consulta

### Código utilizado

```sql
SELECT *
FROM productos
WHERE nombre = 'Teclado' -- ' AND id = 999;
```

### Explicación

En este ejemplo se utiliza el símbolo:

```sql
--
```

En SQL, los dos guiones representan el inicio de un **comentario de una sola línea**.

Esto significa que todo lo que aparece después de `--` deja de ser interpretado como parte de la consulta SQL.

En este caso tenemos:

```sql
WHERE nombre = 'Teclado' -- ' AND id = 999;
```

La parte:

```sql
' AND id = 999;
```

queda ignorada por la base de datos.

Por lo tanto, la consulta termina funcionando prácticamente como:

```sql
SELECT *
FROM productos
WHERE nombre = 'Teclado';
```

La condición `id = 999` ya no es tomada en cuenta.



### Imagen de ejecución
![Ejecución Ejemplo 2](CAPTURAS/ejemplo2SQL.png)

---

### Ejemplo 3 – Inyección SQL basada en tiempo

### Código utilizado

```sql
USE cristell;

SELECT *
FROM productos
WHERE id = 1-SLEEP(5);
```

### Explicación

En este ejemplo se utiliza la función:

```sql
SLEEP(5)
```

Esta función hace que MySQL espere aproximadamente **5 segundos** antes de continuar con la ejecución de la consulta.

La condición utilizada es:

```sql
WHERE id = 1-SLEEP(5);
```

Primero se ejecuta:

```sql
SLEEP(5)
```

Después de esperar los 5 segundos, la función normalmente devuelve `0`.

Por lo tanto, la expresión termina siendo equivalente a:

```sql
id = 1 - 0
```

es decir:

```sql
id = 1
```

La consulta termina buscando el producto con `id = 1`, pero antes de mostrar el resultado se produce un retraso intencional.

Este comportamiento permite explicar un tipo de vulnerabilidad conocido como **Time-Based SQL Injection** o **Inyección SQL basada en tiempo**.

En este tipo de ataque no siempre se obtiene información directamente en pantalla. En cambio, se analiza cuánto demora la base de datos en responder.



### Imagen de ejecución
![Ejecución Ejemplo 3](CAPTURAS/ejemplo3SQL.png)


---

## Conclusión del Problema 1

Con los tres ejemplos realizados se pudieron observar diferentes formas en las que una consulta SQL puede ser manipulada:

1. **`OR '1'='1'`** permite alterar la lógica de una condición y hacer que esta siempre resulte verdadera.
2. **`--`** permite convertir una parte de la consulta en comentario e ignorar condiciones posteriores.
3. **`SLEEP()`** permite provocar un retraso intencional en la consulta y demostrar una inyección basada en tiempo.

Estos ejemplos muestran por qué es importante evitar construir consultas SQL concatenando directamente los datos ingresados por el usuario.

Una de las principales formas de prevención es utilizar **consultas preparadas y parametrizadas**, ya que permiten que los valores ingresados sean tratados como datos y no como instrucciones SQL.

-------

## 🟥PROBLEMA 2 – Uso de Dictionary, List y generación dinámica de SQL

En este problema se trabajó con un `Dictionary<string, object>` para almacenar información de un producto y posteriormente utilizar sus claves para construir de forma dinámica partes de una consulta SQL.

Los temas principales trabajados fueron:

- Uso de `Dictionary<string, object>`.
- Uso de la propiedad `.Keys`.
- Recorrido de colecciones mediante `foreach`.
- Uso de `List<string>`.
- Uso de `string.Join()`.
- Creación dinámica de columnas y parámetros.
- Construcción de una sentencia `INSERT INTO`.

---
### Explicación

#### 1. Creación del diccionario

Se crea un `Dictionary<string, object>` llamado `datosInventario`, donde cada dato se guarda como una relación **clave-valor**:

```text
Nombre -> Laptop HP Envy
Precio -> 850.99
Cantidad -> 15
```

Se usa `object` porque los valores pueden ser de distintos tipos, como `string`, `decimal` o `int`.

---

#### 2. Creación de la lista `setParts`

```csharp
var setParts = new List<string>();
```

Esta lista se utiliza para guardar expresiones como:

```text
Nombre = @Nombre
Precio = @Precio
Cantidad = @Cantidad
```

---

#### 3. Uso de `.Keys` y `foreach`

```csharp
foreach (var key in datosInventario.Keys)
{
    setParts.Add($"{key} = @{key}");
}
```

`.Keys` obtiene las claves del diccionario, es decir:

```text
Nombre
Precio
Cantidad
```

El `foreach` las recorre una por una y crea automáticamente las expresiones con sus respectivos parámetros.

---

#### 4. Uso de `string.Join()`

```csharp
string setClause = string.Join(", ", setParts);
```

`string.Join()` une los elementos de la lista en una sola cadena:

```text
Nombre = @Nombre, Precio = @Precio, Cantidad = @Cantidad
```

Esto permite generar de forma dinámica una cláusula que podría utilizarse en un `UPDATE`.

---

#### 5. Creación de columnas y parámetros

```csharp
var columns = string.Join(", ", datosInventario.Keys);
var placeholders = "@" + string.Join(", @", datosInventario.Keys);
```

Se generan automáticamente:

```text
Columnas:
Nombre, Precio, Cantidad

Parámetros:
@Nombre, @Precio, @Cantidad
```

---

#### 6. Construcción de la sentencia SQL

```csharp
string sql = $"INSERT INTO productos ({columns}) VALUES ({placeholders})";
```

El resultado final es:

```sql
INSERT INTO productos (Nombre, Precio, Cantidad)
VALUES (@Nombre, @Precio, @Cantidad)
```

La consulta se construye automáticamente a partir de las claves del diccionario, sin escribir manualmente cada columna.

## Resultado esperado

Al ejecutar el programa, en la consola se muestra:

```text
Cláusula SET generada: Nombre = @Nombre, Precio = @Precio, Cantidad = @Cantidad

La cadena sql es: INSERT INTO productos (Nombre, Precio, Cantidad) VALUES (@Nombre, @Precio, @Cantidad)
```

---

## Imagen de ejecución

> Insertar aquí la captura de pantalla donde se observa la salida del programa en la consola.

```markdown
![Ejecución Problema 2](CAPTURAS/Problema2.png)
```


## Conclusión del Problema 2

En este problema se practicó el uso conjunto de **diccionarios, listas, ciclos `foreach` y `string.Join()`**.

El `Dictionary<string, object>` permitió almacenar los datos del producto utilizando los nombres de las columnas como claves. Posteriormente, `.Keys` permitió obtener esas claves y utilizarlas para generar automáticamente tanto una cláusula `SET` como una sentencia `INSERT INTO`.

Con este ejemplo se puede comprender cómo estas estructuras permiten crear código más flexible y reutilizable al momento de trabajar con operaciones CRUD y consultas SQL.


