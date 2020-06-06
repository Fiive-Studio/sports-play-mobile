# Sports Play Mobile

![Fiive](https://fiivestudio.com/wp-content/uploads/2020/06/Fiive-Open-Source_2.png)

Aplicación móvil para reservas de canchas del proyecto Sports Play

## Comenzando 🚀

La aplicación permite ver el listado de canchas que están asociadas en la aplicación y permite realizar búsquedas de horarios disponibles para reservar

Mirar la sección **Deployment** para conocer como desplegar el proyecto.

### Pre-requisitos 📋

Este proyecto hace parte de Sports Play, por lo que requiere que tanto el API de autenticación como la de procesamiento estén cargadas para que se pueda realizar la conexión al sistema

## Deployment 📦

Para poder que el proyecto se ejecute correctamente es necesario que se realicen algunas configuraciones:

 - Buscar el archivo RestService.cs que está en la ruta /FutbolPlay/FutbolPlay.WebApi/Services y modificar las variables

|Variable|Descripción|
|--|--|
|_uriAuthBase|URL donde está el API de autenticación|
|_uriBase|URL donde está el API de proceso|
|_clientIdPlay|Client Id que se defina en el API para Sports Play Mobile|
|_clientIdPlayAdmin|Client Id que se defina en el API para Sports Play Coach Mobile|

 - Si se requiere crear integración para la autenticación con Facebook se debe crear una aplicación en dicha plataforma y una vez se tenga creada, se debe buscar el archivo FacebookModel.cs que está en la ruta /FutbolPlay/FutbolPlay.WebApi/Model y modificar la variable

|Variable|Descripción|
|--|--|
|ClientId|Client Id asignado en la aplicación de Facebook|

## Construido con 🛠️

* [Xamarin](https://dotnet.microsoft.com/apps/xamarin)

## Autores ✒️

* **[Pablo Díaz](https://fiivestudio.com/pablo-diaz/)**
* **[Alejandra Morales](https://fiivestudio.com/alejandra-morales/)**

## Notas Adicionales

* Fiive Studio te invita a que experimentes con este proyecto