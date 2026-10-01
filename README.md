# Laboratorio-Repaso-del-CRUD-M-todos-Sobrecargados

🟥##PROBLEMA 1 – Ejemplos de Inyección SQL

En este problema se desarrollaron **tres ejemplos de consultas SQL** para comprender de forma práctica cómo funciona una vulnerabilidad de **Inyección SQL**.

La inyección SQL ocurre cuando una consulta permite que ciertos valores introducidos sean interpretados como parte del código SQL y no solamente como datos. Esto puede cambiar la lógica original de la consulta, permitir consultar información que normalmente no debería mostrarse o incluso aprovechar el comportamiento de la base de datos para obtener información indirectamente.

Los tres ejemplos realizados fueron:

- Manipulación de condiciones mediante `OR '1'='1'`.
- Uso de comentarios `--` para ignorar parte de una consulta.
- Inyección basada en tiempo utilizando `SLEEP()`.

---

## Ejemplo 1 – Manipulación de la condición con `OR '1'='1'`

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

### ¿Qué demuestra este ejemplo?

Demuestra cómo una consulta mal construida puede permitir modificar la lógica del filtro y obtener información que originalmente no debía mostrarse.

### Imagen de ejecución

> Insertar aquí la captura de pantalla de la ejecución del Ejemplo 1.

<!-- Ejemplo:
![Ejecución Ejemplo 1](imagenes/ejemplo1.png)
-->

---

## Ejemplo 2 – Uso de comentarios para ignorar parte de la consulta

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

### ¿Qué demuestra este ejemplo?

Demuestra cómo el uso de comentarios puede utilizarse para **anular condiciones adicionales de una consulta**.

En un sistema real, esto podría permitir ignorar validaciones importantes, como una contraseña, un identificador o alguna otra condición de seguridad.

### Imagen de ejecución

> Insertar aquí la captura de pantalla de la ejecución del Ejemplo 2.

<!-- Ejemplo:
![Ejecución Ejemplo 2](imagenes/ejemplo2.png)
-->

---

## Ejemplo 3 – Inyección SQL basada en tiempo

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

### ¿Qué demuestra este ejemplo?

Demuestra que el **tiempo de respuesta de la base de datos** también puede utilizarse para detectar si una determinada condición SQL fue ejecutada.

Si una entrada permite ejecutar funciones como `SLEEP()`, puede significar que el contenido introducido está siendo interpretado como código SQL.

### Imagen de ejecución

> Insertar aquí la captura de pantalla de la ejecución del Ejemplo 3.

<!-- Ejemplo:
![Ejecución Ejemplo 3](imagenes/ejemplo3.png)
-->

---

## Conclusión del Problema 1

Con los tres ejemplos realizados se pudieron observar diferentes formas en las que una consulta SQL puede ser manipulada:

1. **`OR '1'='1'`** permite alterar la lógica de una condición y hacer que esta siempre resulte verdadera.
2. **`--`** permite convertir una parte de la consulta en comentario e ignorar condiciones posteriores.
3. **`SLEEP()`** permite provocar un retraso intencional en la consulta y demostrar una inyección basada en tiempo.

Estos ejemplos muestran por qué es importante evitar construir consultas SQL concatenando directamente los datos ingresados por el usuario.

Una de las principales formas de prevención es utilizar **consultas preparadas y parametrizadas**, ya que permiten que los valores ingresados sean tratados como datos y no como instrucciones SQL.
