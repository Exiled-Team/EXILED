# EXILED - EXtended In-runtime Library for External Development

![EXILED CI](https://github.com/galaxy119/EXILED/workflows/EXILED%20CI/badge.svg?branch=2.0.0)
<a href="https://github.com/Exiled-Team/EXILED/releases">
  <img src="https://img.shields.io/github/release/Exiled-Team/EXILED/all.svg?style=flat" alt="GitHub Releases">
</a>
![Github All Downloads](https://img.shields.io/github/downloads/galaxy119/EXILED/total.svg?style=flat)
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/dev)
<a href="https://discord.gg/PyUkWTg">
  <img src="https://img.shields.io/discord/656673194693885975?logo=discord" alt="Chat on Discord">
</a>


EXILED es una plataforma de desarollo de plugin para servidores de SCP: Secret Laboratory. Ofrece un sistema de eventos para desarolladores y poder modificar o cambiar código del juego, o implementar sus propias funciónes.
Todos los eventos de EXILED están hechos con Harmony, lo que significa qe no requieren editar los códigos de asamblear de los servidores para funcionar, esto ofrece dos beneficios únicos.

 - Primeramente, toda la plataforma de desarollo puede ser publicada y distribuída, lo cual permite que los desarolladores puedan entender mejor *como funciona*, al igual que sugerir ideas o cambiar alguna opción.
 - Segundo, ya que todo el código relacionado con EXILED está afuera de los archivos de asamblea, cosas como actualizaciónes pequeñas tendrán un pequeño (si acaso) efecto en el programa. Esto permite que siga siendo compatible con actualizaciónes futuras, tambíen hace el proceso de acutalización cuando de verdad *es* necesario.

# READMEs Traducidas
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [English](https://github.com/Exiled-Team/EXILED/blob/master/README.md)

# Instalación
La instalación de EXILED puede parecer muy intricada o complicada que otras plataformas, pero en realidad es bastante simple. Como mencionado previamente, la mayoría de EXILED no está adentro del archivo Assembly-CSharp.dll del servidor, sin embargo, si hay una única pequeña modificación a este archivo, la cual es requerida para poder *iniciar* EXILED junto al Assembly-CSharp.dll, una versión limpia del archivo con esta modificación ya viene incluído junto al instalador.

Si elijes el instalador automático (sí se ejecuta correctamente) se encargará de instalar `Exiled.Loader`, `Exiled.Updater`, `Exiled.Permissions`, `Exiled.API` y `Exiled.Events`, al igual que asegurarse de que tu servidor tiene el archivo Assembly-CSharp.dll instalado.

# Windows
### Instalación automática ([más información](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))
**Nota:** Asegurate de que estás en el usuairo que ejectua el servidor o tiene privilegios de administrador antes de ejecutar el Instalador.

  - Descarga **`Exiled.Installer-Win.exe` [de aquí](https://github.com/galaxy119/EXILED/releases)** (seleccióna Assets -> descarga el Instalador)
  - Colócalo en tu carpeta del servidor (descarga el servidor dedicado si no lo has hecho todavía)
  - Haz doble click en **`Exiled.Installer.exe`** o **[descarga este .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** y colócalo en la carpeta del servidor para descargar la última versón de prueba
  - Para instalar y  conseguir plugins, mira el apartado [Instalando plugins](#installing-plugins) abajo.
**Nota:** Si estás instalando EXILED en un servidor remoto, asegurate de ejecutar el .exe como el mismo usuario que ejecutó el servidor de SCP: SL (o un Administrador)

### Instalación manual
  - Descarga **`Exiled.tar.gz` [de aquí](https://github.com/galaxy119/EXILED/releases)**
  - Extrae los contenidos con [7Zip](https://www.7-zip.org/) o [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Mueve el acrhivo **``Assembly-CSharp.dll``** a: **`(Carpeta de tu servidor)\SCPSL_Data\Managed`** y remplaza el archivo.
  - Mueve la carpeta **``EXILED``** a **`%appdata%`** *¡Nota: Esta carpeta tiene que ir en ``C:\Users\(Usuario)\AppData\Roaming``, y  ***NO*** en ``C:\Users\(Usuario)\AppData\Roaming\SCP Secret Laboratory``, y **DEBE ESTAR** en (...)\AppData\Roaming, no (...)\AppData\!*
    - Windows 10/11:
      Escribe `%appdata%` en Cortana / la barra de buscar, o en la barra del Explorador de Archivos
    - Cualquier otra versión:
      Presiona Win + R y escribe `%appdata%`

### Instalando plugins
¡Ya está! EXILED debería estar instalado y activado la próxima vez que inicies el servidor. Nota que EXILED por si solo no hace paenas nada, así que asegurate de conseguir plugins nuevos de **[nuestro servidor de Discord](https://discord.gg/PyUkWTg)**
- Para instalar un plugin, simplemente:
  - Descarga un plugin de [*su* página de versiones](https://i.imgur.com/u34wgPD.jpg) (**¡DEBE ser un `.dll`!**)
  - Muevelo a: ``C:\Users\(Usuario)\AppData\Roaming\EXILED\Plugins`` (muevete a esta carpeta presionando Win + R, despues escribiendo `%appdata%`)

# Linux
### Instalación automática ([más información](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))

**Nota:** Sí estás instalando EXILED en un servidor remoto, asegurate de que ejecutas el Instalador con el mismo usuario que ejecuta el servidor de SCP: SL (o con root)

  - Descarga **`Exiled.Installer-Linux` [de aquí](https://github.com/galaxy119/EXILED/releases)** (seleccióna Assets -> descarga el Instalador)
  - Se instala escribiendo **`./Exiled.Installer-Linux --path /carpeta/del/servidor`** o muevelo adentro de la carpeta del servidor directamente, muevete a la carpeta desde la terminal (`cd`) y escribe: **`./Exiled.Installer-Linux`**.
  - Si quieras la versión de prueba más reciente, añade **`--pre-releases`**. Ejemplos: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Otro ejemplo si pusiste `Exiled.Installer-Linux` en la carpeta del servidor: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - Para instalar y conseguir plugins, mira el apartado [Instalando plugins](#installing-plugins-1) más abajo.

### Instalación manual
  - **Asegurate** de que estás conectado con el usuario que tiene los servidores de SCP:SL.
  - Descarga **`Exiled.tar.gz` [de aquí](https://github.com/galaxy119/EXILED/releases)** (SSH: click derecho para conseguir el link `Exiled.tar.gz`, despues escribe: **`wget (link_para_descarga)`**)
  - Para extraerlo, escribe **``tar -xzvf EXILED.tar.gz``**
  - Mueve el archivo **``Assembly-CSharp.dll``** incluído a la carpeta **``SCPSL_Data/Managed``** de tu instalación del servidor (SSH: **`mv Assembly-CSharp.dll (instalación_del_servidor)/SCPSL_Data/Managed`**).
  - Mueve la carpeta **`EXILED`** a **``~/.config``**. *Nota: Esta carpeta tiene que ir en ``~/.config``, y ***NO*** en ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)

### Instalando plugins
¡Ya está! EXILED debería estar instalado y activado la próxima vez que inicies el servidor. Nota que EXILED por si solo no hace paenas nada, así que asegurate de conseguir plugins nuevos de **[nuestro servidor de Discord](https://discord.gg/PyUkWTg)**
- Para instalar un plugin, simplemente:
  - Descarga un plugin de [*su* página de versiones](https://i.imgur.com/u34wgPD.jpg) (**¡DEBE ser un `.dll`!**)
  - Muevelo a: ``~/.config/EXILED/Plugins`` (si usas tu SSH como root, busca la carpeta `.config` correcta, la cual estará en `/home/(Usuario Servidor SCP)`)

# Configuración
EXILED por si solo tiene algunas configuraciónes.
Todas son generadas automáticamente cuando inicia el servidor, se encuentran en el archivo ``~/.config/EXILED/Configs/(PuertoServidor)-config.yml`` (``%AppData%\EXILED\Configs\(PuertoServidor)-config.yml`` en Windows).

Configs de plugins ***NO*** están en el archivo ``config_gameplay.txt``, en vez de eso, los ajustes de los plugins están puestos en el archivo ``~/.config/EXILED/Configs/(PuertoServidor)-config.yml`` (``%AppData%\EXILED\(PuertoServidor)-config.yml`` en Windows).
Sin embargo, algunos plugins podrían colocar sus configuraciónes en otras carpetas, este archivo simplemente es el sitio de fábrica de EXILED para ellos. Preguntale al creador del plugin en particular si hay algún error.

# Para desarolladores

Si deseas hacer un plugin para EXILED, es bastante fácil. Si prefieres más bién algún tipo de tutorial, visita nuestra página [Primeros Pasos](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md).

Para una guía más comprensible y tutoriales regularmente actualizado, hechale un vistazo a [la página de EXILED](https://exiled-team.github.io/EXILED/articles/install.html).

Asegurate de seguir estas normas antes de publicar un plugin:

 - Tu plugin debe contener una clase que herede de  Exiled.API.Features.Plugin<>, si no, EXILED no cargará el plugin cuando se inicie el servidor.
 - Cuando se inicia un plugin, el código en el método ``OnEnabled()`` de la clase se lanzará inmediatamente, no espera a que otros plugins se carguen. No espera a que se termine de iniciar el servidor. ***No espera por nada.*** Cuando escribas tu método OnEnable(), asegurate de que no intentas acceder a objetos que aún no estén iniciados por el servidor, como ServerConsole.Port, o PlayerManager.localPlayer.
 - Si tienes que acceder a objetos desde el principio, se recomienda que uses el evento WaitingForPlayers para hacerlo, si, por alguna razón, tienes que hacerlo incluso antes, encapsula el código en un bucle ``` while(!x)``` que comprebe que la función/variables que intentas acceder no es *null* antes de que continúe.
 - EXILED puede recargar plugins dinámicamente en medio de ejecución. Esto significa que, si tienes que actualizar un plugin, puedes hacerlo sin reiniciar el servidor. Sin embargo, si haces esto, el plugin debe estar correctamente configuradoo tendrás un *ligero percance*. Hechale un vistazo al apartado de ``Actualizaciónes Dinámicas`` para más información y directrizes para seguir.
 - ***NO*** existe evento OnUpdate, OnFixedUpdate o OnLateUpdate en  EXILED. Si tienes qe (por algún motivo) ejecutar codigo tán seguido, puedes usar una corutina MEC que espere por un frame (0.01f) o usar una rutina de Timing como Timing.FixedUpdate.

### Deshabilitar parches de eventos de EXILED
***Esta función está en desuso y no está implementada.***

### Corutinas Mec
Si no conoces MEC, esta es una guía muy simple para que empiezes.
Corutinas MEC son métodos cronometradosm, los cuales soportan esperar un tiempo predefinido antes de continuar la ejecución, sin interrumpir/dormir el hilo del juego principal.
Corutinas MEC son seguras para usar con Unity, al contrario de hilos tradicionales. ***NO intentes hacer hilos nuevos para interactuar con Unity, Estos SI causarán crash del servidor.***

Para usar MEC, tendrás que añadir ``Assembly-CSharp-firstpass.dll`` a las referencias del servidor, y incluir ``using MEC;``.
Ejemplo de llamar una corutina simple, que se repetirá en bucle despues de un tiempo de espera:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MiCorutina());
}

public IEnumerator<float> MiCorutina()
{
    for (;;) //repetir esto indefinidamente
    {
        Log.Info("Hey, ¡Soy un bucle infinito!"); //Llamar Log.Info para imprimir una linea a los logs del servidor/consola del juego.
        yield return Timing.WaitForSeconds(5f); //Le dice a la corutina que espere 5 segundos antes de continuar, al estar al final de el bucle, efectivamente evita que el bucle se repita por 5 segundos.
    }
}
```

Se recomienda ***fuertemente*** que hagas un poquito de investigación, o preguntes en el Discord si no conoces mucho de MEC y te gustaría aprender mas, obtener consejos o pedir ayuda. Preguntas, no importa lo 'tontas que sean', siempre se responderán de la forma que más ayuda ofrezca y más clara posible para que pos creadores de plugins mejoren sus habilidades. Buen código es bueno para todos.

### Actualizaciónes Dinámicas
EXILED como plataforma soporta recargas dinámicas de plugins sin reiniciar el servidor.
Por ejemplo, si inicias el servidor con el plugin `Exiled.Events` solamente, y deseas añadir otro, no tienes que reiniciar el servidor para hacer esto. Puedes simplemente iniciar el RemoteAdmin/ConsolaDeServidor junto al comando `reload plugins` para recargar todos los plugins de EXILED, incluyendo otros que no estaban cargados previamente.

Eso también significa que se pueden *actualizar* plugins sin reinicar el servidor completamente. Sin embargo, hay algunas reglas que seguir para que esto funcione correctamente:

***Para Hosts***
 - Si actualizas un plugin, asegurate que el nombre de asamblea no corresponda con la versión instalada (si hay una). El plugin hecho con Actualizaciónes Dinámicas en mente tendrá que hacer esto, cambiar el nombre no funciona.
 - Si el plugin soporta Actualizaciónes Dinámicas, asegurate de eliminar la versión posterior del plugin antes de recargar todos los plugins de EXILED, si no te aseguras de esto, podría haber efectos secundarios indeseados.
 - Cualquier problema que sea causa de las Actualizciónes Dinámicas es responsabilidad exclusivamente de el creador del plugin y de tí mismo. Miéntras que EXILED soporta completamente (y recomienda) el udo de las Actualizaciónes Dinámicas, la única forma de la que podría fallar es si el admin del servidor o el desarollador del plugin hiciera algo mal. Comprueba tres veces de que todo está correcto por los dos lados antes de reportar un bug a los desarolladores de EXILED acerca de las Actualizaciónes Dimámicas.

 ***Para Desarolladores***

 - Plugins que quieran soportar las Actualizaciónes Dinámicas se tienen que asegurar de desubscribirse de todo los eventos cuando es Deshabilitado (Disabled) o Recargado (Reloaded).
 - Plugins que tengan parches propios de Harmony deben de usar algún tipo de variable que cambia adentro del nombre de la Instancia de Harmony, y deben ejecutar UnPatchAll() en su Instancia de Harmony cuando el plugin es deshabilitado o recargado.
 - Cualquier corutina que se inicie en el método OnEnabled también deberá ser detenido cuando el plugin se deshabilite o recargue.

Todos estos pueden ser logrados usando los metodos OnReloaded() o OnDisabled() en la clase Plugin. Cuando EXILED recarga plugins, se llama a OnDisabled() primero, despues a OnReloaded(), despues carga todas las asambleas nuevas, y despues ejecuta OnEnabled().

Fíjate que dije asambleas *nuevas*. Si reemplazas un asamblea con otra con el mismo nombre, ***NO*** será actualizado. Esto es debido al GAC (Global Assembly Cache o Caché de Asamblea Global), si intentas 'cargar' una asamblea que ya está en caché, cargará la del caché en vez de la nueva.
Por esta razón, si tu plugin soporta las Actualizaciónes Dinámicas, debes crear cada versión con un Nombre de Asamblea distinto en las opciónes de compilación (renombrar el archivo no funcionará). Además, ya que la asamblea antigua no es "destruida" cuando ya no se necesita, si no te desubscribes de los eventos, desparchear tu instancia de Harmony, detener corutinas, etc... Ese código seguirá ejecutandose al igual que el código de la nueva versión. 
Esto no es una buena idea.

Es por eso que los plugins que soportan las Actualizaciónes Dinámicas ***DEBEN*** seguir estas guías o serán eliminadas del servidor de Discord para evitar riesgos para los administradores de servidores.

Pero no es obligatorio que todos los plugins soporten las Actualizaciónes Dinámicas. Si no quieres, está bién, simplemente no cambies el Nombre de Asamblea de tu plugin cuando hagas una versión nueva y no tendrás que preocuparte por nada de esto, simplemente asegurate de que el administrador del servidor sepa que tiene que reiniciar completamente el servidor para actualizar tu plugin.
