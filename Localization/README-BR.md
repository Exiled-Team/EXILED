<div align="center">
  <img src="../assets/logo.svg" alt="Logo" width="96px" />
</div>
<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
  <a href="https://github.com/Exiled-Team/EXILED/releases/latest" target="_blank">
    <img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="Build" />
  </a>
  <a href="https://github.com/Exiled-Team/EXILED/releases" target="_blank">
    <img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" alt="Releases" />
  </a>
  <a href="https://github.com/Exiled-Team/EXILED/releases/latest" target="_blank">
    <img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads" />
  </a>
  <a href="https://github.com/Exiled-Team/EXILED/commits/dev" target="_blank">
    <img src="https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/dev?style=for-the-badge&logo=git" alt="Commits" />
  </a>
  <a href="https://discord.gg/exiledreboot" target="_blank">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Discord" />
  </a>
</div>
<br />

O EXILED é uma estrutura para plug-ins de alto nível aos servidores de SCP: Secret Laboratory. Ele oferece um sistema de eventos para os desenvolvedores usarem com o intuito de manipular, alterar o código do jogo ou implementar suas próprias funções.
Todos os eventos do EXILED são codificados com Harmony, o que significa que não requerem edição direta dos Assemblies do servidor para funcionar, o que permite dois benefícios exclusivos.

 - Em primeiro lugar, todo o código da estrutura pode ser publicado e compartilhado livremente, permitindo que os desenvolvedores entendam melhor *como* ele funciona, além de oferecer sugestões para adicionar ou alterar suas funções.
 - Em segundo lugar, como todo o código relacionado à estrutura é feito fora da Assembly do servidor, coisas como pequenas atualizações do jogo terão pouco ou nenhum efeito na framework, tornando-a mais compatível com futuras atualizações do jogo, além de facilitar a atualização quando *for* necessário fazê-la.

# READMEs Localizadas
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Italiano](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-IT.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)
- [Türkçe](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-TR.md)
- [German](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DE.md)
- [Français](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-FR.md)
- [한국어](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-KR.md)
- [ไทย](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ไทย.md)

# Instalação
A instalação do EXILED é bastante simples. Ele se carrega por meio da API de plug-in da NW. É por isso que existem duas pastas dentro de ``Exiled.tar.gz`` nos arquivos de lançamento. ``SCP Secret Laboratory`` contém os arquivos necessários para carregar os recursos do EXILED na pasta ``EXILED``. Com isso dito, tudo o que você precisa fazer é mover essas duas pastas para o caminho adequado que é explicado abaixo, e pronto!

Se você optar por usar o instalador, se executado corretamente, ele cuidará de instalar todos os recursos do EXILED.

