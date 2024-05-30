# เอกสารประกอบ Exiled ระดับล่าง
*(เขียนโดย [KadeDev](https://github.com/KadeDev) สำหรับชุมชน)*

## เริ่มต้นใช้งาน
### บทนำ
Exiled เป็น API ระดับต่ำ ซึ่งหมายความว่าคุณสามารถเรียกใช้ฟังก์ชันต่างๆ จากเกมได้โดยไม่ต้องใช้โบลตแวร์ API จำนวนมาก.

ซึ่งช่วยให้อัปเดต Exiled ได้อย่างง่ายดาย, และสามารถอัปเดต Exiled ได้ก่อนที่การอัปเดตจะเข้าสู่เกมด้วยซ้ำ.

นอกจากนี้ยังช่วยให้นักพัฒนาปลั๊กอินไม่ต้องเปลี่ยนโค้ดหลังจากทุกอัปเดตของ Exiled หรือ SCP:SL. ที่จริงแล้วพวกเขาไม่จำเป็นต้องอัปเดตปลั๊กอินด้วยซ้ำ!

เอกสารนี้จะแสดงพื้นฐานเบื้องต้นของการสร้าง Exiled Plugin จากที่นี่. คุณสามารถเริ่มแสดงให้โลกเห็นว่าคุณสร้างสรรค์สิ่งสร้างสรรค์อะไรได้บ้างด้วยเฟรมเวิร์กนี้!

### ตัวอย่างปลั๊กอิน
นี้คือ [ตัวอย่างปลั๊กอิน](https://github.com/galaxy119/EXILED/tree/master/Exiled.Example) ซึ่งเป็นปลั๊กอินง่ายๆ ที่แสดงอีเวนท์ต่างๆ และวิธีสร้างอีเวนท์อย่างเหมาะสม. การใช้ตัวอย่างนี้จะช่วยให้คุณเรียนรู้วิธีใช้ Exiled ได้อย่างถูกต้อง. มีสองสิ่งที่สำคัญในปลั๊กอินนั้น, มาพูดถึงกันดีกว่า

#### On Enable + On Disable Dynamic Updates
Exiled เป็นเฟรมเวิร์กที่มีคำสั่ง **Reload** ซึ่งสามารถใช้เพื่อรีโหลดปลั๊กอินทั้งหมดและโหลดปลั๊กอินใหม่. ซึ่งหมายความว่าคุณต้องทำให้ปลั๊กอินของคุณ **Dynamically Updatable.** ซึ่งหมายความว่าทุกตัวแปร, อีเวนท์, coroutine, ฯลฯ *ต้อง* ได้รับการกำหนดเมื่อเปิดใช้งาน และจะเป็น nullified เมื่อปิดใช้งาน. วิธี **On Enable** ควรเปิดใช้งานทั้งหมด, และวิธีการ **เปิดปิดใช้งาน** ควรปิดใช้งานทั้งหมด. แต่คุณอาจสงสัยว่า **On Reload** เป็นอย่างไร? void นั้นมีไว้เพื่อส่งต่อตัวแปรคงที่, เนื่องจากค่าคงที่คงที่ทุกค่าที่คุณสร้างจะไม่ถูกล้าง. ดังนั้นคุณสามารถทำสิ่งนี้ได้:
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

และผลลัพธ์ที่ได้จะเป็น:
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
(แน่นอนว่าไม่รวมสิ่งใดนอกจากการตอบกลับที่เกิดขึ้นจริง)
หากไม่ทำเช่นนี้ มันก็จะต้องไปที่ 1 แล้วจึงไปที่ 2 อีกครั้ง

### Players + Events
ตอนนี้เราเสร็จสิ้นการทําปลั๊กอิน **Dynamically Updatable** เเล้ว เรามุ่งความสนใจไปที่การพยายามโต้ตอบกับผู้เล่นในอีเวนท์ต่างๆ ได้เลย!

อีเวนท์นี้เจ๋งมาก, ช่วยให้ SCP:SL สามารถสื่อสารกับ Exiled และต่อด้วย Exiled ไปยังปลั๊กอินทั้งหมดได้!

คุณสามารถฟังอีเวนท์สำหรับปลั๊กอินของคุณได้โดยเพิ่มสิ่งนี้ไว้ที่ด้านบนของไฟล์ต้นฉบับปลั๊กอินหลักของคุณ:
```csharp
using EXILED;
```
จากนั้นคุณจะต้องอ้างอิงไฟล์ reference `Exiled.Events.dll` เพื่อให้คุณสามารถรับอีเวนท์ได้จริง.

เพื่ออ้างอิงถึงอีเวนท์ เราจะใช้คลาสใหม่ที่เราสร้างขึ้น; เรียกว่า "EventHandlers". ตัวจัดการอีเวนท์ไม่ได้จัดเตรียมไว้ตามค่าเริ่มต้น; คุณต้องสร้างมันขึ้นมา.

เราสามารถอ้างอิง reference ได้ใน void OnEnable และ voide OnDisable ดังนี้:

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    // ลงทะเบียนคลาสตัวจัดการอีเวนท์. และเพิ่มอีเวนท์,
    // ไปยัง Listener EXILED_Events ดังนั้นเราจึงได้รับอีเวนท์.
    EventHandler = new EventHandlers();
    Player.Verified += EventHandler.PlayerVerified;
}

public override OnDisable()
{
    // ทำให้สามารถอัปเดตได้แบบไดนามิก.
    // เราทำเช่นนี้โดยการลบ Listener สำหรับอีเวนท์ออก จากนั้นจึงลบล้างตัวจัดการอีเวนท์.
    // กระบวนการนี้จะต้องทำซ้ำสำหรับแต่ละอีเวนท์.
    Player.Verified -= EventHandler.PlayerVerified;
    EventHandler = null;
}
```

และในคลาส EventHandlers เราจะทำ:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {

    }
}
```
ตอนนี้เราได้ทําสำเร็จในการเชื่อมต่อกับอีเวนท์ VerifiedEventArgs ซึ่งจะเริ่มทำงานเมื่อผู้เล่นได้รับการรับรองความถูกต้องหลังจากเข้าร่วมเซิร์ฟเวอร์! 
สิ่งสำคัญคือต้องทราบว่าทุกอีเวนท์มีอาร์กิวเมนต์อีเวนท์ที่แตกต่างกัน, และอาร์กิวเมนต์อีเวนท์แต่ละประเภทมีคุณสมบัติที่แตกต่างกันที่เกี่ยวข้อง.

EXILED มีฟังก์ชั่น broadcast อยู่แล้ว, ดังนั้นเรามาใช้ในอีเวนท์ของเรากันดีกว่า:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        ev.Player.Broadcast(5, "<color=lime>Welcome to my cool server!</color>");
    }
}
```

ตามที่กล่าวไว้ข้างต้น, ทุกอีเวนท์มีข้อโต้แย้งที่แตกต่างกัน. ด้านล่างนี้เป็นอีเวนท์อื่นที่จะปิดประตู Tesla สำหรับผู้เล่น Nine-Tailed Fox.

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
    // อย่าลืมว่าอีเวนท์จะต้องถูกตัดการเชื่อมต่อและทำให้เป็น null ในเมธอด disable.
    Player.TriggeringTesla -= EventHandler.TriggeringTesla;
    EventHandler = null;
}
```

