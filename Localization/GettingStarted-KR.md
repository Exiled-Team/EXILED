# Exiled 저수준 문서
*(Written by [KadeDev](https://github.com/KadeDev) for the community) (번역: [Cocoa](https://github.com/Cocoa2219))*

## 시작하기
### 개요
Exiled는 불필요한 부분 없이 게임에서 함수를 직접적으로 호출 할 수 있는 저수준 API입니다.  

이 덕분에 Exiled는 업데이트가 상당히 쉽고, 게다가 게임이 업데이트하기도 전에 Exiled가 업데이트될 수 있습니다. 

또한 이것은 플러그인 개발자들이 SCP:SL 또는 Exiled의 업데이트마다 코드를 바꿀 필요가 없다는 것을 나타냅니다.

이 문서는 Exiled 플러그인을 만들기 위한 가장 기본적인 부분을 설명하고 있습니다. 여기서부터 Exiled와 함께 당신의 창의력으로 무엇이든지 만들어보세요! 

### 플러그인 예제
이 [플러그인 예제](https://github.com/galaxy119/EXILED/tree/master/Exiled.Example)는 이벤트를 사용하는 방법과 기본적인 구조를 설정하는 방법을 잘 나타내고 있습니다. 이것을 통해 Exiled를 제대로 사용하는 방법을 알 수 있으며, 이 플러그인 예제 안에는 몇 가지의 중요한 부분이 있습니다. 한번 볼까요? 

#### OnEnable + OnDisable 동적 업데이트
Exiled는 **Reload** 명령어를 통해 원래 있던 모든 플러그인과 새 플러그인들을 리로드 할 수 있습니다. 이 말은 여러분이 여러분들의 플러그인을 **동적으로 업데이트** 할 수 있게 만들어야 한다는 뜻입니다. 모든 변수, 이벤트, 코루틴 등등은 *무조건* 활성화되었을 때 할당되고 비활성화되었을 때 null이 되어야 합니다. **OnEnable** 메소드는 모든 것들을 활성화시켜야 하고, **OnDisable** 메소드는 모든 것을 비활성화시켜야 합니다. 그러면 **OnReload**는 뭘까요? 정적인 변수들은 리로드 될 때 지워지지 않으므로, 이 메소드는 변수들을 지워지게 놔두고 싶지 않을 때 사용합니다. 예를 들어:
```csharp
// 이 정적 변수는 리로딩될 때 지워지지 않습니다
public static int StaticCount = 0;

// 이 변수는 리로딩될 때 초기화됩니다
public int counter = 0;

// 플러그인 활성화
public override void OnEnable()
{
    // StaticCount에서 저장된 값 가져오기
    counter = StaticCount;
    counter++;
    Info(counter);
}

// 플러그인 비활성화
public override void OnDisable()
{
    counter++;
    Info(counter);
}

// 플러그인 리로드
public override void OnReload()
{
    // counter는 초기화되니 리로딩 되어도 초기화되지 않는 정적 변수에 저장
    StaticCount = counter;
}
```

출력값은: (가독성을 위해 출력만 보이게 간소화했습니다)
```bash
# OnEnable 호출
1
# Reload 명령어 사용
# OnDisable 호출
2
# OnReload 호출
# 다시 OnEnable 호출
3

```

이렇게 하지 않으면 리로딩 될 때 counter가 1에서 2로 갔다가 변수가 초기화됩니다.

### 플레이어 + 이벤트
이제 플러그인을 **동적 업데이트**가 가능하게 하는 작업이 끝났습니다! 이제 플레이어와 이벤트를 통해 상호작용하는 방법을 알아봅시다.

이벤트는 SCP:SL이 Exiled와 통신하고, Exiled가 다시 모든 플러그인과 통신할 수 있게 합니다.

메인 플러그인 소스 코드의 맨 위에 이런 줄을 추가함으로서 이벤트를 처리할 수 있습니다:
```csharp
using EXILED;
```
그리고 실제로 이벤트를 사용하기 위해 `Exiled.Events.dll` 파일을 참조해야 합니다.

이벤트를 사용하기 위해 "EventHandlers"라는 새로운 클래스를 만들겠습니다.

OnEnable과 OnDisable에서 다음과 같이 이벤트를 구독하고 구독 취소할 수 있습니다:

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    // EventHandlers 클래스를 하나 생성하고
    EventHandler = new EventHandlers();
    // 이벤트 신호를 받을 수 있게 이벤트에 메소드를 구독하세요
    Player.Verified += EventHandler.PlayerVerified;
}

public override OnDisable()
{
    // 동적 업데이트가 가능하게 만들기 위해 이벤트에서 구독 취소하고 (모든 이벤트에서 구독 취소해야 합니다)
    Player.Verified -= EventHandler.PlayerVerified;
    // EventHandler를 해제하세요
    EventHandler = null;
}
```

그리고 EventHandlers 클래스에서는:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {

    }
}
```

이제 (플레이어가 서버에 들어오고 나서 인증 후 호출되는) Verified 이벤트가 호출될 때 우리의 코드를 실행할 수 있습니다! 모든 이벤트는 각각의 이벤트 인수가 있고, 각각의 인수는 각각 다른 속성이 있다는 것에 주의하세요. (VerifiedEventArgs 안에는 접속한 플레이어가 담겨있는 것에 반해, HurtingEventArgs 같은 다른 인수는 공격자, 피해자, 데미지 양 등 다른 속성이 있습니다.)

Exiled는 플레이어에게 자막을 띄울 수 있는 함수가 미리 준비되어 있으므로 사용해 봅시다:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        // 자막(또는 힌트)은 유니티 TextMeshPro의 리치 텍스트를 사용할 수 있습니다
        // 자세한 내용은 https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html (영문)을 참고하세요
        ev.Player.Broadcast(5, "<color=lime>제 멋진 서버에 온 것을 환영해요!</color>");
    }
}
```

위에서 설명했듯이, 모든 이벤드는 각각 다른 인수가 있습니다. 아래 코드는 NTF 진영에 한해 테슬라 작동을 중지시키는 코드입니다.

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

    Player.TriggeringTesla -= EventHandler.TriggeringTesla;
    EventHandler = null;
}
```

