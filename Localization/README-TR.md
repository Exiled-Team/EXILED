<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/PyUkWTg">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>    

</div>

EXILED, SCP: Secret Laboratory sunucuları için yüksek düzeyde bir Framework yani bir eklenti çerçevesidir. Geliştiricilere oyun kodunu değiştirmek veya kendi fonksiyonlarını eklemek için kullanabilecekleri bir olay sistemi sunar. Tüm EXILED eventleri(olayları) Harmony kullanılarak oluşturulmuştur, bu da demek oluyor ki eventlerin(olayların) işlevsel olabilmesi için doğrudan sunucu kodunu değiştirmenize gerek yoktur ve bu durum 2 avantaj sağlar:

 - İlk olarak. Framework (Yazılım iskeleti)'nin özgürce yayımlanabilir ve dağıtalabilir, buda geliştiricilere nasıl çalıştığını daha iyi anlama imkanını sunar ve ek olarak fonksiyonları ekleme, değiştirme yapmalarına olanak tanır.
 - İkinci olarak, Framework (Yazılım iskeleti) tüm kodun, sunucu kodunun dışına çıktığı için küçük oyun güncellemeleri Framework (Yazılım iskeleti)'ne çok az etki yapar, ve eğer gerek varsa güncelleme yapılması kolaylaşır.


# İndirme
EXILED'i indirmek oldukça kolaydır. EXILED kendini Northwood'un Plugin API'si üzerinden yükler, bu nedenle ``Exiled.tar.gz`` dosyasının içinde iki klasör bulunmaktadır. ``SCP Secret Laboratory`` dosyasının içinde, ``EXILED`` klasöründeki eklentileri yüklemek için gerekli dosyalar bulunur. Yapmanız gereken tek şey bu iki dosyayı doğru konuma yerleştirmektir. Bu nasıl yapılacağı aşağıda belirtilmiştir.

Eğer kurulum programını kullanmayı seçerseniz ve doğru bir şekilde çalıştırırsanız, bütün EXILED özelliklerini yüklemekle ilgili işlemleri otomatik olarak gerçekleştirir.