และในคลาส EventHandlers.

`EventHandlers.cs`
```csharp
public class EventHandlers
{
    public void TriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        // ปิดการใช้งานอีเวนท์สำหรับผู้เล่นบุคลากรของ foundation.
        // ซึ่งสามารถทำได้โดยการตรวจสอบฝั่งผู้เล่น.
        if (ev.Player.Role.Side == Side.Mtf) {
            // ปิดการใช้งานทริกเกอร์ tesla โดยการตั้งค่า ev.IsTriggerable เป็น false.
            // ผู้เล่นที่เป็น MTF จะไม่เปิด Tesla อีกต่อไป.
            ev.IsTriggerable = false;
        }
    }
}
```


### Configs
ปลั๊กอิน Exiled ส่วนใหญ่มีการกำหนดค่า configs. การกำหนดค่า configs ช่วยให้ผู้ดูแลเซิร์ฟเวอร์สามารถแก้ไขปลั๊กอินตามความต้องการได้, แม้ว่าจะจำกัดอยู่เพียงการกำหนดค่าที่ผู้พัฒนาปลั๊กอินจัดเตรียมไว้ให้ก็ตาม.

ขั้นแรกให้สร้างคลาส `config.cs` และเปลี่ยนการสืบทอดปลั๊กอินของคุณจาก `Plugin<>` เป็น `Plugin<Config>`

ตอนนี้คุณต้องทำให้การกำหนดค่า config นั้นสืบทอด `IConfig`. หลังจากสืบทอดจาก `IConfig`, แล้วให้เพิ่มคุณสมบัติให้กับคลาสชื่อ 'IsEnabled` และ 'Debug'. คลาส Config ของคุณควรมีลักษณะดังนี้:

```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
```

คุณสามารถเพิ่มตัวเลือกการกำหนดค่า config ใดๆ ในนั้นและอ้างอิงดังนี้:

`Config.cs`
```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        public string TextThatINeed { get; set; } = "this is the default";
    }
```

`MainClass.cs`
```csharp
   public override OnEnabled()
   {
        Log.Info(Config.TextThatINeed);
   }
```

แล้วก็ขอแสดงความยินดีด้วย! คุณได้สร้าง Exiled Plugin ตัวแรกของคุณแล้ว! สิ่งสำคัญคือต้องทราบว่าปลั๊กอินทั้งหมด **ต้อง** มีการกำหนดค่า config IsEnabled. 
การกำหนดค่า config นี้ทำให้เจ้าของเซิร์ฟเวอร์สามารถเปิดใช้งานและปิดใช้งานปลั๊กอินได้ตามต้องการ. Exiled Loader จะอ่านการกำหนดค่า config IsEnabled (ปลั๊กอินของคุณไม่จำเป็นต้องตรวจสอบว่า `IsEnabled == true` หรือไม่.).

### เเล้วอย่างไรต่อ?
หากคุณต้องการข้อมูลเพิ่มเติมคุณควรเข้าร่วม [discord ของเรา!](https://discord.gg/PyUkWTg)

เรามีช่อง #dev-resources ที่คุณอาจพบว่ามีประโยชน์, เช่นเดียวกับผู้มีส่วนร่วมที่ Exiled และนักพัฒนาปลั๊กอินที่ยินดีช่วยเหลือคุณในการสร้างปลั๊กอินของคุณ.

หรือคุณสามารถอ่านอีเวนท์ทั้งหมดที่เรามี! คุณสามารถที่จะตรวจสอบได้จาก [ตรงนี้!](https://github.com/galaxy119/EXILED/tree/master/Exiled.Events/EventArgs)