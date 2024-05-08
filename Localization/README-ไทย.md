<h1 align="center">EXILED - Library รันไทม์สำหรับการพัฒนาภายนอก</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/PyUkWTg">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>    

</div>

EXILED เป็นปลั๊กอินเฟรมเวิร์คระดับสูงสำหรับเซิร์ฟเวอร์ SCP: Secret Laboratory. มันมีระบบอีเวนท์สำหรับนักพัฒนาในการเชื่อมต่อเพื่อปรับแต่งหรือเปลี่ยนแปลงโค้ดเกมหรือสร้างฟังก์ชั่นของคุณเอง.
อีเวนท์ทั้งหมดของ EXILED ถูกเขียนด้วย Harmony, ซึ่งหมายความว่าไม่จำเป็นต้องแก้ไขแอสเซมบลีของเซิร์ฟเวอร์โดยตรงเพื่อให้ทำงาน, ซึ่งมีอยู่ 2 ประการที่โดดเด่น.

 - ประการแรก, โค้ดทั้งหมดของเฟรมเวิร์คเผยแพร่และแชร์ได้อย่างอิสระ, ช่วยให้นักพัฒนาเข้าใจ *วิธี* การทำงานของมันได้ดียิ่งขึ้น และเสนอแนะฟีเจอร์เพิ่มเติมหรือปรับเปลี่ยนฟีเจอร์ที่มีอยู่.
 - ประการที่สอง, เนื่องจากโค้ดทั้งหมดที่เกี่ยวข้องกับเฟรมเวิร์คทำงานแยกกับแอสเซมบลีของเซิร์ฟเวอร์, การอัปเดตเกมเล็กน้อยๆ, จึงส่งผลกระทบต่อเฟรมเวิร์คเพียงเล็กน้อยหรืออาจจะไม่มีผลเลย, ส่งผลให้เฟรมเวิร์ค EXILED มีแนวโน้มเข้ากันได้กับการอัปเดตเกมในอนาคต, รวมถึงง่ายต่อการอัปเดตเฟรมเวิร์คเอง *เมื่อ* จำเป็น.

# ภาษา READMEs อื่นฯ
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

# การติดตั้ง
การติดตั้ง EXILED นั้นง่ายมาก. มันโหลดตัวเองผ่านทาง Northwood's Plugin API. ดังนั้นภายในไฟล์ release ``Exiled.tar.gz`` จึงมีสองโฟลเดอร์. ``SCP Secret Laboratory`` มีไฟล์ที่จำเป็นในการโหลดฟีเจอร์ของ EXILED ที่อยู่ภายในโฟลเดอร์ ``EXILED``. สิ่งที่คุณต้องทำคือย้ายโฟลเดอร์ทั้งสองนี้ไปยังที่อยู่เหมาะสม, ซึ่งจะอธิบายด้านล่าง, เพียงเท่านี้คุณก็ติดตั้งเสร็จเรียบร้อย!

ถ้าคุณเลือกใช้โปรแกรมติดตั้ง (installer), มันจะทำการติดตั้งฟีเจอร์ทั้งหมดของ EXILED ให้โดยอัตโนมัติ,  หากคุณรันโปรแกรมติดตั้งนั้นอย่างถูกต้อง.