# Windows
### Instalação automática ([mais informações](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Nota**: Verifique se você está conectado ao usuário que executa o servidor ou se possui privilégios de administrador antes de executar o Instalador.

  - Baixe o **`Exiled.Installer-Win.exe` [daqui](https://github.com/Exiled-Team/EXILED/releases)** (clique em Assets -> clique no Instalador)
  - Coloque-o na pasta do seu servidor (baixe o servidor dedicado, caso não o tenha feito)
  - Clique duas vezes em **`Exiled.Installer.exe`** ou **[baixe este .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** e coloque-o na pasta do servidor para instalar o pré-lançamento mais recente
  - Para instalar e obter plug-ins, confira a seção [Instalando plug-ins](#installing-plugins) abaixo.
**Nota:** Se você estiver instalando o EXILED em um servidor remoto, certifique-se de executar o .exe como o mesmo usuário que executa seus servidores de SCP:SL (ou um com privilégios de administrador)

### Instalação manual
  - Baixe o **`Exiled.tar.gz` [daqui](https://github.com/Exiled-Team/EXILED/releases)**
  - Extraia seus conteúdos com [7Zip](https://www.7-zip.org/) ou [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Mova a pasta **``EXILED``** para **`%appdata%`** *Note: Esta pasta precisa ir ao diretório ``C:\Users\%NomeDoUsuário%\AppData\Roaming``, e ***NÃO*** ao ``C:\Users\%NomeDoUsuário%\AppData\Roaming\SCP Secret Laboratory``, e **DEVE** estar em (...)\AppData\Roaming, não (...)\AppData\!*
  - Mova **``SCP Secret Laboratory``** para **`%appdata%`**.
    - Windows 10 e 11:
      Escreva `%appdata%` na Cortana / no ícone de pesquisa ou na barra do Windows Explorer
    - Qualquer outra versão do Windows:
      Pressione Win + R e digite `%appdata%`

### Instalando plug-ins
É isso, o EXILED agora deve estar instalado e ativo na próxima vez que você inicializar seu servidor. Observe que o EXILED sozinho não fará quase nada, portanto, certifique-se de obter novos plug-ins no **[nosso servidor do Discord](https://discord.gg/exiledreboot)**
- Para instalar um plug-in, basta:
  - Baixar um plug-in da [página de lançamentos *deles*](https://i.imgur.com/u34wgPD.jpg) (**DEVE ser um `.dll`!**)
  - Mova-o para: ``C:\Users\%NomeDoUsuário%\AppData\Roaming\EXILED\Plugins`` (mova-se para cá pressionando Win + R e, em seguida, escrevendo `%appdata%`)

# Linux
### Instalação automática ([mais informações](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Nota:** Se você estiver instalando o EXILED em um servidor remoto, certifique-se de executar o instalador como o mesmo usuário que executa seus servidores de SCP:SL (ou root)

  - Baixe o **`Exiled.Installer-Linux` [daqui](https://github.com/Exiled-Team/EXILED/releases)** (clique em Assets -> baixe o Instalador)
  - Instale-o digitando **`./Exiled.Installer-Linux --path /path/to/server`** ou mova-o diretamente para dentro da pasta do servidor, mova para ele com o terminal(`cd`) e digite: **`./Exiled.Installer-Linux`**.
  - Se você quiser o último pré-lançamento, simplesmente adicione **`--pre-releases`**. Exemplo: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Outro exemplo, se você colocou `Exiled.Installer-Linux` na pasta do seu servidor: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - Para instalar e obter plug-ins, confira a seção [Instalando plug-ins](#installing-plugins-1) abaixo.

### Instalação manual
  - **Tenha certeza** de que você está conectado ao usuário que executa os servidores de SCP.
  - Baixe o **`Exiled.tar.gz` [daqui](https://github.com/Exiled-Team/EXILED/releases)** (SSH: clique com o botão direito do mouse para receber o link do `Exiled.tar.gz` e então digite: **`wget (link_para_baixar)`**)
  - Para extraí-lo à sua pasta atual, digite **``tar -xzvf EXILED.tar.gz``**
  - Mova a pasta **`EXILED`** para **``~/.config``**. *Nota: Esta pasta precisa ir ao diretório ``~/.config``, e ***NÃO*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Mova a pasta **`SCP Secret Laboratory`** para **``~/.config``**. *Nota: Esta pasta precisa ir ao diretório ``~/.config``, e ***NÃO*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Instalando plug-ins
É isso, o EXILED agora deve estar instalado e ativo na próxima vez que você inicializar seu servidor. Observe que o EXILED sozinho não fará quase nada, portanto, certifique-se de obter novos plug-ins no **[nosso servidor do Discord](https://discord.gg/exiledreboot)**
- Para instalar um plug-in, basta:
  - Baixar um plug-in da [página de lançamento *deles*](https://i.imgur.com/u34wgPD.jpg) (**DEVE ser um `.dll`!**)
  - Mova-o para: ``~/.config/EXILED/Plugins`` (se você utiliza SSH como root, então procure pela `.config` correta, que estará dentro de `/home/(Usuário do Servidor de SCP)`)

# Configuração
O EXILED por si só oferece algumas opções de configuração.
Todas elas são geradas automaticamente na inicialização do servidor e estão localizadas no arquivo ``~/.config/EXILED/Configs/(PortaDoServidorAqui)-config.yml`` (``%AppData%\EXILED\Configs\(PortaDoServidorAqui)-config.yml`` no Windows).

As configurações do plug-in ***NÃO*** estarão no arquivo ``config_gameplay.txt`` supracitado, em vez disso, as configurações do plug-in são definidas no arquivo ``~/.config/EXILED/Configs/(PortaDoServidor)-config.yml`` (``%AppData%\EXILED\(PortaDoServidor)-config.yml`` no Windows).
No entanto, alguns plug-ins podem obter suas configurações de outros locais por conta própria. Esta é simplesmente a localização padrão do EXILED para eles, portanto, consulte o criador do plug-in se houver problemas.

# Para Desenvolvedores

Se você deseja fazer um plug-in ao EXILED, é bem simples de fazê-lo. Se você quiser ver algum tipo de tutorial, visite nosso [Manual de Instruções.](https://exiled.to/Archive/GettingStarted)

Para tutoriais mais abrangentes e ativamente atualizados, consulte [o site da EXILED](https://exiled.to).

Mas certifique-se de seguir estas regras ao publicar seus plug-ins:

 - Seu plug-in deve conter uma classe herdada de ``Exiled.API.Features.Plugin<>``, caso contrário, o EXILED não carregará seu plug-in quando o servidor iniciar.
 - Quando um plug-in é carregado, o código dentro do método ``OnEnabled()`` da classe supracitada é acionado imediatamente, ele não espera que outros plug-ins sejam carregados. Ele não espera a conclusão do processo de inicialização do servidor. ***Ele não espera por nada.*** Ao configurar seu método ``OnEnable()``, certifique-se de não estar acessando coisas que ainda não foram inicializadas pelo servidor, como ``ServerConsole.Port``, ou ``PlayerManager.localPlayer``.
 - Se você precisar acessar coisas que não foram inicializadas antes do carregamento do plug-in, é recomendável simplesmente aguardar o evento ``WaitingForPlayers`` para fazê-lo, se por algum motivo precisar fazer as coisas antes, envolva o código em um loop ``` while(!x)``` que verifica se a variável/objeto que você precisa não é mais *null* antes de continuar.
 - O EXILED suporta o recarregamento dinâmico de Assemblies de plug-ins no meio da execução. Isso significa que, se você precisar atualizar um plug-in, isso pode ser feito sem reiniciar o servidor, no entanto, se você estiver atualizando um plug-in no meio da execução, o plug-in precisa ser configurado corretamente para suportá-lo, ou você terá um sério problema. Consulte a seção ``Atualizações Dinâmicas`` para mais informações e orientações a seguir.
 - **NÃO** há evento OnUpdate, OnFixedUpdate ou OnLateUpdate no EXILED. Se você precisar, por algum motivo, executar o código com frequência, poderá usar uma corrotina MEC que espera por um quadro, 0.01f, ou usar uma camada de Timing como Timing.FixedUpdate.

### Desativando patches de evento do EXILED
***Atualmente, esta função não está mais implementada.***

### Corrotinas MEC
Se você não estiver familiarizado com o MEC, este será um guia muito breve e simples para você começar.
Corrotinas MEC são basicamente métodos cronometrados que suportam períodos de espera antes de continuar a execução, sem interromper/suspender o alinhamento (thread) principal do jogo.
As corrotinas MEC são seguras para usar com o Unity, ao contrário do alinhamento tradicional. ***NÃO tente criar novos alinhamentos para interagir com o Unity, eles VÃO travar o servidor.***

Para usar o MEC, você precisará referenciar ``Assembly-CSharp-firstpass.dll`` dos arquivos do servidor e incluir ``using MEC;``.
Exemplo de chamada de uma corrotina simples, que se repete com um atraso entre cada ciclo:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //Repete o evento seguinte por tempo indefinido
    {
        Log.Info("Ei, eu sou um ciclo infinito!"); //Designar Log.Info para reproduzir uma linha nos registros do console/servidor do jogo.
        yield return Timing.WaitForSeconds(5f); //Diz à corrotina para esperar 5 segundos antes de continuar, e quando está no final do ciclo, efetivamente interrompe a repetição do ciclo por 5 segundos.
    }
}
```

É **altamente** recomendável que você pesquise no Google ou pergunte no Discord se não estiver familiarizado com o MEC e quiser aprender mais, obter conselhos ou precisar de ajuda. As perguntas, não importa o quão 'estúpidas' sejam, sempre serão respondidas da maneira mais útil e clara possível para que os desenvolvedores de plug-ins se destaquem. Um bom código é melhor para todos.

### Atualizações Dinâmicas
O EXILED como uma estrutura suporta o recarregamento dinâmico de Assemblies de plug-ins sem exigir uma reinicialização do servidor.
Por exemplo, se você iniciar o servidor apenas com `Exiled.Events` como o único plug-in e desejar adicionar um novo, não será necessário reiniciar o servidor para concluir esta tarefa. Você pode simplesmente usar o comando do RemoteAdmin/ServerConsole `reload plugins` para recarregar todos os plug-ins do EXILED, incluindo os novos que não foram carregados antes.

Isso também significa que você pode *atualizar* os plug-ins sem precisar reinicializar totalmente o servidor. No entanto, existem algumas diretrizes que devem ser seguidas pelo desenvolvedor do plug-in para que isso seja realizado corretamente:

***Para Hosters***
 - Se você estiver atualizando um plug-in, certifique-se de que o nome do Assembly não seja o mesmo da versão atual que você instalou (se houver uma). O plug-in deve ser construído pelo desenvolvedor com atualizações dinâmicas em mente para que isso funcione, simplesmente renomear o arquivo não basta.
 - Se o plug-in suporta Atualizações Dinâmicas, certifique-se de que, ao colocar a versão mais recente do plug-in na pasta "Plugins", você também remova a versão mais antiga da pasta, antes de recarregar o EXILED; a falha em garantir isso resultará em muitos problemas indesejados.
 - Quaisquer problemas decorrentes da Atualização Dinâmica de um plug-in são de sua exclusiva responsabilidade e do desenvolvedor do plug-in em questão. Embora o EXILED suporte e incentive totalmente as Atualizações Dinâmicas, a única maneira de isso falhar ou dar errado é se o anfitrião do servidor ou o desenvolvedor do plug-in fizer algo errado. Verifique três vezes se tudo foi feito corretamente por ambas as partes antes de relatar um erro aos desenvolvedores da EXILED em relação às Atualizações Dinâmicas.

 ***Para Desenvolvedores***

 - Os plug-ins que desejam oferecer suporte à Atualização Dinâmica precisam cancelar a assinatura de todos os eventos aos quais estão conectados quando são desativados ou recarregados.
 - Os plug-ins que possuem patches personalizados do Harmony devem usar algum tipo de variável mutável no nome da instância do Harmony e devem usar UnPatchAll() em sua instância do Harmony quando o plug-in for desativado ou recarregado.
 - Quaisquer corrotinas iniciadas pelo plug-in em ``OnEnabled()`` também devem ser eliminadas quando o plug-in for desativado ou recarregado.

Tudo isso pode ser realizado nos métodos ``OnReloaded()`` ou ``OnDisabled()`` na classe do plug-in. Quando o EXILED recarrega os plug-ins, ele designa OnDisabled(), então ``OnReloaded()``, então ele carregará nos novos Assemblies, e então executará ``OnEnabled()``.

Observe que eu disse *novos* Assemblies. Se você substituir um Assembly por outro com o mesmo nome, ele ***NÃO*** será atualizado. Isso se deve ao GAC (Global Assembly Cache), se você tentar 'carregar' um Assembly que já está no cache, ele sempre usará o Assembly em cache.
Por esse motivo, se o seu plug-in oferecer suporte a Atualizações Dinâmicas, você deverá criar cada versão com um nome de Assembly diferente nas opções de compilação (renomear o arquivo não funcionará). Além disso, como o Assembly antigo não é "destruído" quando não é mais necessário, se você não cancelar a assinatura de eventos, desfazer o patch de sua instância de Harmony, eliminar corrotinas, etc., esse código continuará a ser executado, bem como o código da nova versão.
Esta é uma péssima, bem péssima situação para se deixar ocorrer.

Como tal, os plug-ins que oferecem suporte a Atualizações Dinâmicas ***DEVEM*** seguir estas diretrizes ou serão removidos do servidor do Discord devido ao risco potencial para os anfitriões de servidor.

Mas nem todo plug-in tem de oferecer suporte a Atualizações Dinâmicas. Se você não pretende oferecer suporte a Atualizações Dinâmicas, tudo bem, simplesmente não altere o nome do Assembly do seu plug-in ao criar uma nova versão e não precisará se preocupar com nada disso, apenas certifique-se de que os anfitriões de servidor saibam que eles precisarão reinicializar completamente seus servidores para atualizar seu plug-in.

**Tradução brasileira feita por**: *Firething*
