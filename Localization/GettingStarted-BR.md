# Documento de Baixo Nível do Exiled
*(Escrito por [KadeDev](https://github.com/KadeDev) para a comunidade) (Traduzido por [Firething](https://github.com/Firething))*

## Manual de Instruções
### Introdução
Exiled é uma API de baixo nível, o que significa que você pode chamar funções do jogo sem precisar de vários bloatwares de API.

Isso permite com que o Exiled atualize-se facilmente, e ele pode ser atualizado antes mesmo da atualização chegar ao jogo.

Isso também permite que desenvolvedores de plug-in não precisem atualizar seus códigos sempre que houver uma atualização do Exiled ou SCP:SL. Na realidade, eles nem precisarão atualizar seus plug-ins!

Esse documento mostrará a você os básicos de como se fazer um Plug-in para o Exiled. A partir daqui, você poderá mostrar ao mundo as coisas criativas que você pode criar com essa framework!

### Exemplo de Plug-in
Um [Exemplo de Plug-in](https://github.com/galaxy119/EXILED/tree/master/Exiled.Example) que é um plug-in simples que mostra eventos e como fazer eles adequadamente. Usar esse exemplo ajudará você a aprender a como usar o Exiled apropriadamente. Há alguns aspectos nesse plug-in que são importantes, falaremos sobre eles.

#### Atualizações Dinâmicas em On Enable + On Disable
Exiled é uma framework que tem um comando de **Reload** que pode ser usado para recarregar todos os plug-ins e obter novos. Isso significa que você deve fazer com que seus plug-ins sejam **Dinamicamente Atualizáveis.** Isso significa que toda variável, evento, corrotina, etc *deve* ser atribuída quando ativada e anulada quando desativada. O método **On Enable** deve ativar todos, e o método **On Disable** deve desativar todos. Mas talvez você esteja se perguntando 'E o **On Reload**'? Essa função tem como objetivo carregar variáveis estáticas para que toda constante estática que você fizer não seja apagada. Então você poderia fazer algo assim:
```csharp
public static int StaticCount = 0;
public int counter = 0;

public override void OnEnable()
{
    counter = StaticCount;
    counter++;
    Info(counter);
}

public override void OnDisable()
{
    counter++;
    Info(counter);
}

public override void OnReload()
{
    StaticCount = counter;
}
```

E o resultado seria:
```bash
# On enable fires
1
# Reload command
# On Disable fires
2
# On Reload fires
# On Enable fires again
3

```
(Claro, excluindo qualquer coisa além das respostas reais)
Sem fazer isso, teria ido apenas para o 1 e então para o 2 novamente.

### Jogadores + Eventos
Agora que terminamos de fazer com que nossos plug-ins sejam **Dinamicamente Atualizáveis**, podemos focar em tentar interagir com jogadores por meio de eventos!

Um evento é bem interessante, ele permite com que o SCP:SL se comunique com o Exiled e depois com o Exiled para todos os plug-ins!

Você pode ouvir os eventos do seu plug-in adicionando isso à parte superior do arquivo de origem do plug-in principal:
```csharp
using EXILED;
```
E então você precisa referenciar o arquivo `Exiled.Events.dll` para que você realmente obtenha eventos.

Para referenciar um evento, nós estaremos utilizando uma nova classe que criamos; denominada "EventHandlers". O gerenciador de eventos não é fornecido por padrão; você deve criá-lo.


Nós podemos referenciá-lo no void OnEnable e OnDisable assim:

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    // Registre a classe de gerenciador de evento. E adicione o evento
    // ao ouvinte de eventos 'EXILED_Events' para que obtenhamos o evento.
    EventHandler = new EventHandlers();
    Player.Verified += EventHandler.PlayerVerified;
}

public override OnDisable()
{
    // Torne-o dinamicamente atualizável.
    // Fazemos isso ao remover o ouvinte para o evento e então anulando o gerenciador de eventos.
    // Esse processo deve ser repetido para cada evento.
    Player.Verified -= EventHandler.PlayerVerified;
    EventHandler = null;
}
```

E na classe EventHandlers, faríamos:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {

    }
}
```
Agora conseguimos nos conectar a um evento de jogador verificado que é executado sempre que um jogador é autenticado após entrar no servidor! É importante destacar que todos eventos têm diferentes argumentos de evento, e cada tipo de argumento de evento tem propriedades diferentes associadas.

O EXILED já fornece uma função de aviso (broadcast), então a usaremos em nosso evento:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        ev.Player.Broadcast(5, "<color=lime>Bem-vindo ao meu servidor maneiro!</color>");
    }
}
```

Como destacado acima, todo evento tem diferentes argumentos. Abaixo há um evento diferente que desliga os portões de Tesla para jogadores da Nine-Tailed Fox.

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    EventHandler = new EventHandlers();
    Player.TriggeringTesla += EventHandler.TriggeringTesla;
}

public override OnDisable()
{
    // Não se esqueça, eventos devem ser desconectados e anulados no metódo Disable.
    Player.TriggeringTesla -= EventHandler.TriggeringTesla;
    EventHandler = null;
}
```

E na classe EventHandlers.

`EventHandlers.cs`
```csharp
public class EventHandlers
{
    public void TriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        // Desativa o evento para jogadores da equipe da Fundação.
        // Isso pode ser feito ao verificar o lado (side) do jogador.
        if (ev.Player.Role.Side == Side.Mtf) {
            // Desative o acionamento da Tesla ao definir o ev.IsTriggerable para 'false'.
            // Jogadores que tiverem uma patente na FTM não irão mais ativar portões de Tesla.
            ev.IsTriggerable = false;
        }
    }
}
```


### Configurações
A maioria dos plug-ins do Exiled contém configurações. As configurações permitem que os gerentes de servidor modifiquem os plug-ins livremente, embora sejam limitadas à configuração que o desenvolvedor do plug-in fornece.

Primeiro crie uma classe `config.cs` e mude a herança do seu plug-in de `Plugin<>` para `Plugin<Config>`

Agora você precisa fazer essa configuração herdar `IConfig`. Após herdar de `IConfig`, adicione uma propriedade para a classe titulada como `IsEnabled` e `Debug`. Sua classe de Configuração agora deve se assemelhar a isso:

```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
```

Você pode adicionar qualquer opção de configuração ali e referenciá-la assim:

`Config.cs`
```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        public string TextThatINeed { get; set; } = "esse é o padrão";
    }
```

`MainClass.cs`
```csharp
   public override OnEnabled()
   {
        Log.Info(Config.TextThatINeed);
   }
```

E parabéns! Você fez o seu primeiro Plug-in para o Exiled! É importante destacar que todos os plug-ins **devem** ter uma configuração IsEnabled. Essa configuração permite que donos de servidor ativem e desativem o plug-in quando quiserem. A configuração IsEnabled será lida pelo carregador do Exiled (seu plug-in não precisa verificar se `IsEnabled == true` ou não.).

### E agora?
Se você quiser mais informações, você deve entrar no nosso [discord!](https://discord.gg/PyUkWTg)

Nós temos um canal de #resources que você pode considerar útil, assim como colaboradores do EXILED e desenvolvedores de plug-in que estariam dispostos a ajudá-lo na criação de seus plug-ins.

Ou você poderia ler sobre todos os eventos que nós temos! Se você deseja verificá-los, veja [aqui!](https://github.com/galaxy119/EXILED/tree/master/Exiled.Events/EventArgs)