# Windows
### การติดตั้งอัตโนมัติ ([รายละเอียดเพิ่มเติม](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**หมายเหตุ**: ตรวจสอบให้แน่ใจว่าคุณใช้บัญชีผู้ใช้ที่รันเซิร์ฟเวอร์อยู่, หรือที่มีสิทธิ์แอดมินก่อนที่จะรันโปรแกรมติดตั้ง.

  - ดาวน์โหลด **`Exiled.Installer-Win.exe` [จากที่นี่](https://github.com/Exiled-Team/EXILED/releases)** (คลิกที่ Assets -> จากนั้น คลิกที่ Installer)
  - ย้ายโปรแกรมติดตั้งไปที่โฟลเดอร์เซิร์ฟเวอร์ของคุณ (ดาวน์โหลด dedicated server หากคุณยังไม่มี)
  - ดับเบิ้ลคลิก **`Exiled.Installer.exe`** หรือ **[ดาวน์โหลด .bat นี้](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** และวางไว้ในโฟลเดอร์เซิร์ฟเวอร์เพื่อติดตั้งเวอร์ชัน pre-release ล่าสุด
  - วิธีการหาและติดตั้งปลั๊กอิน, ให้ไปที่ [การติดตั้งปลั๊กอิน](#installing-plugins) ส่วนด้านล่าง.
**หมายเหตุ:** ถ้าคุณติดตั้ง EXILED บน remote เซิร์ฟเวอร์, ตรวจสอบให้แน่ใจว่าคุณรันไฟล์ .exe ในฐานะผู้ใช้เดียวกับที่รันเซิร์ฟเวอร์ SCP:SL ของคุณ (หรือผู้ที่มีสิทธิ์แอดมิน)

### การติดตั้งด้วยตนเอง
  - ดาวน์โหลด **`Exiled.tar.gz` [จากที่นี่](https://github.com/Exiled-Team/EXILED/releases)**
  - แตกไฟล์ได้โดยใช้ [7Zip](https://www.7-zip.org/) หรือ [WinRar](https://www.win-rar.com/download.html?&L=6)
  - ย้ายโฟลเดอร์ **``EXILED``** ไปที่ **`%appdata%`** *หมายเหตุ: โฟลเดอร์นี้ต้องอยู่ใน ``C:\Users\%UserName%\AppData\Roaming``, เเละ ***ไม่ใช่*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, เเละ **มันต้องอยู่** ข้างใน (...)\AppData\Roaming, ไม่ใช่ (...)\AppData\!*
  - ย้าย **``SCP Secret Laboratory``** ไปที่ **`%appdata%`**.
    - Windows 10 & 11:
      พิมพ์ `%appdata%` ใน Cortana / search icon, หรือที่ Windows Explorer bar.
    - Windows เวอร์ชั่นอื่นฯ:
      กด Win + R เเละพิมพ์ `%appdata%`

### การติดตั้งปลั๊กอิน
เรียบร้อย!, EXILED จะถูกติดตั้งและทำงานในครั้งต่อไปที่คุณบูทเซิร์ฟเวอร์ของคุณ. โปรดทราบว่า EXILED เพียงอย่างเดียวจะแทบไม่มีประโยชน์, ดังนั้นอย่าลืมหาปลั๊กอินใหม่จาก **[Discord server ของเรา](https://discord.gg/PyUkWTg)**
- หากต้องการติดตั้งปลั๊กอิน, ง่ายฯ:
  - ดาวน์โหลดปลั๊กอินจาก [หน้า releases ของ *ผู้ทําปลั๊กอิน*](https://i.imgur.com/u34wgPD.jpg) (**มันต้องเป็น `.dll`!**)
  - ย้ายไปที่: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins`` (ไปที่นี่โดยการกด Win + R, เเล้วพิมพ์ `%appdata%`)

# Linux
### การติดตั้งอัตโนมัติ ([รายละเอียดเพิ่มเติม](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**หมายเหตุ:** หากคุณคุณติดตั้ง EXILED บน remote เซิร์ฟเวอร์. ตรวจสอบให้แน่ใจว่าคุณรันโปรแกรมติดตั้ง, ด้วยผู้ใช้เดียวกับที่รันเซิร์ฟเวอร์ SCP:SL ของคุณ (หรือผู้ใช้ root) 

  - ดาวน์โหลด **`Exiled.Installer-Linux` [จากที่นี่](https://github.com/Exiled-Team/EXILED/releases)** (คลิกที่ Assets -> จากนั้น คลิกที่ Installer)
  - ติดตั้งโดยการพิมพ์ **`./Exiled.Installer-Linux --path /path/to/server`** หรือย้ายไปไว้ในโฟลเดอร์เซิร์ฟเวอร์โดยตรง, ย้ายไปที่เทอร์มินัล (`cd`) แล้วพิมพ์: **`./Exiled.Installer-Linux`**.
  - หากคุณต้องการเวอร์ชัน pre-release ล่าสุด, เเค่เพิ่ม **`--pre-releases`**. ตัวอย่าง: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - อีกตัวอย่าง, หากคุณวาง `Exiled.Installer-Linux` ไว้ในโฟลเดอร์เซิร์ฟเวอร์ของคุณ: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - วิธีการหาและติดตั้งปลั๊กอิน, ให้ไปที่ [การติดตั้งปลั๊กอิน](#installing-plugins-1) ส่วนด้านล่าง.

### การติดตั้งด้วยตนเอง
  - **ตรวจสอบ** ว่าคุณเข้าสู่ระบบของผู้ใช้ที่ใช้งานเซิร์ฟเวอร์ SCP
  - ดาวน์โหลด **`Exiled.tar.gz` [จากที่นี่](https://github.com/Exiled-Team/EXILED/releases)** (SSH: คลิกขวาเเล้วกดคัดลอกลิ้ง `Exiled.tar.gz`, เเล้วพิมพ์: **`wget (link_to_download)`**)
  - เพื่อแตกไฟล์ไปยังโฟลเดอร์ปัจจุบันของคุณ, พิมพ์ **``tar -xzvf EXILED.tar.gz``**
  - ย้ายโฟลเดอร์ **`EXILED`** ไปที่ **``~/.config``**. *หมายเหตุ: โฟลเดอร์ต้องอยู่ใน ``~/.config``, เเละ ***ไม่ใช่*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - ย้ายโฟลเดอร์ **`SCP Secret Laboratory`** ไปที่ **``~/.config``**. *หมายเหตุ: โฟลเดอร์ต้องอยู่ใน ``~/.config``, เเละ ***ไม่ใช่*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### การติดตั้งปลั๊กอิน
เรียบร้อย!, EXILED จะถูกติดตั้งและทำงานในครั้งต่อไปที่คุณบูทเซิร์ฟเวอร์ของคุณ. โปรดทราบว่า EXILED เพียงอย่างเดียวจะแทบไม่มีประโยชน์, ดังนั้นอย่าลืมหาปลั๊กอินใหม่จาก **[Discord server ของเรา](https://discord.gg/PyUkWTg)**
- หากต้องการติดตั้งปลั๊กอิน, ง่ายฯ:
  - ดาวน์โหลดปลั๊กอินจาก [หน้า releases ของ *ผู้ทําปลั๊กอิน*](https://i.imgur.com/u34wgPD.jpg) (**มันต้องเป็น `.dll`!**)
  - ย้ายไปที่: ``~/.config/EXILED/Plugins`` (หากคุณใช้ SSH ในฐานะ root, ให้ค้นหา `.config` ที่ถูกต้องซึ่งมันจะอยู่ที่ `/home/(ผู้ใช้เซิร์ฟเวอร์ SCP)`)

# Config
EXILED เองมีตัวเลือกการกําหนด config บางอย่าง.
ทั้งหมดถูกสร้างขึ้นโดยอัตโนมัติเมื่อเริ่มต้นเซิร์ฟเวอร์, ซึ่งไฟล์อยู่ที่ ``~/.config/EXILED/Configs/(พอร์ตเซิร์ฟเวอร์)-config.yml`` (``%AppData%\EXILED\Configs\(พอร์ตเซิร์ฟเวอร์)-config.yml`` บน Windows).

การกำหนดค่า configs ปลั๊กอินจะ ***ไม่*** อยู่ในไฟล์ ``config_gameplay.txt``, โดยไฟล์ configs ปลั๊กอินจะอยู่ที่ ``~/.config/EXILED/Configs/(พอร์ตเซิร์ฟเวอร์)-config.yml`` (``%AppData%\EXILED\(พอร์ตเซิร์ฟเวอร์)-config.yml`` บน Windows).
อย่างไรก็ตาม, ปลั๊กอินบางตัวอาจการตั้งค่า config จากตำแหน่งอื่นบนเซิร์ฟเวอร์เอง, ซึ่งถือเป็นตำแหน่งเริ่มต้นที่ EXILED กำหนดไว้ให้โดยทั่วไป, ดังนั้นหากพบปัญหาควรอ้างอิงตามคำแนะนำของปลั๊กอินแต่ละตัวไป.

# สำหรับนักพัฒนา

คุณสามารถสร้างปลั๊กอินสำหรับ EXILED ได้เอง, ไม่ยากเลย. ถ้าหากคุณต้องการคำแนะนำเพิ่มเติมกรุณาไปที่ [หน้าเริ่มต้น](https://github.com/Exiled-Team/EXILED/blob/master/GettingStarted.md).

สำหรับบทช่วยสอนที่ครอบคลุมและอัปเดตมากขึ้น, โปรดไปที่ [เว็บไซต์ EXILED](https://exiled-team.github.io/EXILED/articles/install.html).

แต่อย่าลืมปฏิบัติตามกฎเหล่านี้เมื่อเผยแพร่ปลั๊กอินของคุณ:

 - ปลั๊กอินของคุณจำเป็นต้องมีคลาสที่ inherits มาจาก ``Exiled.API.Features.Plugin<>``, ถ้าไม่มีคลาสนี้, EXILED จะไม่โหลดปลั๊กอินของคุณเมื่อเซิร์ฟเวอร์เริ่มต้น.
 - เมื่อปลั๊กอินโหลดเสร็จ, โค้ดภายในเมธอด ``OnEnabled()`` ของคลาสดังกล่าวจะทำงานทันที, โดยไม่รอให้ปลั๊กอินอื่นโหลดเสร็จ. และไม่รอให้กระบวนการเริ่มต้นของเซิร์ฟเวอร์เสร็จสิ้น ***มันไม่รออะไรทั้งนั้น*** ดังนั้น ในขณะที่คุณตั้งค่าเมธอด ``OnEnabled()``, ต้องแน่ใจว่าคุณไม่ได้เข้าถึงสิ่งต่างๆ ที่เซิร์ฟเวอร์อาจยังไม่ได้เตรียมไว้, เช่น  ``ServerConsole.Port`` หรือ ``PlayerManager.localPlayer``.
 - หากคุณต้องการเข้าถึงสิ่งต่าง ๆ ที่ยังไม่ได้เริ่มต้นก่อนที่ปลั๊กอินของคุณจะโหลด, ขอแนะนำให้รออีเวนท์ ``WaitingForPlayers`` ในการทำ, ถ้าคุณต้องการทำบางสิ่งอย่างที่เร็วกว่านั้น, คุณสามารถใช้โค้ดในลูป ```while(!x)``` ซึ่งจะตรวจสอบ variable/object ที่คุณต้องการไม่ให้เป็น null ก่อนที่จะดำเนินการต่อไปได้.
 - EXILED รองรับการรีโหลดแอสเซมบลีของปลั๊กอินแบบไดนามิกขณะรันโปรแกรม, ซึ่งหมายความว่าหากคุณต้องการอัปเดตปลั๊กอิน, สามารถทำได้โดยไม่ต้องรีบูตเซิร์ฟเวอร์, อย่างไรก็ตาม, หากคุณกำลังอัปเดตปลั๊กอินขณะรัน, ปลั๊กอินนั้นจำเป็นต้องได้รับการตั้งค่าอย่างเหมาะสมเพื่อรองรับการทำงานนี้, มิเช่นนั้นคุณจะประสบปัญหาร้ายแรง. โปรดดูส่วน ``การอัปเดตแบบไดนามิก`` สำหรับข้อมูลเพิ่มเติมและแนวทางที่ควรปฏิบัติตาม.
 - ***ไม่มี*** อีเวนท์ OnUpdate, OnFixedUpdate หรือ OnLateUpdate ใน EXILED. ถ้าคุณต้องการรันโค้ดบ่อยๆ, คุณสามารถใช้ MEC coroutine ที่รอเฟรม, 0.01f, หรือใช้เลเยอร์การจับเวลาอย่าง Timing.FixedUpdate แทน.
 ### MEC Coroutines
ถ้าคุณไม่คุ้นเคยกับ MEC, บทความนี้จะเป็นบทนำสั้น ๆ และง่าย ๆ เพื่อช่วยให้คุณเริ่มต้น.
MEC Coroutines ทำหน้าที่เหมือนกับการตั้งเวลาให้กับเมธอด, โดยสามารถพักเพื่อรอช่วงเวลานึงก่อนที่จะทำงานต่อ, โดยไม่รบกวนการทำงานของเธรดหลักของเกม.
MEC coroutines ปลอดภัยสำหรับใช้งานกับ Unity, แตกต่างจากการสร้างเธรดแบบดั้งเดิม. ***ห้าม สร้างเธรดใหม่เพื่อโต้ตอบกับ Unity, เพราะจะทำให้เซิร์ฟเวอร์ล่ม.***

ในการใช้ MEC, คุณต้องอ้างอิง reference ของ Assembly-CSharp-firstpass.dll จากไฟล์เซิร์ฟเวอร์และใส่ using MEC;
ตัวอย่างการเรียกใช้ coroutine ง่ายๆ, ที่ทำงานวนซ้ำ โดยมีการ delay เวลาระหว่างแต่ละรอบ:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //repeat the following infinitely
    {
        Log.Info("Hey I'm a infinite loop!"); //Call Log.Info to print a line to the game console/server logs.
        yield return Timing.WaitForSeconds(5f); //Tells the coroutine to wait 5 seconds before continuing, since this is at the end of the loop, it effectively stalls the loop from repeating for 5 seconds.
    }
}
```

ผู้ที่ไม่คุ้นเคยกับ MEC ***แนะนำอย่างยิ่ง*** ให้ลองค้นหาข้อมูลเพิ่มเติมบน Google หรือสอบถามใน Discord เพื่อเรียนรู้เพิ่มเติม, ขอคำแนะนำ, หรือต้องการความช่วยเหลือ. คำถามใดๆ, ไม่ว่าจะดูเหมือน 'โง่' แค่ไหน, ก็จะได้รับคำตอบอย่างชัดเจนและเป็นประโยชน์ที่สุด เพื่อช่วยให้ผู้พัฒนาปลั๊กอินเก่งกาจยิ่งขึ้น, เพราะโค้ดที่ดีกว่าย่อมดีสำหรับทุกคน.

### การอัปเดตแบบไดนามิก
EXILED เป็นเฟรมเวิร์คที่รองรับการรีโหลดแอสเซมบลีของปลั๊กอินแบบไดนามิกโดยไม่ต้องรีบูตเซิร์ฟเวอร์.
ตัวอย่างเช่น, หากคุณเริ่มต้นเซิร์ฟเวอร์ด้วย `Exiled.Events` เป็นปลั๊กอินเพียงอย่างเดียว, แล้วต้องการเพิ่มปลั๊กอินใหม่, คุณไม่จำเป็นต้องรีบูตเซิร์ฟเวอร์เพื่อดำเนินการนี้.
คุณสามารถใช้คำสั่ง `reload plugins` ใน Remote Admin หรือ Server Console เพื่อโหลดปลั๊กอิน EXILED ใหม่ทั้งหมด, รวมถึงปลั๊กอินใหม่ที่ยังไม่ได้โหลดก่อนหน้านี้.

นอกจากนี้ คุณยังสามารถ *อัปเดต* ปลั๊กอินได้โดยไม่ต้องรีบูตเซิร์ฟเวอร์ทั้งหมดเช่นกัน. อย่างไรก็ตาม, ผู้พัฒนาปลั๊กอินจำเป็นต้องปฏิบัติตามหลักเกณฑ์บางประการเพื่อให้บรรลุเป้าหมายนี้ได้อย่างถูกต้อง:

***สําหรับโฮสต์***
 - หากคุณกําลังอัปเดตปลั๊กอิน, ตรวจสอบให้แน่ใจว่าชื่อแอสเซมบลีไม่ตรงกับเวอร์ชันปัจจุบันที่คุณติดตั้ง (ถ้ามี). ปลั๊กอินต้องสร้างโดยผู้พัฒนา โดยคำนึงถึงการอัปเดตแบบไดนามิก, การเปลี่ยนชื่อไฟล์เพียงอย่างเดียวจะไม่สามารถใช้งานได้.
 - ถ้าปลั๊กอินรองรับการอัปเดตแบบไดนามิก, เวลาที่คุณวางปลั๊กอินเวอร์ชันใหม่ลงในโฟลเดอร์ "Plugins", ต้องลบปลั๊กอินเวอร์ชันเก่าออกจากโฟลเดอร์ก่อนรีโหลด EXILED ใหม่, ไม่เช่นนั้นอาจเกิดปัญหาต่างๆ ได้.
 - ปัญหาใด ๆ ที่เกิดขึ้นจากการอัปเดตปลั๊กอินแบบไดนามิก เป็นความรับผิดชอบของคุณและผู้พัฒนาปลั๊กอินที่เกี่ยวข้องเท่านั้น. แม้ว่า EXILED จะรองรับและส่งเสริมการอัปเดตแบบไดนามิกอย่างเต็มที่, แต่ปัญหาที่อาจเกิดขึ้นได้คือ โฮสต์เซิร์ฟเวอร์หรือผู้พัฒนาปลั๊กอินทำอะไรผิดพลาด. ตรวจสอบให้แน่ใจว่าทั้งสองฝ่ายดำเนินการอย่างถูกต้องก่อนรายงาน Bug เกี่ยวกับการอัปเดตแบบไดนามิกไปยังผู้พัฒนา EXILED.

 ***สําหรับผู้พัฒนา***

 - ปลั๊กอินที่รองรับการอัปเดตแบบไดนามิก ต้อง unsubscribe อีเวนต์ทั้งหมดที่เคยเชื่อมต่อไว้ เมื่อปลั๊กอินถูกปิดใช้งานหรือรีโหลดซ้ำ.
 - ปลั๊กอินที่มี Harmony patches แบบกำหนดเอง ต้องใช้ตัวแปรเปลี่ยนบางอย่างภายในชื่อของ Harmony Instance, และต้องใช้คำสั่ง ``UnPatchAll()`` กับ Harmony instance ของตัวเองเมื่อปลั๊กอินถูกปิดใช้งานหรือรีโหลดซ้ำ.
 - โค้ด coroutines ใด ๆ ที่ปลั๊กอินเริ่มต้นในฟังก์ชัน ``OnEnabled()`` จะต้องถูกหยุดการทำงานด้วยเช่นกัน เมื่อปลั๊กอินถูกปิดใช้งานหรือรีโหลดซ้ำ.

บรรลุสิ่งเหล่านี้ได้ทั้งในเมธอด ``OnReloaded()`` หรือ ``OnDisabled()`` ของคลาสปลั๊กอิน. เมื่อ EXILED รีโหลดปลั๊กอินซ้ำ, จะเรียกใช้ ``OnDisabled()`` จากนั้น ``OnReloaded()``, ต่อไปจะโหลดแอสเซมบลีใหม่, แล้วจึงรัน ``OnEnabled()``.

ต้องใช้แอสเซมบลี *ใหม่* เสมอ. แทนที่แอสเซมบลีเดิมด้วยอันใหม่ที่มีชื่อเดียวกันจะ ***ไม่*** สามารถอัพเดทได้, เนื่องจากระบบ GAC (Global Assembly Cache), ถ้าโหลดแอสเซมบลีที่มีอยู่แล้วในแคช, ระบบจะดึงจากแคชมาใช้แทน.
ด้วยเหตุผลนี้, หากปลั๊กอินของคุณรองรับการอัปเดตแบบไดนามิก, คุณต้องสร้างแต่ละเวอร์ชันด้วยชื่อแอสเซมบลีที่แตกต่างกันในตัวเลือกการสร้าง (การเปลี่ยนชื่อไฟล์ใช้ไม่ได้). นอกจากนี้, เนื่องจากแอสเซมบลีเก่าไม่ได้ "ถูกทำลาย" เมื่อไม่จำเป็นอีกต่อไป, หากคุณไม่สามารถ unsubscribe อีเวนต์, ยกเลิกการแพทช์ harmony instance ของคุณ, ยกเลิก coroutines, เป็นต้น., โค้ดดังกล่าวจะยังคงทำงานควบคู่ไปกับโค้ดของเวอร์ชันใหม่ด้วย. ซึ่งเป็นสิ่งที่ไม่ควรเกิดขึ้นอย่างยิ่ง.

ด้วยเหตุนี้ปลั๊กอินที่รองรับการอัพเดทแบบไดนามิก, ***ต้อง*** ปฏิบัติตามแนวทางเหล่านี้อย่างเคร่งครัด มิเช่นนั้นจะถูกลบออกจากเซิฟเวอร์ดิสคอร์ด เนื่องจากความเสี่ยงที่อาจเกิดขึ้นกับโฮสต์เซิฟเวอร์.

ปลั๊กอินไม่จำเป็นต้องรองรับการอัพเดทแบบไดนามิกทั้งหมด. หากคุณไม่ต้องการรองรับการอัพเดทแบบไดนามิก, ก็ไม่เป็นไร. เพียงแค่หลีกเลี่ยงการเปลี่ยนชื่อแอสเซมบลีของปลั๊กอินของคุณเมื่อสร้างเวอร์ชันใหม่.
ในกรณีดังกล่าว โปรดแจ้งให้โฮสต์เซิฟเวอร์ทราบว่าพวกเขาจำเป็นต้องรีบู๊ตเซิฟเวอร์ทั้งหมดเพื่ออัปเดทปลั๊กอินของคุณ.