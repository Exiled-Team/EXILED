<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/exiledreboot">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>

</div>

EXILED는 SCP: 비밀 연구소 서버들을 위한 고급 플러그인 프레임워크입니다. 개발자가 게임의 코드를 바꾸거나 자신만의 기능을 넣을 수 있는 이벤트 시스템을 제공하며, 모든 EXILED 이벤트들은 Harmony로 작성되어 직접적으로 서버의 어셈블리를 바꿀 필요가 없어, 2가지의 장점을 만들어냅니다.

- 첫째로, 모든 프레임워크의 코드는 자유롭게 공유되거나 배포될 수 있어, 개발자들이 _어떻게_ 프레임워크가 작동하고 기능을 추가하고나 바꾸는 방법을 배울 수 있습니다.
- 둘째로, 프레임워크가 관련된 모든 코드가 서버 어셈블리 밖에서 작동되기 때문에, 만약 작은 업데이트가 존재하더라도, 프레임워크에는 아주 작은 영향을 미치게 됩니다. 이로 하여금 계속되는 업데이트와의 호환성이 (거의) 보장되며, 업데이트를 _필요할 때_ 할 수 있습니다.

# 번역본

- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Italiano](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-IT.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)
- [Türkçe](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-TR.md)
- [German](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DE.md)
- [Français](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-FR.md)
- [한국어](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-KR.md)

# EXILED 설치하기

EXILED의 설치 과정은 상당히 간단합니다. 노스우드의 Plugin API을 이용해 프레임워크를 불러오며, 이는 릴리즈 파일 `Exiled.tar.gz` 안에 2개의 폴더가 있는 이유가 됩니다. `SCP Secret Laboratory` 폴더는 `EXILED` 폴더에 있는 EXILED의 모든 기능을 불러오는데 필요한 파일들을 담고 있습니다. 이제 이 두 폴더를 (아래에 설명된) 위치로 옮기면, 짜잔! EXILED 설치 완료!

만약 설치 프로그램을 사용한다면, 제대로 실행할 시 모든 EXILED의 기능을 자동으로 설치할 것입니다.

# Windows

### 설치 프로그램 사용 ([설치 프로그램 설명 (영어)](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**주의사항**: 설치 프로그램을 사용하기 전에, SCP: 비밀 연구소 서버를 실행할 유저와 같은 유저인지, 또는 관리자 권한을 가진 유저인지 확인해주세요.