`EventHandlers.cs`
```csharp
public class EventHandlers
{
    public void TriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        // 만약 플레이어의 팀이 Mtf 진영이라면
        if (ev.Player.Role.Side == Side.Mtf) {
            // 테슬라 트리거를 비활성화 합니다.
            ev.IsTriggerable = false;
        }
    }
}
```


### 설정
Exiled 플러그인의 대부분은 설정이 포함되어 있다는 것입니다. 설정으로 서버 유지관리자가 플러그인을 입맛에 맞게 (물론 플러그인 개발자가 제공하는 부분에 한하여) 바꿀 수 있습니다. 

일단 `Config` 클래스를 만들고, 메인 플러그인 클래의 상속을 `Plugin<>`에서 `Plugin<Config>` 로 바꾸세요.

이제 `Config` 클래스가 `IConfig`으로부터 상속받아야 합니다. `IConfig`을 상속하고 난 후, `IsEnabled`와 `Debug` 2개의 속성을 클래스 안에 추가하세요. 당신의 설정 클래스는 이제 이렇게 보여야 합니다:

```csharp
public class Config : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }
}
```

설정 옵션을 설정 클래스 안에 넣고 다음과 같이 불러올 수 있습니다:

`Config.cs`
```csharp
public class Config : IConfig
{
    public bool IsEnabled { get; set; }
    public bool Debug { get; set; }

    // 개발자가 추가한 설정값
    public string TextThatINeed { get; set; } = "this is the default";
}
```

`MainClass.cs`
```csharp
public override OnEnabled()
{
    // 불러온 설정 출력
    Log.Info(Config.TextThatINeed);
}
```

이제 축하합니다! 첫 Exiled 플러그인을 만드셨군요! 아, 그리고 모든 플러그인은 **무조건** IsEnabled 옵션이 있어야 한다는 것을 꼭 기억하세요. 이 옵션은 서버 유지관리자가 플러그인을 상황에 맞게 활성화하거나 비활성화 할 수 있게 할 때 꼭 필요합니다. IsEnabled 옵션은 Exiled를 불러올 때 로더가 읽을 것입니다 (`IsEnabled == true` 등 IsEnabled가 참인지 조건을 확인하지 않아도 됩니다).

### 다음으론?
더 많은 정보를 얻고 싶으시다면 [디스코드](https://discord.gg/PyUkWTg)에 가입해보세요!

#resources 채널에 들어가 유용한 정보들을 찾을 수 있으며, 여러분의 플러그인 개발을 기꺼이 도와줄 Exiled 기여자분들 또는 플러그인 개발자분들이 준비되어 있습니다.

...아니면 현재 존재하는 이벤트를 모두 읽고 싶으시다면 [여기](https://github.com/galaxy119/EXILED/tree/master/Exiled.Events/EventArgs)를 클릭해주세요!