# Windows
### Otomatik indirme ([Daha fazla bilgi için bana tıkla](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Not**: Sunucuyu indirdiğin kullanıcıda olduğundan emin ol veya yönetici olduğundan emin ol.

  - **`Exiled.Installer-Win.exe`'i [Buradan indir](https://github.com/Exiled-Team/EXILED/releases)** (Assets'e tıkla -> Installer'ı indir)
  - İndirdiğinizde sunucu klasörünüze yerleştirin. (Eğer indirmediyseniz, sunucuyu indirin.)
  - **`Exiled.Installer.exe`**'e iki kere tıkla ve aç veya **[bana tıklayarak .bat'ı indir](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** En son ön yayınını yüklemek için sunucu klasörünün içine koy.
  - Eklenti indirmek için [Installing plugins](#Eklenti-İndirme) Kısmına göz gezdir.
**Not:** eğer EXILED'i uzaktan bağlantılı olan bir sunucuya indiriyor iseniz .exeyi Sunucu açtığınız kullanıcı ile aynı olduğundan emin oluyon veya Yönetici izinleri verin.

### Manuel indirme
  -  **`Exiled.tar.gz` ['yi buradan indir](https://github.com/Exiled-Team/EXILED/releases)**
  - İçeriğini [7Zip](https://www.7-zip.org/) veya [WinRar](https://www.win-rar.com/download.html?&L=6) ile çıkartın.
  - **``EXILED``** Klasörünü **`%appdata%`** ya taşıyın *Not: Bu klasör  ``C:\Users\%UserName%\AppData\Roaming``, ve ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``**'nin içinde değil!**, ve (...)\AppData\Roaming'**de olması zorunludur**. (...)\AppData*'YA DEĞİL!
  - **``SCP Secret Laboratory``**Klasörünü **`%appdata%`**'ya taşı.
    - Windows 10 ve 11:
      Cortanaya / Arama simgesine veya Windows Explorer çubuğuna `%appdata%` yazın.
    - Diğer windows sürümleri
      Win + R tuşlarına basın ve `%appdata%` yazın.

### Eklenti İndirme
Bu kadar, EXILED şimdi sunucunuzda kuruldu ve bir sonraki sunucu başladığında aktif olmalıdır. Unutmayın ki EXILED kendi başına neredeyse hiçbir şey yapmaz, bu yüzden yeni eklentileri **[Discord](https://discord.gg/PyUkWTg)** sunucumuzdan almayı unutmayın.
- Bir Eklenti indirmek için aşağıdaki talimatları okuyun:
  - Bir eklentiyi indirmek için [*Onun* releases (yayınlanma) sayfasına gidin](https://i.imgur.com/u34wgPD.jpg) (**`.dll` UZANTILI OLMALIDIR**)
  - İndirdiğiniz eklentiyi: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins``'dizinine taşıyın (Win + R Tuşlarına basarak `%appdata%` yazarak buraya taşıyabilirsiniz)

# Linux
### Otomatik indirme ([daha fazla bilgi](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Not:** EXILED'i uzaktan bağlanılan bir sunucuya indiriyor iseniz, indirme programını sunucuyu kurduğunuz kullanıcı ile açın veya (root) yetkiniz olması gerekir.

  - **`Exiled.Installer-Linux`'i [Buradan indir](https://github.com/Exiled-Team/EXILED/releases)** (Assets'e tıkla -> Installer'ı indir)
  - Ya **`./Exiled.Installer-Linux --path /sunucuya/giden/klasor`** Yazarak ya da doğrudan sunucu klasörüne taşıyarak ve ardından terminalde (`cd` kullanarak) şu komutu yazarak yükleyin: **`./Exiled.Installer-Linux`**.
  - eğer en yeni ön yayını istiyor iseniz **`--pre-releases`** ekle. Örnek: **`./Exiled.Installer-Linux /sunucuya/giden/klasor --pre-releases`**
  - Başka bir örnek: `eğer Exiled.Installer-Linux` dosyası sunucu klasörüne yerleştirdiyseniz `/sunucuya/giden/klasor/Exiled.Installer-Linux --pre-releases`
  - Eklenti indirmek için [Bana tıkla ve göz gezdir!](#Eklenti-Indirme)

### Manuel indirme
  - SCP sunucusunu açan kullanıcı olduğundan **Emin** ol
  - **`Exiled.tar.gz` ['yi buradan indir](https://github.com/Exiled-Team/EXILED/releases)** (SSH: sağ tık yap ve `Exiled.tar.gz`'nin bağlantısını al, ve **`wget (bağlantı)`** Komudunu yazın.)
  - Bulunduğunuz klasöre çıkartmak için **``tar -xzvf EXILED.tar.gz``** Komudunu yazın.
  - **`EXILED`** Klasörünü **``~/.config``**'e taşı. *Not: Bu klasör ``~/.config``'e gitmeli, ``~/.config/SCP Secret Laboratory``**'nin içine değil!** (SSH: **`mv EXILED ~/.config/`**)
  - **`SCP Secret Laboratory`** Klasörünü **``~/.config``**'e taşı. *Not: Bu klasör ``~/.config``'nin içine gitmelidir,  ``~/.config/SCP Secret Laboratory``**'nin içine değil!** (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Eklenti Indirme
Bu kadar, EXILED şimdi sunucunuzda kuruldu ve bir sonraki sunucu başladığında aktif olmalıdır. Unutmayın ki EXILED kendi başına neredeyse hiçbir şey yapmaz, bu yüzden yeni eklentileri **[Discord](https://discord.gg/PyUkWTg)** sunucumuzdan almayı unutmayın.
- Bir Eklenti indirmek için aşağıdaki talimatları okuyun:
  - Bir eklentiyi indirmek için [*Onun* releases (yayınlanma) sayfasına gidin](https://i.imgur.com/u34wgPD.jpg) (**`.dll` UZANTILI OLMALIDIR**)
  - İndirdiğiniz Eklentiyi: ``~/.config/EXILED/Plugins``'dizininine taşıyın (eğer SSH'yi root olarak kullanıyor iseniz, o zaman doğru `.config`'i `/home/(SCP Server User)` dizininin içinde arayın.)

# Config
EXILED kendi başına bazı yapılandırma seçenekleri sunar.
Bunların hepsi sunucu açıldığı zaman otomatik olarak yapılır, Bunlar şuradadır Linuxda: ``~/.config/EXILED/Configs/(serverportu)-config.yml`` Windows için: (``%AppData%\EXILED\Configs\(serverportu)-config.yml``).

Eklenti Configleri(Yapılandırma seçenekleri) ``config_gameplay.txt`` Dosyasında **değildir** Onun yerine eklenti seçeneklieri Linuxda: ``~/.config/EXILED/Configs/(serverportu)-config.yml`` Windows için: (``%AppData%\EXILED\(serverportu)-config.yml``).
Ancak, bazı eklentiler kendi başlarına diğer yerlerden yapılandırma/ayarları alabilir. Bu klasör genellikle eklenti ayarlarının/bağlantılarının bulunduğu yerdir. Hatalar varsa lütfen ilgili eklenti geliştiricisine başvurun.

# Geliştiriciler için

Eğer EXILED için bir eklenti yapmak istiyorsanız, bunu yapmak oldukça basittir [Daha fazla bilgi için bana tıkla!](https://github.com/Exiled-Team/EXILED/blob/master/GettingStarted.md).

Daha kapsamlı ve sürekli güncellenen öğreticiler için [EXILED websitesine](https://exiled-team.github.io/EXILED/articles/install.html) göz atın.

Ama pluginlerini halka açık yaparken bu kuralları takip etmen lazım:

 - Eklentiniz ``Exiled.API.Features.Plugin<>`` sınıfından türetilmiş bir sınıf içermelidir; aksi halde EXILED sunucu başladığında eklentinizi yüklemeyecektir.
 - Bir eklenti yüklendiğinde, yukarıda bahsedilen sınıfın ``OnEnabled()`` yöntemindeki kod hemen yürütülür; diğer eklentilerin yüklenmesini beklemez. Sunucu başlatma sürecinin tamamlanmasını beklemez. Hiçbir şeyi beklemez. ``OnEnabled()`` yönteminizi yaparken, sunucu henüz başlatılmamış olabilecek şeylere erişim sağlamadığınızdan emin olun, ÖRNEK: ServerConsole.Port veya PlayerManager.localPlayer vb...
 - Eğer eklentiniz yüklendiğinde henüz başlatılmamış olan şeylere erişim sağlamanız gerekiyorsa, bunu yapmak için önerilen yol, bu işlemi gerçekleştirmek için ```WaitingForPlayers`` eventini(etkinliğini) beklemektir. Eğer daha erken bazı işlemler yapmanız gerekiyorsa, kodunuzu devam etmeden önce gerekli değişkenin/nesnenin null olmadığını kontrol eden bir ``while(!x)`` döngüsü içine almanız önerilir.
 - EXILED, yürütme sırasında eklenti derlemelerini dinamik olarak yeniden yükleme işlemini destekler. Bu, bir eklentiyi güncellemeniz gerektiğinde sunucuyu yeniden başlatmadan yapılabilir. Ancak, yürütme sırasında bir eklentiyi güncelliyorsanız, eklentinin bunu desteklemesi gerekmektedir; aksi halde sorunlarla karşılaşabilirsiniz. Daha fazla bilgi ve takip edilmesi gereken kurallar için ``Dinamik Güncelleme`` bölümüne başvurun.
 - EXILED'da OnUpdate, OnFixedUpdate veya OnLateUpdate eventi(etkinliği) ***Bulunmamaktadır!***, Eğer sık sık çalışan bir kod calıştırmanız gerekiyor ise bir MEC coroutine Kullanabilirsiniz ki bu bir frame, 0.01f bekler ya da Timing.FixedUpdate gibi bir Timing katmanı kullanabilirsiniz.
 ### MEC (More Effective Coroutines) Coroutines (Eş zamanlı iş parçacığı)
Hiç MEC (More Effective Coroutines) kullanmadı iseniz işte size MEC kullanmanız için bir rehber!
MEC Coroutine'leri zamanlanmış yöntemlerdir. ve çalışan bir MEC kodunun kesilmeden / devre dışı bırakmadan önce belirli bir süre beklemenizi destekler
MEC Coroutine'leri Unity ile kullanılmak üzere güvenlidir AMA ***Unity ile etkileşimde bulunmak için yeni Threadler(iş parçacıkları) oluşturmayın!! sunucuyu çökertir.***

MEC Kullanmak için ``Assembly-CSharp-firstpass.dll``'yi referans etmeniz ve ``Using MEC;``'yi eklemeniz gerekir.

HER DÖNGÜ ARASINDA 5 SANİYE BEKLEYEN BİR COROUTINE YAPIMI:

```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //aşağıdaki kodu sonsuza kadar çalıştır
    {
        Log.Info("ben bir döngüyüm!"); //Log.Info yu sunucu konsolunda bir satır yazmak için çağırıldı.
        yield return Timing.WaitForSeconds(5f); //Bu coroutine'a 5 saniye beklemesini söyler, çünkü bu döngünün sonunda olduğu için, döngünün tekrarlanmasını etkili bir şekilde 5 saniye boyunca duraklatır.
    }
}
```

Eğer MEC hakkında bilgi sahibi değilseniz ve daha fazla öğrenmek, tavsiye almak veya yardıma ihtiyacınız varsa, **kesinlikle** biraz Google'da araştırma yapmanız veya Discord'ta soru sormanız tavsiye edilir. Sorular, ne kadar 'saçma' olursa olsun, her zaman mümkün olan en yardımcı ve net şekilde cevaplanacaktır; bu, eklenti geliştiricilerinin daha iyi kod yazmalarına yardımcı olmak içindir. Daha iyi bir kod, herkes için daha iyidir.

### Dinamik Güncelleme
EXILED, bir sunucu yeniden başlatma işlemine gerek olmadan eklenti derlemelerini dinamik olarak yeniden yükleme işlemini destekleyen bir Framework(Yazılım iskeleti)'dir.
Örneğin, sunucuyu sadece `Exiled.Events` eklentisiyle başlatırsanız ve yeni bir eklenti eklemek istiyorsanız, bu görevi tamamlamak için sunucuyu yeniden başlatmanıza gerek yoktur. Basitçe Remote Admin veya Sunucu Konsolu komutu olan ``reload plugins` komutunu kullanarak, önce yüklenmemiş olan yeni eklentiler dahil olmak üzere tüm EXILED eklentilerini yeniden yükleyebilirsiniz.

Bu aynı zamanda eklentileri tamamen yeniden başlatmadan *güncelleme* yapmanıza da olanak tanır. Ancak, bunun düzgün bir şekilde gerçekleşmesi için eklenti geliştiricisi tarafından takip edilmesi gereken birkaç kılavuz bulunmaktadır:

***Sunucu sahipleri için***
 - Eğer bir eklentiyi güncelliyor iseniz emin olunki derlemenin adı şu anda yüklü olan sürüm ile (varsa) aynı değildir. bu işlem eklentiyi yapan Geliştirici tarafından Dinamik Güncelleme özelliği gözetilerek yapılmış olması gerekir, sadece dosya adını değiştirmek işe yaramaz.
 - Eğer eklenti dinamik güncellemeleri destekliyorsa, yeni sürümü "Plugins" klasörüne koyarken, aynı zamanda eski sürümü de klasörden kaldırdığınızdan emin olun. EXILED'ı yeniden yüklemeden önce bunu sağlamamak, birçok kötü duruma yol açabilir.
 - Dinamik olarak bir eklentiyi güncellemenin ortaya çıkardığı herhangi bir sorun, yalnızca sizin ve ilgili eklentinin geliştiricisinin sorumluluğundadır. EXILED dinamik güncellemeleri tamamen destekler ve teşvik eder; ancak, hata ihtimali, sunucu sahibi veya eklenti geliştiricisi tarafından yanlış bir şeyler yapıldığında ortaya çıkabilir. Dinamik güncellemelerle ilgili bir hata bildirmeden önce, her iki tarafın da işlemi doğru bir şekilde gerçekleştirdiğini doğrulayın.

 ***Geliştiriciler için***

 - Dinamik güncellemeleri desteklemek isteyen eklentiler, devre dışı bırakıldıklarında veya yeniden yüklendiklerinde bağlı oldukları tüm olaylardan aboneliklerini(Subscribe) iptal etmeye dikkat etmelidir.
 - Özel Harmony yamaları(patch)'leri bulunan eklentiler, Harmony örneğinin adında bir değişken kullanmalı ve eklenti devre dışı bırakıldığında veya yeniden yüklendiğinde Harmony örneğini ``UnPatchAll()`` kullanarak iptal etmelidir.
 - ``OnEnabled()`` içinde başlatılan herhangi bir coroutine, eklenti devre dışı bırakıldığında veya yeniden yüklendiğinde sonlandırılmalıdır / Bitirilmelidir.

Bu işlemlerin hepsi, eklenti sınıfındaki ``OnReloaded()`` veya ``OnDisabled()`` yöntemlerinde gerçekleştirilebilir. EXILED eklentileri yeniden yüklediğinde, İlk olarak ``OnDisabled()``, ardından ``OnReloaded()``, daha sonra yeni derlemeleri yükler ve en son olarak ``OnEnabled()`` yöntemini çalıştırır.


Unutmayın ki: bu yeni derlemelerin olduğu anlamına gelir. Eğer aynı isimdeki bir derlemeyi başka bir Derleme ile değiştirir iseniz o derleme ***GÜNCELLENMEZ***. Bu, Global Assembly Cache (GAC) nedeniyledir. Eğer Önbellekte zaten var olan bir derlemeyi 'yüklemeye' çalışır iseniz her zaman önbellekteki derlemeyi kullanır

Bu nedenle, eğer eklentiniz dinamik güncellemeleri destekliyorsa, her sürümü farklı bir derleme adıyla derlemelisiniz (dosyanın adını değiştirmek işe yaramaz). Ayrıca, eski derleme artık gerekli olmadığında "Silinmediği" için, olaylardan abonelik(Subscribe) iptal etmeyi, Harmony örneğinizi iptal etmeyi, coroutine'leri sonlandırmayı vb... unutmazsanız, bu kodun eski sürümü de yeni sürüm koduyla birlikte çalışmaya devam eder. Bu, gerçekleşmesine izin vermek için çok kötü bir fikirdir.

Bu nedenle, dinamik güncellemeleri destekleyen eklentilerin bu yönergeleri takip etmeleri ***ZORUNLUDUR***; aksi halde potansiyel risk nedeniyle Discord sunucudan kaldırılabilirler.

Her eklentinin dinamik güncellemeleri desteklemesi gerekmez. Eğer dinamik güncellemeleri desteklemeyi planlamıyorsanız, bu tamamen uygun bir durumdur. Sadece yeni bir sürüm oluştururken eklentinizin derleme adını değiştirmekten kaçının. Bu tür durumlarda, sunucu sahiplerinin eklentinizi güncellemek için sunucularını tamamen yeniden başlatmaları gerektiğini bildirin.

çeviri: Enes Batur (_funnyman_xdxdxd.rofl.)