- **[여기에서](https://github.com/Exiled-Team/EXILED/releases) `Exiled.Installer-Win.exe`** 를 다운로드합니다. (Assets 클릭 -> Exiled.Installer-Win.exe 클릭)
- 서버와 같은 폴더에 프로그램을 위치시킵니다. (서버를 다운로드 받지 않았다면 설치하세요.)
- **`Exiled.Installer.exe`** 를 실행하거나 프리릴리즈 버전을 설치하려면 **이 [.bat 파일](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)을 받아** 서버 폴더에 넣고 실행합니다.
- 플러그인을 다운로드하고 설치하려면, 아래 [플러그인 설치](#플러그인-설치) 문단을 참고하세요.
  **주의사항:** EXILED를 원격 서버에 설치하려고 한다면, .exe 파일을 실행할 때 SCP: 비밀 연구소 서버를 실행할 유저와 같은 유저인지, 또는 관리자 권한을 가진 유저인지 확인해주세요.

### 수동 설치

- **`Exiled.tar.gz`를 [여기서](https://github.com/Exiled-Team/EXILED/releases)** 다운로드합니다.
- 안의 내용을 [7Zip](https://www.7-zip.org/) 이나 [WinRar](https://www.win-rar.com/download.html?&L=6)로 압축을 풀어주세요.
- **`EXILED`** 폴더를 **`%appdata%`** 로 옮깁니다. \*주의사항: 이 폴더는 `C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory`**_가 아닌_** `C:\Users\%UserName%\AppData\Roaming`에 위치해야만 하며, `(...)\AppData` 가 아닌, **무조건** `(...)\AppData\Roaming`에 위치해야 합니다.\*
- **`SCP Secret Laboratory`** 폴더를 **`%appdata%`** 로 옮기세요.
  - Windows 10 & 11:
    `%appdata%`를 Windows 검색 창에 입력하거나, Windows 탐색기 상단 바에 입력하세요.
  - 다른 Windows 버전:
    `Win + R`을 누르고 `%appdata%`를 입력하세요.

### 플러그인 설치

자, 이제 모든 설치가 완료되었습니다! EXILED는 이제 설치되었고 다음 서버 구동시 활성화 될 것입니다. EXILED 자체로는 아무것도 하지 않습니다. **[저희의 디스코드 서버](https://discord.gg/PyUkWTg)** 에서 새롭고 재미있는 플러그인을 찾아보세요!

- 플러그인을 설치하려면:
  - [_플러그인의_ 릴리즈 페이지](https://i.imgur.com/u34wgPD.jpg)에서 플러그인을 다운로드하세요. (**무조건 `.dll` 파일 형식이어야 합니다!**)
  - 그리고 파일을 옮기세요! `C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins` 로 .dll 파일을 옮기세요. (Win + R을 누르고, `%appdata%`을 적는 것으로 이동할 수 있습니다.)

# 리눅스

### 설치 프로그램 사용 ([설치 프로그램 설명 (영어)](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**주의사항**: 설치 프로그램을 사용하기 전에, SCP: 비밀 연구소 서버를 실행할 유저와 같은 유저인지, 또는 `root` 유저인지 확인해주세요.

- **`Exiled.Installer-Linux`** 를 **[여기서](https://github.com/Exiled-Team/EXILED/releases)** 다운로드 하세요. (Assets 클릭 -> 설치 프로그램 다운로드)
- **`./Exiled.Installer-Linux --path /path/to/server`** 를 입력해 설치하거나, 파일을 서버 폴더 안에 위치시키고 터미널을 이동시킨 다음 (`cd`) **`./Exiled.Installer-Linux`** 를 입력하세요.
- 프리릴리즈 버전을 원한다면, 실행 시 **`--pre-releases`** 를 추가하세요. 예시: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
- 또 다른 예시: 만약 `Exiled.Installer-Linux`를 서버 폴더 안에 위치시켰다면: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`** 를 입력하세요.
- 플러그인을 다운로드하고 설치하려면 [플러그인 설치](#플러그인-설치-1) 문단을 참고해주세요.

### 수동 설치

- SCP 서버를 실행하는 유저와 같은지 **확인하세요.**
- **`Exiled.tar.gz`** 를 **[여기서](https://github.com/Exiled-Team/EXILED/releases)** 다운로드하세요. (SSH: 우클릭으로 `Exiled.tar.gz` 링크를 복사하고, **`wget (다운로드-링크)`** 를 입력하세요.)
- 현재 폴더로 압축을 풀려면, **`tar -xzvf EXILED.tar.gz`** 를 입력하세요.
- **`EXILED`** 폴더를 **`~/.config`** 로 위치시키세요. \*주의사항: 이 폴더는 `~/.config/SCP Secret Laboratory` **_가 아닌_** `~/.config`에 위치해야 합니다. (SSH: **`mv EXILED ~/.config/`**)
- **`SCP Secret Laboratory`** 폴더를 **`~/.config`** 에 위치시키세요. \*주의사항: 이 폴더는 `~/.config/SCP Secret Laboratory` **_가 아닌_** `~/.config`에 위치해야 합니다. (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### 플러그인 설치

자, 이제 모든 설치가 완료되었습니다! EXILED는 이제 설치되었고 다음 서버 구동시 활성화 될 것입니다. EXILED 자체로는 아무것도 하지 않습니다. **[저희의 디스코드 서버](https://discord.gg/PyUkWTg)** 에서 새롭고 재미있는 플러그인을 찾아보세요!

- 플러그인을 설치하려면:
  - [_플러그인의_ 릴리즈 페이지](https://i.imgur.com/u34wgPD.jpg)에서 플러그인을 다운로드하세요. (**무조건 `.dll` 파일 형식이어야 합니다!**)
  - 그리고 파일을 옮기세요!
    `~/.config/EXILED/Plugins` 로 .dll 파일을 옮기세요. (만약 SSH를 root로 사용한다면, `/home/(SCP 서버 유저)` 안에 있는 올바른 `.config`를 찾아보세요.)

# 설정

EXILED 자체도 몇 가지 설정을 가지고 있습니다. 이 설정들은 서버 시작 시 자동으로 생성되며, `~/.config/EXILED/Configs/(서버-포트)-config.yml` 파일에 위치해 있습니다. (Windows의 경우 `%AppData%\EXILED\Configs\(서버포트)-config.yml` 입니다.)

플러그인 설정은 **_위의_** `config_gameplay.txt` 파일에 위치해 있지 않습니다. 대신, 플러그인 설정은 `~/.config/EXILED/Configs/(서버-포트)-config.yml` 파일에 위치해 있습니다. (Windows의 경우 `%AppData%\EXILED\Configs\(서버포트)-config.yml` 입니다.)
하지만, 몇몇 플러그인은 아마도 자신만의 설정 파일을 가지고 있을 수 있습니다. 이 경우, 플러그인의 설명서를 참고하세요.

# 개발자들을 위한 정보

만약 EXILED 플러그인을 만들고 싶다면, 사실 간단합니다. 튜토리얼을 원한다면 [여기 (영어)](https://github.com/Exiled-Team/EXILED/blob/master/GettingStarted.md)를 읽어주세요.

좀 더 상세하고 자주 업데이트 되는 튜토리얼을 원한다면, [EXILED 웹사이트](https://exiled.to)를 방문해보세요.

하지만 플러그인을 만들기 전에, 몇 가지 주의사항을 알아두세요:

- 당신의 플러그인은 **무조건** `Exiled.API.Features.Plugin<>`을 상속받는 클래스를 가지고 있어야 합니다. 없다면 EXILED 실행 시 플러그인이 작동되지 않습니다.
- 플러그인이 실행되면, `OnEnabled()` 메서드가 **바로** 실행됩니다. 다른 플러그인이 불러와질 때까지 기다리지도 않고, 서버 시작 절차가 끝날 때가지 기다리지도 않습니다. **_아무것도 기다리지 않아요._** `OnEnabled()` 메소드을 구현할 때, 아직 설정이 되지 않은 것들, 예를 들어 `ServerConsole.Port`, 혹은 `PlayerManager.localPlayer` 을 사용하고 있지는 않은지 확인하세요.
- 만약 아직 설정이 되지 않은 것들을 빨리 사용하고 싶다면, `WaitingForPlayers` 이벤트를 기다리거나, 더 빨리, 최대한 빨리 사용하고 싶다면 코드를 `while(!x)` 안에 접근하고 싶은 것이 `null`이 아닌지 확인하는 코드와 함께 넣어주세요.
- EXILED는 플러그인 어셈블리들을 서버 동작 중 동적으로 다시 불러올 수 있습니다. 플러그인을 업데이트하고 싶다면, 서버를 재시작할 필요가 없습니다. 하지만, 플러그인을 다시 불러온다면, 플러그인이 리로드를 지원하는지 확인하세요. 지원하지 않은 상태로 리로드를 시도하면, 아주, 매우, 안 좋은 시간을 보내게 될 겁니다. `동적 리로드` 문단을 읽어 보세요.
- OnUpdate, OnFixedUpdate 이나 OnLateUpdate는 EXILED에 **_존재하지 않습니다_**. 코드를 그 정도로 많이 실행시키고 싶다면, MEC 코루틴을 사용하여 1 프레임, 0.01초 기다리는 코루틴을 사용하거나 Timing 레이어 (Timing.FixedUpdate 등) 를 사용하세요.

### MEC 코루틴

만약 MEC과 익숙치 않다면, 괜찮아요! 이 문단은 아주 간단한 당신의 MEC의 시작점이 될 것입니다.
MEC 코루틴은 다시 말해서 그냥 기다릴 수 있는 메소드입니다.
코드를 실행하기 전에, SCP: 비밀 연구소 게임의 기본 스레드를 건드리지 않고, 몇 초/분 ~~(또는 아마도 시간)~~ 을 기다릴 수 있습니다. MEC 코루틴은 일반적인 스레딩과 달리 유니티와 사용할 때 안전합니다. **_유니티와 상호작용하려고 새로운 스레드를 만들지 마십시오. 이 행동은 서버를 터뜨릴 것입니다._**

MEC을 사용하려면, `Assembly-CSharp-firstpass.dll`을 서버 파일로부터 참조하고 `using MEC;`을 코드에 넣어야 합니다.
여기 반복 할 때 딜레이를 주는 간단한 예제가 있습니다:

```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) // 무한 반복
    {
        Log.Info("무⭐한⭐반⭐복!"); // 서버 로그를 생성합니다.
        yield return Timing.WaitForSeconds(5f); //코루틴에게 5초를 기다리라고 지시합니다.
    }
}
```

MEC과 익숙치 않거나, 더 배우고 싶거나, 조언을 좀 받고 싶나, 도움이 필요하다면 구글링을 좀 하거나 디스코드에서 물어보는 것이 **_매우_** 추천됩니다. 질문들은 얼마나 '수준 낮은지' 와 상관없이, 플러그인 개발자분들에게 추진력을 실어주기 위해 항상 최대한 호의적이고 깔끔하게 답변될 것입니다. 더 좋은 코드는 모두에게 좋은 것이니까요!

### 동적 리로드

EXILED는 플러그인 어셈블리를 서버를 재시작하지 않고도 동적으로 재시작 할 수 있는 프레임워크입니다.
예를 들어, 만약 `Exiled.Events` 1개의 플러그인을 서버를 시작하고, 새로운 플러그인을 추가하고 실행하고 싶다면, 서버를 재시작 할 필요가 없습니다. `reload plugins`을 RA나 서버 콘솔에 입력해 모든 EXILED 플러그인-전에 없던 플러그인들도 리로딩할 수 있습니다.

이것은 또한 플러그인을 완전히 서버를 재시작하지 않다고 *업데이트*를 할 수 있다는 것을 의미합니다. 하지만 이를 지원하기 위해, 플러그인 개발자들이 따라야 할 몇 가지 규칙이 있습니다.

**_서버 주인은_**

- 플러그인을 업데이트하고 있다면, 플러그인의 어셈블리 이름이 현재와 다른지 확인하세요. 플러그인 개발자가 동적 리로딩을 염두에 두고 플러그인을 개발해야 했으며, 파일 이름을 바꾸는 것 만으로는 충분하지 않습니다.
- 만약 플러그인이 동적 리로딩을 지원한다면, 새로운 플러그인을 "Plugins" 폴더에 넣을 때, 전에 있던 플러그인을 먼저 지워야 합니다. 그렇지 않으면, 안 좋은 일이 많이 일어날 수 있습니다.
- 플러그인을 동적으로 리로딩하는데 생기는 모든 책임은 모두 플러그인 개발자 혹은 서버 주인에게 있습니다. EXILED가 동적 리로드를 지원하는데, 문제가 생기는 방법은 플러그인 개발자나 서버 주인이 무언가를 잘못하는 방법 밖에는 없습니다. EXILED 개발자들에게 동적 리로드로 버그를 신고하기 전에 둘 모두에게 모든 것이 제대로 동작되었는지 확인하세요.

**_개발자는_**

- 동적 리로딩을 지원하는 플러그인을 만들 때, 플러그인이 걸려있던 모든 이벤트들을 비활성화되거나 리로딩 할 때 해제되어야 합니다.
- Harmony 패치가 있는 플러그인이라면 Harmony 인스턴스를 만들 때 이름을 일종의 항상 바뀌는 변수로 만들어야 합니다. 또한 플러그인이 비활성화되거나 리로딩 될 때 Harmony 패치를 `UnPatchAll()` 메소드로 해제해야 합니다.
- `OnEnabled()` 메소드에서 시작된 모든 코루틴들은 플러그인이 비활성화되거나 리로딩 될 때 중지되어야 합니다.

위의 모든 것들은 `OnReloaded()` 또는 `OnDisabled()` 메소드에서 수행될 수 있습니다. EXILED가 플러그인을 리로드할 때, `OnDisabled()`가 호출되고 `OnReloaded()`가 호출됩니다. 그리고 새로운 어셈블리를 로드하고 `OnEnabled()`가 호출됩니다.

_새로운_ 어셈블리들이라는 것에 주목해주세요. 만약 똑같은 이름을 가진 어셈블리로 바꾼다면, 업데이트되지 **_않을 것입니다_**. 이것은 GAC (Global Assembly Cache) 때문이며, 이미 캐시에 있는 어셈블리를 '불러오려고' 시도할 때, 이미 캐시에 있던 어셈블리를 불러 올 것입니다.
이러한 이유로, 만약 당신의 플러그인이 동적 리로딩을 지원한다면, 빌드 옵션에서 다른 어셈블리 이름을 가지고 빌드해야 합니다. (파일 이름을 바꾸는 것은 동작하지 않습니다.) 또한, 원래의 어셈블리가 필요하지 않을 때 "제거되지" 않으므로 만약 이벤트 해제, Harmony 인스턴스 언패칭, 코루틴 중지 등을 하지 않는다면 새로운 코드와 함께 오래된 코드가 같이 실행될 것입니다. 별로 좋은 일이 아니에요.

이 때문에, 동적 리로딩을 지원하는 플러그인을 만들 때, **_무조건_** 이러한 규칙을 따라야 합니다. 만약 이러한 규칙을 따르지 않는다면, 디스코드 서버에서 삭제될 수 있습니다.

모든 플러그인이 동적 리로딩을 지원하지 않아도 됩니다. 만약 동적 리로딩을 지원하지 않기로 생각했다면, 진짜 괜찮아요. :)
플러그인의 어셈블리 버전을 바꾸는 것을 피하시고, 서버 주인분들에게 플러그인을 업데이트하기 위해 서버를 재시작하라고 요청하세요.
    
